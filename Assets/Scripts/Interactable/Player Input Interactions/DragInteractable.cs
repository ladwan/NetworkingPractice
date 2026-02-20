using ForeverFight.GameMechanics.Movement;
using ForeverFight.Interactable.PlayerInputInteractions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.Interactable.PlayerInputInteractions
{
    public class DragInteractable : BaseInteractable
    {
        [SerializeField] private DragMovement dragMovementREF;


        protected override void OnClicked()
        {
            dragMovementREF.UpdateDragMover();
            // Debug.Log("DragInteractable was clicked!");
        }

        protected override void OnUnclicked()
        {
            dragMovementREF.OnMouseUpCustom();
            //Debug.Log("DragInteractable was released!"); 
        }
    }
}