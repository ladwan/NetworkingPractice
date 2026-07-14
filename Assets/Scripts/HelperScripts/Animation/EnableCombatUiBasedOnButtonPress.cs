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
            // The free movement system needs no drag token to enable - the character's
            // drag collider is always present and the planner gates on UI state.
            movementUiREF.SetActive(true);
        }

        private void ActivateAttackUi()
        {
            attackUiREF.SetActive(true);
        }
    }
}
