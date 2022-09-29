using ForverFight.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ForverFight.HelperScripts
{
    public class CleanUpUiAfterTurn : MonoBehaviour
    {
        [Header("Player 1")]
        [SerializeField]
        private List<GameObject> player1ObjectsToTurnEnable = new List<GameObject>();
        [SerializeField]
        private List<GameObject> player1ObjectsToTurnDisable = new List<GameObject>();
        [SerializeField]
        private UnityEvent player1TurnEndEvent = new UnityEvent();

        [Header("Player 2")]
        [SerializeField]
        private List<GameObject> player2ObjectsToTurnEnable = new List<GameObject>();
        [SerializeField]
        private List<GameObject> player2ObjectsToTurnDisable = new List<GameObject>();
        [SerializeField]
        private UnityEvent player2TurnEndEvent = new UnityEvent();


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
                for (int i = 0; i < player1ObjectsToTurnEnable.Count; i++)
                {
                    player1ObjectsToTurnEnable[i].SetActive(true);
                }

                for (int i = 0; i < player1ObjectsToTurnDisable.Count; i++)
                {
                    player1ObjectsToTurnDisable[i].SetActive(false);
                }

                player1TurnEndEvent?.Invoke();
            }
            else if (ClientInfo.playerNumber == 2)
            {
                for (int i = 0; i < player2ObjectsToTurnEnable.Count; i++)
                {
                    player2ObjectsToTurnEnable[i].SetActive(true);
                }

                for (int i = 0; i < player2ObjectsToTurnDisable.Count; i++)
                {
                    player2ObjectsToTurnDisable[i].SetActive(false);
                }

                player2TurnEndEvent?.Invoke();
            }
        }

        public void CleanUpAbilityRadius(bool value)
        {
            if (LocalStoredNetworkData.localPlayerSelectAbilityToCast)
            {
                LocalStoredNetworkData.localPlayerSelectAbilityToCast.ToggleAbilityRadius(value);
            }
        }
    }
}
