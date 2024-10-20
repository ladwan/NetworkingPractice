using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Networking;
using ForeverFight.Interactable.Abilities;

namespace ForeverFight.FlowControl
{
    public class ToggleTimerAndUi : MonoBehaviour
    {
        [SerializeField]
        private GameObject uiToToggle = null;


        private static ToggleTimerAndUi instance = null;
        private Coroutine sub = null;


        public static ToggleTimerAndUi Instance { get => instance; set => instance = value; }


        protected void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.Log("More Than 1 Floor grid detected, Destroying self...");
                Destroy(instance);
            }
        }


        public void ToggleInteractivityWhileAnimating()
        {
            ToggleInteractableUiAndTimer();
        }

        public void ToggleInteractivityWhileAnimating(Animator animatorREF, string triggerToFire, CharAbility.CameraShakeParameters parameters)
        {
            if (sub != null)
            {
                return;
            }

            ToggleInteractableUiAndTimer();
            animatorREF.SetTrigger(triggerToFire);
            sub = StartCoroutine(ListenForAnimEnd(animatorREF, triggerToFire, parameters)); //the trigger and the state should always have the same name, so this should work.
        }

        public void FireAnimationWithoutToggleOffInteractivity(Animator animatorREF, string triggerToFire, CharAbility.CameraShakeParameters parameters)
        {
            if (sub != null)
            {
                return;
            }

            animatorREF.SetTrigger(triggerToFire);
            sub = StartCoroutine(ListenForAnimEnd(animatorREF, triggerToFire, parameters)); //the trigger and the state should always have the same name, so this should work.
        }


        private void ToggleInteractableUiAndTimer()
        {
            LocalStoredNetworkData.GetCountdownTimerScript().TellNetworkToToggleTimer();
            uiToToggle.SetActive(!uiToToggle.activeInHierarchy);
        }


        private IEnumerator ListenForAnimEnd(Animator animatorREF, string desiredStateName, CharAbility.CameraShakeParameters parameters)
        {
            ClientSend.ClientSendAnimationTrigger(desiredStateName, parameters.duration, parameters.magnitude);

            yield return new WaitUntil(() => animatorREF.GetCurrentAnimatorStateInfo(0).IsName(desiredStateName));
            var animStateInfo = animatorREF.GetCurrentAnimatorStateInfo(0);
            yield return new WaitUntil(() => animatorREF.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95);

            ToggleInteractableUiAndTimer();
            sub = null;
        }
    }
}
