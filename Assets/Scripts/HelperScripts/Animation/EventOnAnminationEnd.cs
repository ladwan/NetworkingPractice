using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ForverFight.HelperScripts.Animation
{
    public class EventOnAnminationEnd : MonoBehaviour
    {
        [SerializeField]
        private Animator animatorREF = null;
        [SerializeField]
        private string animatorStateName = "";
        [SerializeField]
        private UnityEvent animationEndEvent = new UnityEvent();


        private float noralizedTime = 0;


        protected void Update()
        {
            var animStateInfo = animatorREF.GetCurrentAnimatorStateInfo(0);

            if (animStateInfo.IsName(animatorStateName))
            {
                noralizedTime = animStateInfo.normalizedTime;
                if (noralizedTime >= 1.0f)
                {
                    animationEndEvent?.Invoke();
                    this.enabled = false;
                }
            }
        }

    }
}
