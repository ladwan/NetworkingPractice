using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ForeverFight.Networking;
using ForeverFight.FlowControl;
using ForeverFight.HelperScripts;
using ForeverFight.GameMechanics.Movement;
using ForeverFight.Interactable.Characters;

namespace ForeverFight.Interactable.Abilities
{
    // TODO: rework as a proper NavMesh dash post-refactor. This ability is currently
    // dormant (Haste's ToggleAugmentMovement call is commented out); this rewrite only
    // keeps it compiling against the free movement system with equivalent structure:
    // below the distance threshold it confirms the planned move normally, above it it
    // zips through random arena points before finishing at the planned endpoint.
    public class MoveToRandomGPs : MonoBehaviour, IAugmentedMovement
    {
        [SerializeField]
        private RandomNavPointProvider randomNavPointProviderREF = null;
        [SerializeField]
        private UnityEvent movementBeganEvent = new UnityEvent();
        [SerializeField]
        private UnityEvent movementCompletedEvent = new UnityEvent();
        [SerializeField] private Speedster speedsterREF = null;
        // Old gate was "fewer than 10 squares moved"; 1 square = 1 world unit.
        [SerializeField] private float dashDistanceThreshold = 10f;
        [SerializeField] private int randomPointCount = 4;


        public void BeginMovement()
        {
            if (LocalStoredNetworkData.distanceMovedThisInstance < dashDistanceThreshold)
            {
                ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating();
                MovementNetworkBridge.Instance.ConfirmLocalMove();
                LocalStoredNetworkData.distanceMovedThisInstance = 0f;
                return;
            }

            var planner = MovementPlanner.Instance;
            if (!planner.HasPlan)
            {
                return;
            }

            movementBeganEvent?.Invoke();

            // Dash route: current position -> random points -> planned endpoint.
            var dashPoints = randomNavPointProviderREF.GetRandomPoints(randomPointCount);
            var waypoints = new List<Vector3> { PlayerSpawnManager.Instance.LocalPlayerSpawn.transform.position };
            waypoints.AddRange(dashPoints);
            waypoints.Add(planner.PlannedWaypoints[planner.PlannedWaypoints.Count - 1]);

            LocalStoredNetworkData.distanceMovedThisInstance = 0f;
            AugmentedMovementManager.Instance.CurrentlyDoingAugmentedMovement = true;

            MovementExecutor.Instance.Play(
                PlayerSpawnManager.Instance.LocalPlayerSpawn,
                speedsterREF,
                waypoints,
                () =>
                {
                    AugmentedMovementManager.Instance.CurrentlyDoingAugmentedMovement = false;
                    movementCompletedEvent?.Invoke();
                });
        }
    }
}
