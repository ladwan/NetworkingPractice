using System;
using System.Collections;
using UnityEngine;

namespace ForeverFight.HelperScripts
{
    public class ExecuteMethodAfterDelay : MonoBehaviour
    {
        private static ExecuteMethodAfterDelay instance = null;


        public static ExecuteMethodAfterDelay Instance { get => instance; set => instance = value; }


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

    }
}
