using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForverFight.Ui
{
    public class StatusEffectDisplayFormatter : MonoBehaviour
    {
        public enum StatusEffectType
        {
            None = 0,
            Momentum = 1,
        }


        [SerializeField]
        private StatusEffectType currentStatusEffectType = StatusEffectType.None;
        [SerializeField]
        private Vector3 localScale = new Vector3(0, 0, 0);
        [SerializeField]
        private Vector3 localEulerAngles = new Vector3(0, 0, 0);


        public StatusEffectType CurrentStatusEffectType { get => currentStatusEffectType; set => currentStatusEffectType = value; }

        public Vector3 LocalScale { get => localScale; set => localScale = value; }

        public Vector3 LocalEulerAngles { get => localEulerAngles; set => localEulerAngles = value; }
    }
}
