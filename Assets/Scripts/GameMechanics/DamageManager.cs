using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ForverFight.Networking;

namespace ForverFight.GameMechanics
{
    public class DamageManager : MonoBehaviour
    {

        [SerializeField]
        private Slider localPlayerHealthBar = null;
        [SerializeField]
        private Slider otherPlayerHealthBar = null;
        [SerializeField]
        private HealthUpdateNumbersManager healthUpdateNumbersManagerREF = null;


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
            ClientSend.RequestToDamageOpponentsHealth(dmg);
            healthUpdateNumbersManagerREF.Animator.SetTrigger("OpponentHealthEvent");
            healthUpdateNumbersManagerREF.HealthDecreased(healthUpdateNumbersManagerREF.OpponentHealthUpdateNumber, dmg);
        }

        public void ReceiveDamage(int dmg)
        {
            var health = LocalStoredNetworkData.GetLocalHealthSlider();
            if (health)
            {
                health.value -= dmg;
            }
            healthUpdateNumbersManagerREF.Animator.SetTrigger("LocalPlayerHealthEvent");
            healthUpdateNumbersManagerREF.HealthDecreased(healthUpdateNumbersManagerREF.LocalPlayerHealthUpdateNumber, dmg);
        }

        // ToDO : Add healing logic
    }
}
