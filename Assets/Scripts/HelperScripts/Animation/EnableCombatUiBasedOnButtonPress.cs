using ForeverFight.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ForeverFight.HelperScripts.Animation
{
    public class EnableCombatUiBasedOnButtonPress : MonoBehaviour
    {
        [SerializeField] private GameObject dragMoverREF = null;
        [SerializeField] private GameObject movementUiREF = null;
        [SerializeField] private GameObject attackUiREF = null;


        private Action uiEnabler = null;


        public void EnableCombatUi(bool movementButtonClicked)
        {
            uiEnabler = movementButtonClicked ? ActivateMovementUi : ActivateAttackUi;
            uiEnabler?.Invoke();
        }

        private void ActivateMovementUi()
        {
            dragMoverREF.SetActive(true);
            movementUiREF.SetActive(true);
        }

        private void ActivateAttackUi()
        {
            attackUiREF.SetActive(true);
        }
    }
}
