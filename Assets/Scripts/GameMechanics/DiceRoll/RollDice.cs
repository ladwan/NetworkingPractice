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
                DistributedDieValue.SetDieRollValue(1);
                DistributedDieValue.SetUnchangingDieRollValue(1);
                sixSidedDieAnimator.SetTrigger("6sidedRoll1");
                break;

            case 2:
                DistributedDieValue.SetDieRollValue(2);
                DistributedDieValue.SetUnchangingDieRollValue(2);
                sixSidedDieAnimator.SetTrigger("6sidedRoll2");
                break;

            case 3:
                DistributedDieValue.SetDieRollValue(3);
                DistributedDieValue.SetUnchangingDieRollValue(3);
                sixSidedDieAnimator.SetTrigger("6sidedRoll3");
                break;

            case 4:
                DistributedDieValue.SetDieRollValue(4);
                DistributedDieValue.SetUnchangingDieRollValue(4);
                sixSidedDieAnimator.SetTrigger("6sidedRoll4");
                break;

            case 5:
                DistributedDieValue.SetDieRollValue(5);
                DistributedDieValue.SetUnchangingDieRollValue(5);
                sixSidedDieAnimator.SetTrigger("6sidedRoll5");
                break;

            case 6:
                DistributedDieValue.SetDieRollValue(6);
                DistributedDieValue.SetUnchangingDieRollValue(6);
                sixSidedDieAnimator.SetTrigger("6sidedRoll6");
                break;
        }
    }
}

