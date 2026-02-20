using ForeverFight.GameMechanics.Movement;
using ForeverFight.Interactable.PlayerInputInteractions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.Interactable.PlayerInputInteractions
{
    public class GridInteractable : BaseInteractable
    {
        [SerializeField] private GridPoint gridPointREF;


        protected override void OnClicked()
        {
            gridPointREF.OnMouseEnterCustom();
        }

        protected override void OnUnclicked()
        {
        }
    }
}