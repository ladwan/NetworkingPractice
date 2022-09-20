using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ForverFight.Networking;

namespace ForverFight.HelperScripts
{
    public class CleanUpUiAfterTurn : MonoBehaviour
    {
        [Header("Player 1")]
        [SerializeField]
        private GameObject player1BackgroundPanel = null;
        [SerializeField]
        private GameObject player1MovementUi = null;
        [SerializeField]
        private GameObject player1AttackUi = null;
        [SerializeField]
        private Button player1RollDieButton = null;

        [Header("Player 2")]
        [SerializeField]
        private GameObject player2BackgroundPanel = null;
        [SerializeField]
        private GameObject player2MovementUi = null;
        [SerializeField]
        private GameObject player2AttackUi = null;
        [SerializeField]
        private Button player2RollDieButton = null;


        [NonSerialized]
        private static CleanUpUiAfterTurn instance = null;


        public static CleanUpUiAfterTurn Instance { get => instance; set => instance = value; }


        protected void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.Log("Clean up already exsists , Destroying..");
                Destroy(instance);
            }
        }

        public void CleanUpUi()
        {
            if (ClientInfo.playerNumber == 1)
            {
                if (player1BackgroundPanel.activeInHierarchy)
                {
                    player1BackgroundPanel.SetActive(false);
                }
                if (player1MovementUi.activeInHierarchy)
                {
                    player1MovementUi.SetActive(false);
                }
                if (player1AttackUi.activeInHierarchy)
                {
                    player1AttackUi.SetActive(false);
                }
                if (!player1RollDieButton.interactable)
                {
                    player1RollDieButton.interactable = true;
                }
            }
            else if (ClientInfo.playerNumber == 2)
            {
                if (player2BackgroundPanel.activeInHierarchy)
                {
                    player2BackgroundPanel.SetActive(false);
                }
                if (player2MovementUi.activeInHierarchy)
                {
                    player2MovementUi.SetActive(false);
                }
                if (player2AttackUi.activeInHierarchy)
                {
                    player2AttackUi.SetActive(false);
                }
                if (!player2RollDieButton.interactable)
                {
                    player2RollDieButton.interactable = true;
                }
            }
        }

        public void CleanUpAbilityRadius(bool value)
        {
            if (LocalStoredNetworkData.localPlayerSelectAbilityToCast)
            {
                LocalStoredNetworkData.localPlayerSelectAbilityToCast.ToggleAbilityRadius(value);
                LocalStoredNetworkData.localPlayerSelectAbilityToCast = null;
            }
        }
    }
}
