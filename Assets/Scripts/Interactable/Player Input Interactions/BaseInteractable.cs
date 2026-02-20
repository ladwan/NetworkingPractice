using ForeverFight.GameMechanics.Movement;
using ForeverFight.HelperScripts;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.Interactable.PlayerInputInteractions
{
    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        public bool IsClicked { get; private set; }


        public void Clicked()
        {
            if (!SafetyNet.IsValid(BasePlayerInputInteraction.Instance, "BasePlayerInputInteraction.Instance On Clicked"))
            {
                return;
            }

            BasePlayerInputInteraction.Instance.OnTouchEnd += Unclicked;
            IsClicked = true;
            OnClicked();
            //Debug.Log("[INTERACTABLE] Ive been CLICKED !");
        }


        private void Unclicked(Vector2 screenPos)
        {

            IsClicked = false;
            BasePlayerInputInteraction.Instance.OnTouchEnd -= Unclicked;
            OnUnclicked();
            //Debug.Log("[INTERACTABLE] Ive been UNCLICKED !");
        }

        private void OnDestroy()
        {
            if (!SafetyNet.IsValid(BasePlayerInputInteraction.Instance, "BasePlayerInputInteraction.Instance On Destroy"))
            {
                return;
            }

            BasePlayerInputInteraction.Instance.OnTouchEnd -= Unclicked;
        }


        protected virtual void OnClicked()
        {
        }

        protected virtual void OnUnclicked()
        {
        }
    }
}