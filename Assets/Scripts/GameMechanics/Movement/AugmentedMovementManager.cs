using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.Movement
{
    public class AugmentedMovementManager : MonoBehaviour
    {
        private bool movementIsAugmented = false;
        private static AugmentedMovementManager instance = null;
        private IAugmentedMovement augmentedMovementLogicREF = null;


        public bool MovementIsAugmented => movementIsAugmented;

        public static AugmentedMovementManager Instance => instance;

        public IAugmentedMovement AugmentedMovementLogicREF { get => augmentedMovementLogicREF; set => augmentedMovementLogicREF = value; }


        protected void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.Log("More Than 1 Augmented Movement Manager detected, Destroying self...");
                Destroy(instance);
            }
        }

        public void ToggleAugmentMovement(IAugmentedMovement movementREF)
        {
            if (!movementIsAugmented)
            {
                movementIsAugmented = true;
                augmentedMovementLogicREF = movementREF;

                return;
            }

            movementIsAugmented = false;
            augmentedMovementLogicREF = null;
        }

        public void DetermineIfMovementIsAugmented()
        {
            if (augmentedMovementLogicREF == null)
            {
                FloorGrid.Instance.ConfirmMove();
                return;
            }

            FloorGrid.Instance.ConfirmMove();
            augmentedMovementLogicREF.BeginMovement();
        }
    }
}
