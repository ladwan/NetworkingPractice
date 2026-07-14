using UnityEngine;
using ForeverFight.Ui;

namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// Bridges path distance to the existing AP-light UI. Each AP unlocks a bucket of
    /// UnitsPerAp world-units of movement. The Speedster's passive AP is not a separate
    /// budget: the plannable pool is passive + main combined. Passive AP is always spent
    /// first (and refunded last on backtrack), while each pool keeps its own distinct
    /// light bar - blinks land on whichever bar the AP actually came from.
    /// </summary>
    public class ApDistanceBank : MonoBehaviour
    {
        // Tuned so crossing the ~10 unit arena costs ~9 AP (1 unit ~= 1 old grid cell).
        [SerializeField] private float unitsPerAp = 1.1f;
        // Drags shorter than this are treated as a misclick and cost nothing.
        [SerializeField] private float deadZone = 0.35f;

        private int pendingPassiveCost = 0;
        private int pendingMainCost = 0;

        public static ApDistanceBank Instance { get; private set; }

        public float UnitsPerAp => unitsPerAp;

        public float DeadZone => deadZone;

        public int PendingCost => pendingPassiveCost + pendingMainCost;


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
            return MainRemaining() + PassiveRemaining();
        }

        public bool CanPlan()
        {
            return RemainingAp() + PendingCost > 0;
        }

        public float MaxPlannableDistance()
        {
            return (RemainingAp() + PendingCost) * unitsPerAp;
        }

        /// <summary>
        /// Blinks or refunds AP lights until the pending spend matches newCost. Spending
        /// drains the passive bar first, then main; shrinking refunds in reverse (main
        /// back first, passive last) so the passive is always the first AP consumed.
        /// </summary>
        public void SyncPendingCost(int newCost)
        {
            var apManager = ActionPointsManager.Instance;

            while (PendingCost < newCost)
            {
                if (PassiveRemaining() > 0)
                {
                    apManager.ApMovementBlink(PassiveApLists);
                    pendingPassiveCost++;
                }
                else if (MainRemaining() > 0)
                {
                    apManager.ApMovementBlink(apManager.MainApLists);
                    pendingMainCost++;
                }
                else
                {
                    break;
                }
            }

            while (PendingCost > newCost)
            {
                if (pendingMainCost > 0)
                {
                    apManager.UpdateAP(apManager.MainApLists, 1);
                    apManager.UpdateBlinkingAP(apManager.MainApLists);
                    pendingMainCost--;
                }
                else if (pendingPassiveCost > 0)
                {
                    apManager.UpdateAP(PassiveApLists, 1);
                    apManager.UpdateBlinkingAP(PassiveApLists);
                    pendingPassiveCost--;
                }
                else
                {
                    break;
                }
            }
        }

        public void Commit()
        {
            var apManager = ActionPointsManager.Instance;

            if (pendingPassiveCost > 0 && PassiveApLists != null)
            {
                apManager.MoveWasConfirmed(PassiveApLists);
            }
            if (pendingMainCost > 0)
            {
                apManager.MoveWasConfirmed(apManager.MainApLists);
            }

            pendingPassiveCost = 0;
            pendingMainCost = 0;
        }

        public void RefundAll()
        {
            var apManager = ActionPointsManager.Instance;

            if (pendingMainCost > 0)
            {
                apManager.ResetApUsage(apManager.MainApLists);
            }
            if (pendingPassiveCost > 0 && PassiveApLists != null)
            {
                apManager.ResetApUsage(PassiveApLists);
            }

            pendingPassiveCost = 0;
            pendingMainCost = 0;
        }


        // The passive pool only exists while the local character's movement passive is
        // registered (e.g. the Speedster's FasterPassive registers itself when active).
        private ApReferenceLists PassiveApLists
        {
            get
            {
                var provider = ActionPointsManager.Instance.MovementPassiveApProvider;
                return provider != null ? provider.PassiveApLists : null;
            }
        }

        private int PassiveRemaining()
        {
            var passiveLists = PassiveApLists;
            return passiveLists != null ? passiveLists.UpdateValueOfRelevantAp(0) : 0;
        }

        private int MainRemaining()
        {
            return ActionPointsManager.Instance.MainApLists.UpdateValueOfRelevantAp(0);
        }
    }
}
