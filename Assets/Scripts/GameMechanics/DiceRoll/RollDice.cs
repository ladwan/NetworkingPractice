using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class RollDice : MonoBehaviour
{
    [SerializeField]
    private Animator sixSidedDieAnimator = new Animator();


    public void RollSixSidedDie()
    {
        TriggerSixSidedDieAnim(RandomRoll());
    }


    private int RandomRoll()
    {
        var value = Random.Range(1, 7);
        return value;
    }

    private void TriggerSixSidedDieAnim(int rollValue)
    {
        switch (rollValue)
        {
            case 1:
                StaticDieValue.SetDieRollValue(1);
                sixSidedDieAnimator.SetTrigger("6sidedRoll1");
                break;

            case 2:
                StaticDieValue.SetDieRollValue(2);
                sixSidedDieAnimator.SetTrigger("6sidedRoll2");
                break;

            case 3:
                StaticDieValue.SetDieRollValue(3);
                sixSidedDieAnimator.SetTrigger("6sidedRoll3");
                break;

            case 4:
                StaticDieValue.SetDieRollValue(4);
                sixSidedDieAnimator.SetTrigger("6sidedRoll4");
                break;

            case 5:
                StaticDieValue.SetDieRollValue(5);
                sixSidedDieAnimator.SetTrigger("6sidedRoll5");
                break;

            case 6:
                StaticDieValue.SetDieRollValue(6);
                sixSidedDieAnimator.SetTrigger("6sidedRoll6");
                break;
        }
    }
}

