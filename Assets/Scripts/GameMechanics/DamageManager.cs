using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ForeverFight.Ui;
using ForeverFight.Networking;
using ForeverFight.FlowControl;

namespace ForeverFight.GameMechanics
{
    public class DamageManager : MonoBehaviour
    {

        [SerializeField]
        private Slider localPlayerHealthBar = null;
        [SerializeField]
        private Slider otherPlayerHealthBar = null;
        [SerializeField]
        private HealthUpdateNumbersManager healthUpdateNumbersManagerREF = null;
        [SerializeField]
        private GameLoopManager gameLoopManagerREF = null;


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

            if (health.value <= 0)
            {
                ClientSend.SendWinnerStatus(true);
                gameLoopManagerREF.ShowLoserScreen();
            }
        }

        // ToDO : Add healing logic
    }
}
