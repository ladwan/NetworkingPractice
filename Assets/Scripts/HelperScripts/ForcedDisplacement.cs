using System;
using UnityEngine;
using UnityEngine.AI;
using ForeverFight.GameMechanics.Movement;

namespace ForeverFight.HelperScripts
{
    /// <summary>
    /// World-space pull / knockback, replacing the grid-based ProceduralGridManipulation.
    /// Distances are world units (1 unit ~= 1 old grid cell). A knockback that reaches
    /// the NavMesh edge counts as hitting a wall - the same semantics as stepping off
    /// the old grid dictionary. As before, the attacker sends the override; the victim's
    /// client applies it locally and echoes its confirmed position back.
    /// </summary>
    public class ForcedDisplacement : MonoBehaviour
    {
        [SerializeField] private float minimumSpacing = 1.0f; // Old "adjacent cell" spacing.

        public static ForcedDisplacement Instance { get; private set; }

        /// <summary>Attacker-local wall-hit hook (Haymaker's OffBalance bonus subscribes here).</summary>
        public Action EnemyHitAWallAction { get; set; }


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.Log("More than 1 ForcedDisplacement detected, destroying self...");
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

        public float DistanceBetweenPlayers()
        {
            var spawnManager = PlayerSpawnManager.Instance;
            Vector3 offset = spawnManager.OpponentSpawn.transform.position - spawnManager.LocalPlayerSpawn.transform.position;
            offset.y = 0f;
            return offset.magnitude;
        }

        /// <summary>Pulls the opponent straight toward the local player by pullDistance,
        /// never closer than minimumSpacing. Pulls can't hit walls (matches old behavior).</summary>
        public void PullEnemy(float pullDistance)
        {
            var spawnManager = PlayerSpawnManager.Instance;
            Vector3 localPosition = spawnManager.LocalPlayerSpawn.transform.position;
            Vector3 opponentPosition = spawnManager.OpponentSpawn.transform.position;

            Vector3 direction = opponentPosition - localPosition;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f)
            {
                return;
            }
            direction.Normalize();

            Vector3 target = opponentPosition - direction * pullDistance;
            if ((target - localPosition).magnitude < minimumSpacing)
            {
                target = localPosition + direction * minimumSpacing;
            }

            SendOverride(ClampToNavMesh(target, opponentPosition));
        }

        /// <summary>Pulls the opponent to exactly minimumSpacing away (melee range).</summary>
        public void PullEnemyToMeleeRange()
        {
            PullEnemy(DistanceBetweenPlayers());
        }

        /// <summary>Pushes the opponent directly away from the local player. Fires
        /// EnemyHitAWallAction (attacker-local) when the NavMesh edge cuts the push short.</summary>
        public void KnockbackEnemy(float knockbackDistance)
        {
            var spawnManager = PlayerSpawnManager.Instance;
            Vector3 localPosition = spawnManager.LocalPlayerSpawn.transform.position;
            Vector3 opponentPosition = spawnManager.OpponentSpawn.transform.position;

            Vector3 direction = opponentPosition - localPosition;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.0001f)
            {
                direction = spawnManager.OpponentSpawn.transform.forward;
                direction.y = 0f;
            }
            direction.Normalize();

            Vector3 target = opponentPosition + direction * knockbackDistance;
            Vector3 finalPosition;

            if (NavMesh.Raycast(opponentPosition, target, out NavMeshHit hit, NavMesh.AllAreas))
            {
                finalPosition = hit.position;
                EnemyHitAWallAction?.Invoke();
            }
            else
            {
                finalPosition = ClampToNavMesh(target, opponentPosition);
            }

            SendOverride(finalPosition);
        }

        /// <summary>Receiver side: applies a forced move to the local player and echoes
        /// the confirmed position (preserves the old OverridePlayerPosition echo).</summary>
        public void ApplyForcedMoveToLocalPlayer(Vector3 destination)
        {
            Vector3 clamped = ClampToNavMesh(destination, PlayerSpawnManager.Instance.LocalPlayerSpawn.transform.position);
            PlayerSpawnManager.Instance.OverrideLocalPlayerPosition(clamped);
            ClientSend.UpdatePlayerPosition(clamped);
        }


        private static Vector3 ClampToNavMesh(Vector3 target, Vector3 fallback)
        {
            if (NavMesh.SamplePosition(target, out NavMeshHit hit, NavPathUtility.NavSampleRadius, NavMesh.AllAreas))
            {
                var position = hit.position;
                position.y = 0f;
                return position;
            }

            return fallback;
        }

        private static void SendOverride(Vector3 position)
        {
            ClientSend.OverrideOpponentPosition(position);
        }
    }
}
