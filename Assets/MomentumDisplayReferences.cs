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
        private Coroutine coroutineREF;


        public TMP_Text StoredMomentumDisplayTmp { get => storedMomentumDisplayTmp; set => storedMomentumDisplayTmp = value; }

        public Momentum MomentumREF { get => momentumREF; set => momentumREF = value; }

        protected void OnEnable()
        {
            BeginCoroutine();
        }


        private void BeginCoroutine()
        {
            EndCoroutine();
            coroutineREF = StartCoroutine(WaitForMomentumReference());
        }

        private void EndCoroutine()
        {
            if (coroutineREF != null)
            {
                StopCoroutine(coroutineREF);
                coroutineREF = null;
            }
        }

        private IEnumerator WaitForMomentumReference()
        {
            yield return new WaitUntil(() => momentumREF);

            momentumREF.OnMoveConfirmed += UpdateMomentumDisplayText;
        }

        private void UpdateMomentumDisplayText()
        {
            StoredMomentumDisplayTmp.text = momentumREF.StoredMomentum.ToString();
        }
    }
}
