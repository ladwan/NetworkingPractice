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
        [SerializeField] private GameObject unclickedDragMoverModel = null;
        [SerializeField] private GameObject clickedDragMoverModel = null;


        protected override void OnClicked()
        {
            dragMovementREF.UpdateDragMover();

            clickedDragMoverModel.SetActive(true);
            unclickedDragMoverModel.SetActive(false);
            // Debug.Log("DragInteractable was clicked!");
        }

        protected override void OnUnclicked()
        {
            dragMovementREF.OnMouseUpCustom();

            unclickedDragMoverModel.SetActive(true);
            clickedDragMoverModel.SetActive(false);
            //Debug.Log("DragInteractable was released!"); 
        }
    }
}