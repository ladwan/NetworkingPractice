using ForeverFight.Interactable.Characters;
using ForeverFight.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveMoveSpeed : MonoBehaviour
{
    [SerializeField]
    private AnimationCurve curve = null;
    [SerializeField]
    private float moveSpeed = 0.0f;
    [SerializeField]
    private float curveEvaluationSpeed = 1.0f;


    private bool evaluate = false;
    private float time = 0.0f;
    private Character localCharacter = null;
    private int curveIndex = -1;
    private AnimationCurve currentCurve = null;


    public void TestDynamicMoveSpeed()
    {
        curveIndex = LocalStoredNetworkData.squaresMovedThisInstanceOfMovement - 1;

        if (localCharacter == null)
        {
            localCharacter = LocalStoredNetworkData.GetLocalCharacter();
        }

        currentCurve = localCharacter.MovementCurves[curveIndex];

        evaluate = true;
    }


    private void Update()
    {
        if (evaluate)
        {
            time += Time.deltaTime * curveEvaluationSpeed;
            LocalStoredNetworkData.GetLocalCharacter().MoveSpeed = currentCurve.Evaluate(time);
            moveSpeed = LocalStoredNetworkData.GetLocalCharacter().MoveSpeed;
            localCharacter.CharacterAnimationReferences.CharacterAnimator.SetFloat("CharSpeed", moveSpeed);

            if (time != 0 && LocalStoredNetworkData.GetLocalCharacter().MoveSpeed == currentCurve.keys[currentCurve.keys.Length - 1].value)
            {
                LocalStoredNetworkData.GetLocalCharacter().MoveSpeed = 1;
                time = 0.0f;
                evaluate = false;
            }
        }
    }


    //How long should it take to translate across the grid?
}
