using System.Collections;
using UnityEngine;
using ForverFight.Ui;

namespace ForverFight.GameMechanics.DiceRoll
{
    public class RollDice : MonoBehaviour
    {
        [SerializeField]
        private Animator sixSidedDieAnimator = new Animator();
        [SerializeField]
        private ToggleObjectsOnDieRoll uiToToggle = null;
        [SerializeField]
        private ActionPointsManager apManager = null;


        public void RollSixSidedDie()
        {
            TriggerSixSidedDieAnim(RandomRoll());
        }


        private int RandomRoll()
        {
            var value = Random.Range(1, 7);
            return value;
        }

        public void CallUiDelay()
        {
            StartCoroutine(UiDelay());
        }

        private void TriggerSixSidedDieAnim(int rollValue)
        {
            switch (rollValue)
            {
                case 1:
                    apManager.UpdateAP(1);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll1");
                    DistributedDieValue.distributedDieRollValue = 1;
                    break;

                case 2:
                    apManager.UpdateAP(2);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll2");
                    DistributedDieValue.distributedDieRollValue = 2;
                    break;

                case 3:
                    apManager.UpdateAP(3);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll3");
                    DistributedDieValue.distributedDieRollValue = 3;
                    break;

                case 4:
                    apManager.UpdateAP(4);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll4");
                    DistributedDieValue.distributedDieRollValue = 4;
                    break;

                case 5:
                    apManager.UpdateAP(5);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll5");
                    DistributedDieValue.distributedDieRollValue = 5;
                    break;

                case 6:
                    apManager.UpdateAP(6);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll6");
                    DistributedDieValue.distributedDieRollValue = 6;
                    break;
            }
        }

        private IEnumerator UiDelay()
        {
            yield return new WaitUntil(() => !sixSidedDieAnimator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !sixSidedDieAnimator.GetCurrentAnimatorStateInfo(0).IsName("Spawn") && sixSidedDieAnimator.IsInTransition(0) == false);
            yield return new WaitForSecondsRealtime(sixSidedDieAnimator.GetCurrentAnimatorClipInfo(0)[0].clip.length);
            uiToToggle.ToggleObjects();
        }
    }
}