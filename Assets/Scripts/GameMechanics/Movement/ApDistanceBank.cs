using UnityEngine;
using ForeverFight.Ui;

namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// Bridges path distance to the existing AP-light UI. Each AP unlocks a bucket of
    /// UnitsPerAp world-units of movement. Reuses the exact blink / refund / commit call
    /// pairs the grid system used on ActionPointsManager, so the Speedster passive pool
    /// routing (via CurrentApReferenceListsREF) keeps working untouched.
    /// </summary>
    public class ApDistanceBank : MonoBehaviour
    {
        // Tuned so crossing the ~10 unit arena costs ~9 AP (1 unit ~= 1 old grid cell).
        [SerializeField] private float unitsPerAp = 1.1f;
        // Drags shorter than this are treated as a misclick and cost nothing.
        [SerializeField] private float deadZone = 0.35f;

        private int pendingCost = 0;

        public static ApDistanceBank Instance { get; private set; }

        public float UnitsPerAp => unitsPerAp;

        public float DeadZone => deadZone;

        public int PendingCost => pendingCost;


        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.Log("More than 1 ApDistanceBank detected, destroying self...");
                Destroy(this);
            }
        }

        public int CostForDistance(float distance)
        {
            if (distance < deadZone)
            {
                return 0;
            }

            return Mathf.Max(1, Mathf.CeilToInt(distance / unitsPerAp));
        }

        public int RemainingAp()
        {
            var apManager = ActionPointsManager.Instance;
            var currentList = apManager.CurrentApReferenceListsREF != null
                ? apManager.CurrentApReferenceListsREF
                : apManager.MainApLists;

            return currentList.UpdateValueOfRelevantAp(0);
        }

        public bool CanPlan()
        {
            return RemainingAp() + pendingCost > 0;
        }

        public float MaxPlannableDistance()
        {
            return (RemainingAp() + pendingCost) * unitsPerAp;
        }

        /// <summary>
        /// Blinks or refunds AP lights until the pending spend matches newCost.
        /// Spending uses BlinkCurrentListReference (deduct + blink), shrinking uses the
        /// UpdateAP(+1) / UpdateBlinkingAP pair — identical to the old backtrack refund.
        /// </summary>
        public void SyncPendingCost(int newCost)
        {
            var apManager = ActionPointsManager.Instance;

            while (pendingCost < newCost)
            {
                if (RemainingAp() <= 0)
                {
                    break;
                }

                apManager.BlinkCurrentListReference();
                pendingCost++;
            }

            while (pendingCost > newCost)
            {
                apManager.UpdateAP(apManager.CurrentApReferenceListsREF, 1);
                apManager.UpdateBlinkingAP(apManager.CurrentApReferenceListsREF);
                pendingCost--;
            }
        }

        public void Commit()
        {
            var apManager = ActionPointsManager.Instance;
            if (apManager.CurrentApReferenceListsREF != null)
            {
                apManager.MoveWasConfirmed(apManager.CurrentApReferenceListsREF);
            }
            pendingCost = 0;
        }

        public void RefundAll()
        {
            var apManager = ActionPointsManager.Instance;
            if (pendingCost > 0 && apManager.CurrentApReferenceListsREF != null)
            {
                apManager.ResetApUsage(apManager.CurrentApReferenceListsREF);
            }
            pendingCost = 0;
        }
    }
}
