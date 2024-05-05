using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.Ui
{
    public class StatusEffectDisplayFormatter : MonoBehaviour
    {
        [SerializeField]
        private Vector3 localScale = new Vector3(0, 0, 0);
        [SerializeField]
        private Vector3 localEulerAngles = new Vector3(0, 0, 0);


        public Vector3 LocalScale { get => localScale; set => localScale = value; }

        public Vector3 LocalEulerAngles { get => localEulerAngles; set => localEulerAngles = value; }
    }
}
