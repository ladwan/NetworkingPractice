using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ForeverFight.FlowControl;
using ForeverFight.Ui;

namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// Owns the planned move for the free click-and-drag movement system.
    /// While the player drags, computes a NavMesh path from the local character to the
    /// drag point, truncates it at the AP distance cap, keeps it clear of the opponent
    /// and syncs the pending AP cost. The plan persists after drag release until it is
    /// confirmed (MovementNetworkBridge) or canceled.
    /// </summary>
    public class MovementPlanner : MonoBehaviour
    {
        // Recompute throttle: skip work unless the target moved and a little time passed.
        [SerializeField] private float recomputeDistanceThreshold = 0.15f;
        [SerializeField] private float recomputeInterval = 0.05f;
        // Free-space equivalent of "can't stop on the opponent's square".
        [SerializeField] private float opponentClearanceRadius = 0.75f;

        private readonly List<Vector3> plannedWaypoints = new List<Vector3>();
        private NavMeshPath reusablePath = null;
        private Vector3 lastComputedTarget = Vector3.positiveInfinity;
        private float lastComputeTime = -1f;
        private float plannedPathLength = 0f;
        private bool isDragging = false;

        public static MovementPlanner Instance { get; private set; }

        public event Action<float, int> OnPlanUpdated;
        public event Action OnPlanCleared;
        public event Action<float> OnMoveConfirmed;
        public event Action OnMoveCompleted;

        public IReadOnlyList<Vector3> PlannedWaypoints => plannedWaypoints;

        public float PlannedPathLength => plannedPathLength;

        public bool IsDragging => isDragging;

        public bool HasPlan => plannedWaypoints.Count > 1 && plannedPathLength >= ApDistanceBank.Instance.DeadZone;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.Log("More than 1 MovementPlanner detected, destroying self...");
                Destroy(this);
                return;
            }

            reusablePath = new NavMeshPath();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public bool BeginDrag()
        {
            if (PlayerTurnManager.Instance == null || !PlayerTurnManager.Instance.IsLocalPlayersTurn)
            {
                return false;
            }

            if (CombatUiStatesManager.Instance != null
                && CombatUiStatesManager.Instance.CurrentCombatUiState != CombatUiStatesManager.CombatUiState.movement)
            {
                return false;
            }

            if (MovementExecutor.Instance.IsPlaying || !ApDistanceBank.Instance.CanPlan())
            {
                return false;
            }

            isDragging = true;
            lastComputedTarget = Vector3.positiveInfinity;
            return true;
        }

        /// <summary>
        /// Re-grabs a released plan from its endpoint marker so the drag continues where it
        /// left off. The existing plan stays displayed until the pointer moves, then re-plans
        /// through the normal UpdateDrag path with the full (remaining + pending) AP budget.
        /// </summary>
        public bool ResumeDrag()
        {
            if (!HasPlan)
            {
                return false;
            }

            return BeginDrag();
        }

        /// <summary>Called every held frame by the input layer with a sampled ground point.</summary>
        public void UpdateDrag(Vector3 groundPoint)
        {
            if (!isDragging || PlayerTurnManager.Instance == null || !PlayerTurnManager.Instance.IsLocalPlayersTurn)
            {
                return;
            }

            if (Vector3.Distance(groundPoint, lastComputedTarget) < recomputeDistanceThreshold
                || Time.unscaledTime - lastComputeTime < recomputeInterval)
            {
                return;
            }

            var spawnManager = PlayerSpawnManager.Instance;
            if (spawnManager == null || spawnManager.LocalPlayerSpawn == null)
            {
                return;
            }

            Vector3 start = spawnManager.LocalPlayerSpawn.transform.position;
            if (!NavMesh.SamplePosition(start, out NavMeshHit startHit, NavPathUtility.NavSampleRadius, NavMesh.AllAreas))
            {
                return;
            }

            if (!NavMesh.CalculatePath(startHit.position, groundPoint, NavMesh.AllAreas, reusablePath)
                || reusablePath.status == NavMeshPathStatus.PathInvalid)
            {
                return; // Keep the last valid plan.
            }

            lastComputedTarget = groundPoint;
            lastComputeTime = Time.unscaledTime;

            float maxDistance = ApDistanceBank.Instance.MaxPlannableDistance();
            NavPathUtility.TruncateAtDistance(reusablePath.corners, maxDistance, plannedWaypoints);

            if (spawnManager.OpponentSpawn != null)
            {
                NavPathUtility.PullBackFromPoint(
                    plannedWaypoints, spawnManager.OpponentSpawn.transform.position, opponentClearanceRadius);
            }

            NavPathUtility.FlattenY(plannedWaypoints, 0f);
            plannedPathLength = NavPathUtility.PathLength(plannedWaypoints);

            int cost = ApDistanceBank.Instance.CostForDistance(plannedPathLength);
            ApDistanceBank.Instance.SyncPendingCost(cost);
            OnPlanUpdated?.Invoke(plannedPathLength, cost);
        }

        public void EndDrag()
        {
            if (!isDragging)
            {
                return;
            }

            isDragging = false;

            if (plannedPathLength < ApDistanceBank.Instance.DeadZone)
            {
                CancelPlan(); // Misclick: refund and clear, no AP charge.
            }
        }

        /// <summary>Refunds pending AP and clears the plan (back button, turn expiry, misclick).</summary>
        public void CancelPlan()
        {
            isDragging = false;
            ApDistanceBank.Instance.RefundAll();
            ClearPlanInternal();
        }

        /// <summary>Clears the plan without touching AP (after a confirmed move committed it).</summary>
        public void ClearPlanAfterConfirm()
        {
            isDragging = false;
            ClearPlanInternal();
        }

        public void NotifyMoveConfirmed(float pathDistance)
        {
            OnMoveConfirmed?.Invoke(pathDistance);
        }

        public void NotifyMoveCompleted()
        {
            OnMoveCompleted?.Invoke();
        }


        private void ClearPlanInternal()
        {
            plannedWaypoints.Clear();
            plannedPathLength = 0f;
            lastComputedTarget = Vector3.positiveInfinity;
            OnPlanCleared?.Invoke();
        }
    }
}
