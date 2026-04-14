using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ForeverFight.HelperScripts
{
    public class EventOnDisable : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onDisableEvent = new UnityEvent();
        [SerializeField]
        private bool isDoOnce = false;


        private bool doOnce = false;


        protected void OnDisable()
        {
            if (isDoOnce)
            {
                if (!doOnce)
                {
                    onDisableEvent?.Invoke();
                    doOnce = true;
                }
            }
            else
            {
                onDisableEvent?.Invoke();
            }
        }
    }
}
