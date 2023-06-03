using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ForverFight.HelperScripts
{
    public class EventOnEnable : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent onEnableEvent = new UnityEvent();
        [SerializeField]
        private bool isDoOnce = false;


        private bool doOnce = false;


        protected void OnEnable()
        {
            if (isDoOnce)
            {
                if (!doOnce)
                {
                    onEnableEvent?.Invoke();
                    doOnce = true;
                }
            }
            else
            {
                onEnableEvent?.Invoke();
            }
        }
    }
}
