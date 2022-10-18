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
                    DistributedDieValue.SetDieRollValue(1);
                    DistributedDieValue.SetUnchangingDieRollValue(1);
                    apManager.UpdateAP(1);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll1");
                    break;

                case 2:
                    DistributedDieValue.SetDieRollValue(2);
                    DistributedDieValue.SetUnchangingDieRollValue(2);
                    apManager.UpdateAP(2);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll2");
                    break;

                case 3:
                    DistributedDieValue.SetDieRollValue(3);
                    DistributedDieValue.SetUnchangingDieRollValue(3);
                    apManager.UpdateAP(3);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll3");
                    break;

                case 4:
                    DistributedDieValue.SetDieRollValue(4);
                    DistributedDieValue.SetUnchangingDieRollValue(4);
                    apManager.UpdateAP(4);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll4");
                    break;

                case 5:
                    DistributedDieValue.SetDieRollValue(5);
                    DistributedDieValue.SetUnchangingDieRollValue(5);
                    apManager.UpdateAP(5);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll5");
                    break;

                case 6:
                    DistributedDieValue.SetDieRollValue(6);
                    DistributedDieValue.SetUnchangingDieRollValue(6);
                    apManager.UpdateAP(6);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll6");
                    break;
            }
        }
        IEnumerator UiDelay()
        {
            yield return new WaitForSeconds(8);
            uiToToggle.ToggleObjects();
        }
    }
}