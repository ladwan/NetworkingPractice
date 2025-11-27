using System;
using System.Collections;
using UnityEngine;

namespace ForeverFight.HelperScripts
{
    public class ExecuteMethodAfterDelay : MonoBehaviour
    {
        private static ExecuteMethodAfterDelay instance = null;
        private bool waitUntilTrue = false;
        private Coroutine sub = null;


        public static ExecuteMethodAfterDelay Instance { get => instance; set => instance = value; }
        public bool WaitUntilTrue { get => waitUntilTrue; set => waitUntilTrue = value; }


        protected void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.Log("More Than 1 ExecuteMethodAfterDelay detected, Destroying self...");
                Destroy(instance);
            }
        }


        public void BeginDelay(float delayTime, Action callback)
        {
            StartCoroutine(Delay(delayTime, callback));
        }

        private IEnumerator Delay(float delay, Action callback)
        {
            yield return new WaitForSecondsRealtime(delay);
            callback?.Invoke();
        }


        public void BeginWaitUntilTrue(Action callback)
        {
            if(sub != null) { return; }

            waitUntilTrue = false;
            sub = StartCoroutine(WaitUntilTrueIEnum(callback));
        }

        private IEnumerator WaitUntilTrueIEnum(Action callback)
        {
            Debug.Log($"~~~~~ {waitUntilTrue} : 02");
            yield return new WaitUntil(()=> waitUntilTrue == true);
            callback?.Invoke();
            waitUntilTrue = false;
            sub = null;
        }
    }
}
