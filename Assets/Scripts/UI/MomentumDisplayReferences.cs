using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForverFight.Interactable.Abilities;
using System;

namespace ForverFight.Ui
{
    public class MomentumDisplayReferences : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text storedMomentumDisplayTmp = null;


        private Momentum momentumREF = null;


        public TMP_Text StoredMomentumDisplayTmp { get => storedMomentumDisplayTmp; set => storedMomentumDisplayTmp = value; }

        public Momentum MomentumREF { get => momentumREF; set => momentumREF = value; }


        public void SubscribeToOnMoveConfirmed(Momentum momentumInstance)
        {
            momentumREF = momentumInstance;
            momentumREF.OnMoveConfirmed += UpdateMomentumDisplayText;
        }


        private void UpdateMomentumDisplayText()
        {
            StoredMomentumDisplayTmp.text = momentumREF.StoredMomentum.ToString();
        }
    }
}
