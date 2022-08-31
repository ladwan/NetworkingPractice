using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ForverFight.GameMechanics
{
    public class DamageManager : MonoBehaviour
    {

        [SerializeField]
        private Slider localPlayerHealthBar = null;
        [SerializeField]
        private Slider otherPlayerHealthBar = null;


        [NonSerialized]
        private static DamageManager instance = null;


        public static DamageManager Instance { get => instance; set => instance = value; }


        protected void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("Instance of DMG MANAGER already exsists, destroying!");
                Destroy(this);
            }
        }

        public void DealDamage(int dmg)
        {
            otherPlayerHealthBar.value -= dmg;
        }
    }
}
