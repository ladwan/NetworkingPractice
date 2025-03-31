using System.Collections;
using UnityEngine;
using ForeverFight.Ui;
using ForeverFight.Networking;
using System.Threading.Tasks;
using ForeverFight.Interactable.Characters;
using UnityEngine.Events;

namespace ForeverFight.GameMechanics.DiceRoll
{
    public class RollDice : MonoBehaviour
    {
        [SerializeField]
        private Animator sixSidedDieAnimator = new Animator();
        [SerializeField]
        private ToggleObjectsOnDieRoll uiToToggle = null;
        [SerializeField]
        private ActionPointsManager apManager = null;
        [SerializeField]
        private BoxCollider dieInteractityCollider = null;
        [SerializeField]
        private UnityEvent onDieClicked = null; //This assumes that you will only use this event for methods local to this class
        [SerializeField]
        private bool isDebug = false;


        private Animator localCharacterAnimator = null;
        public static int numberOfTimesCalled = 0;


        public Animator SixSidedDieAnimator => sixSidedDieAnimator;
        public BoxCollider DieInteractityCollider { get => dieInteractityCollider; set => dieInteractityCollider = value; }


        private void OnMouseDown()
        {
            //This will be toggled off to prevent double clicking
            //This will be turned back on by an AnimationEvent on the Spawn anim of the 6 sided die
            dieInteractityCollider.enabled = false;
            onDieClicked?.Invoke();
        }


        private void Start()
        {
            StartCoroutine(LocalStoredNetworkData.WaitForCharacterAnimationReferences(SetCharacterAnimatorReferences));
        }


        public void RollSixSidedDie()
        {
            TriggerSixSidedDieAnim(RandomRoll());
            StartCoroutine(DieRollAnimDelay());
        }

        public void SetDebug()
        {
            isDebug = true;
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
            if (isDebug)
            {
                apManager.UpdateAP(apManager.MainApLists, 9);
                sixSidedDieAnimator.SetTrigger("6sidedRoll6");
                DistributedDieValue.distributedDieRollValue = 9;
                return;
            }

            switch (rollValue)
            {
                case 1:
                    apManager.UpdateAP(apManager.MainApLists, 1);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll1");
                    DistributedDieValue.distributedDieRollValue = 1;
                    break;

                case 2:
                    apManager.UpdateAP(apManager.MainApLists, 2);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll2");
                    DistributedDieValue.distributedDieRollValue = 2;
                    break;

                case 3:
                    apManager.UpdateAP(apManager.MainApLists, 3);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll3");
                    DistributedDieValue.distributedDieRollValue = 3;
                    break;

                case 4:
                    apManager.UpdateAP(apManager.MainApLists, 4);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll4");
                    DistributedDieValue.distributedDieRollValue = 4;
                    break;

                case 5:
                    apManager.UpdateAP(apManager.MainApLists, 5);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll5");
                    DistributedDieValue.distributedDieRollValue = 5;
                    break;

                case 6:
                    apManager.UpdateAP(apManager.MainApLists, 6);
                    sixSidedDieAnimator.SetTrigger("6sidedRoll6");
                    DistributedDieValue.distributedDieRollValue = 6;
                    break;
            }
        }

        private IEnumerator UiDelay()
        {
            yield return new WaitUntil(() => localCharacterAnimator.GetCurrentAnimatorStateInfo(1).IsName("Camera - Birds Eye"));
            yield return new WaitForSecondsRealtime(localCharacterAnimator.GetCurrentAnimatorClipInfo(1)[0].clip.length);

            uiToToggle.ToggleObjects();
        }

        private IEnumerator DieRollAnimDelay()
        {
            yield return new WaitForSecondsRealtime(2);

            if (localCharacterAnimator is not null)
            {
                localCharacterAnimator.SetTrigger("Camera - Go to Birds Eye");
            }
            else
            {
                Debug.LogWarning("localCharacterAnimator is not set, yet you are trying to access it !");
            }
        }

        private void SetCharacterAnimatorReferences(CharacterAnimationReferences animationReferences)
        {
            localCharacterAnimator = animationReferences.CharacterAnimator;
        }
    }
}