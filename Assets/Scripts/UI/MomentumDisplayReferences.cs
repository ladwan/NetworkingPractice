using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForverFight.Interactable.Abilities;
using ForverFight.FlowControl;

namespace ForverFight.Ui
{
    public class MomentumDisplayReferences : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text storedMomentumDisplayTmp = null;

        private Action onMoveConfirmedSub = null;
        private Action onTurnEndSub = null;
        private Momentum momentumREF = null;


        public TMP_Text StoredMomentumDisplayTmp { get => storedMomentumDisplayTmp; set => storedMomentumDisplayTmp = value; }

        public Momentum MomentumREF { get => momentumREF; set => momentumREF = value; }


        public void SubscribeToOnMoveConfirmed(Momentum momentumInstance)
        {
            momentumREF = momentumInstance;

            if (onMoveConfirmedSub == null)
            {
                momentumREF.OnMoveConfirmed += UpdateMomentumDisplayText;
            }
            if (onTurnEndSub == null)
            {
                PlayerTurnManager.Instance.OnTurnEnd += UpdateMomentumDisplayText;
            }
        }


        private void UpdateMomentumDisplayText()
        {
            StoredMomentumDisplayTmp.text = momentumREF.StoredMomentum.ToString();
        }

        private void OnDestroy()
        {
            if (onMoveConfirmedSub != null)
            {
                momentumREF.OnMoveConfirmed -= UpdateMomentumDisplayText;
            }
            if (onTurnEndSub != null)
            {
                PlayerTurnManager.Instance.OnTurnEnd -= UpdateMomentumDisplayText;
            }
        }
    }
}
