using System.Collections.Generic;
using UnityEngine;
using ForeverFight.FlowControl;
using ForeverFight.Networking;

namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// The seam between the movement system and the custom TCP netcode.
    /// Outbound: confirms the local plan, sends the full waypoint list in one packet
    /// (before local playback starts, so the path always precedes a later endTurn on
    /// the same TCP stream), then plays the move locally.
    /// Inbound: replays a received waypoint list on the opponent's spawn - the exact
    /// same executor path, so both clients render the same trajectory.
    /// </summary>
    public class MovementNetworkBridge : MonoBehaviour
    {
        public const int MaxWaypointsPerMove = 256;

        public static MovementNetworkBridge Instance { get; private set; }


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.Log("More than 1 MovementNetworkBridge detected, destroying self...");
                Destroy(this);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        /// <summary>Confirm-button entry point (via AugmentedMovementManager).</summary>
        public void ConfirmLocalMove()
        {
            var planner = MovementPlanner.Instance;
            var executor = MovementExecutor.Instance;

            if (!planner.HasPlan || executor.IsPlaying)
            {
                return;
            }

            var waypoints = new List<Vector3>(planner.PlannedWaypoints);
            float pathDistance = planner.PlannedPathLength;
            int apSpent = ApDistanceBank.Instance.PendingCost;

            ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating();

            // Send before local playback: TCP ordering guarantees the opponent has the
            // full move before any subsequent endTurn from this client.
            ClientSend.SendMovementPath(waypoints, pathDistance, apSpent);

            ApDistanceBank.Instance.Commit();
            planner.NotifyMoveConfirmed(pathDistance);

            var localSpawn = PlayerSpawnManager.Instance.LocalPlayerSpawn;
            var localCharacter = LocalStoredNetworkData.GetLocalCharacter();

            executor.Play(localSpawn, localCharacter, waypoints, () =>
            {
                Vector3 finalPosition = waypoints[waypoints.Count - 1];
                ClientSend.UpdatePlayerPosition(finalPosition); // Cheap drift insurance.
                planner.ClearPlanAfterConfirm();
                planner.NotifyMoveCompleted();
                ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating();
            });
        }

        /// <summary>Called by ClientHandle with a validated waypoint list from the opponent.</summary>
        public void ReplayRemoteMove(List<Vector3> waypoints, float totalPathDistance, int apSpent)
        {
            if (waypoints == null || waypoints.Count < 2)
            {
                Debug.LogError("Received a movement path with fewer than 2 waypoints, ignoring");
                return;
            }

            // Snapping to the sender's start position erases any accumulated drift.
            PlayerSpawnManager.Instance.UpdateOpponentPosition(waypoints[0]);

            var opponentSpawn = PlayerSpawnManager.Instance.OpponentSpawn;
            var opponentCharacter = LocalStoredNetworkData.GetOpponentCharacter();

            MovementExecutor.Instance.Play(opponentSpawn, opponentCharacter, waypoints, () =>
            {
                MovementPlanner.Instance.NotifyMoveCompleted();
            });
        }
    }
}
