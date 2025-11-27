using ForeverFight.Interactable.Characters;
using ForeverFight.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace ForeverFight.GameMechanics.Movement
{
    public class CurveMoveSpeed : MonoBehaviour
    {
        [SerializeField]
        private AnimationCurve curve = null;
        [SerializeField]
        private float moveSpeed = 0.0f;
        [SerializeField]
        private float curveEvaluationSpeed = 1.0f;


        private bool currentlyEvaluating = false;
        private float time = 0.0f;
        private Character currentCharacter = null;
        private int curveIndex = -1;
        private AnimationCurve currentCurve = null;


        public bool CurrentlyEvaluating => currentlyEvaluating;


        public void DetermineCurveFromMovementSegment(Character playerToMove, List<Vector3> movementSegment)
        {
            if (playerToMove == null)
            {
                Debug.LogError("Player to move was null, this is NOT allowed!");
                return;
            }

            if (AugmentedMovementManager.Instance.MovementIsAugmented)
            {
                return;
            }

            currentCharacter = playerToMove;

            //You need to not use Local, this can be called by remote player too 
            curveIndex = movementSegment.Count - 1;

            //currentCurve = currentCharacter.MovementCurves[curveIndex];
            currentCurve = currentCharacter.MovementAnimCurves[currentCharacter.MovementIndex].movementCurves[curveIndex];

            time = 0;
            currentlyEvaluating = true;
        }


        private void Update()
        {
            if (currentlyEvaluating)
            {
                time += Time.deltaTime * curveEvaluationSpeed;
                currentCharacter.MoveSpeed = currentCurve.Evaluate(time);
                currentCharacter.CharacterAnimationReferences.CharacterAnimator.SetFloat("CharSpeed", currentCharacter.MoveSpeed);

                //We need to make sure this isnt the begining of the curve
                //Then we need to check if we're at the end of the curve, (the value will be 0)
                //Because this is happening on the update tick and we only want the logic in this check to run once
                //Set the moveSpeed to a "sentinel" value, 1 for now
                if (time != 0 && currentCharacter.MoveSpeed == currentCurve.keys[currentCurve.keys.Length - 1].value)
                {
                    //currentCharacter.MoveSpeed = currentCharacter.BaseMoveSpeed;
                    currentCharacter.MoveSpeed = 1;
                    currentlyEvaluating = false;
                    time = 0.0f;
                }
            }
        }


        //How long should it take to translate across the grid?
    }
}
