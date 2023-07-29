using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForverFight.Ui
{
    public class StatusEffectDisplayFormatter : MonoBehaviour
    {
        public enum StatusEffectType //Take this and put it in its own static class
        {
            None = 0,
            Momentum = 1,
        }


        [SerializeField]
        private Vector3 localScale = new Vector3(0, 0, 0);
        [SerializeField]
        private Vector3 localEulerAngles = new Vector3(0, 0, 0);
        [SerializeField]
        private StatusEffectType currentStatusEffectType = StatusEffectType.None;


        public Vector3 LocalScale { get => localScale; set => localScale = value; }

        public Vector3 LocalEulerAngles { get => localEulerAngles; set => localEulerAngles = value; }

        public StatusEffectType CurrentStatusEffectType { get => currentStatusEffectType; set => currentStatusEffectType = value; }
    }
}
