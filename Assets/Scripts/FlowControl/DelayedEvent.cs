using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ForverFight.FlowControl
{
    public class DelayedEvent : MonoBehaviour
    {
        [SerializeField]
        private float delayTime = 0;
        [SerializeField]
        private UnityEvent eventOnDelay = new();


        private Coroutine coroutineREF;


        public float DelayTime { get => delayTime; set => delayTime = value; }

        public UnityEvent EventOnDelay { get => eventOnDelay; set => eventOnDelay = value; }


        public void BeginDelayCoroutine()
        {
            EndCoroutine();
            coroutineREF = StartCoroutine(Delay());
        }

        private void EndCoroutine()
        {
            if (coroutineREF != null)
            {
                StopCoroutine(coroutineREF);
                coroutineREF = null;
            }
        }

        private IEnumerator Delay()
        {
            yield return new WaitForSecondsRealtime(delayTime);

            eventOnDelay?.Invoke();
        }
    }
}
