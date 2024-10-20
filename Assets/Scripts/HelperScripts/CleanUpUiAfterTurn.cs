using ForeverFight.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ForeverFight.HelperScripts
{
    public class CleanUpUiAfterTurn : MonoBehaviour
    {
        [Header("Player 1")]
        [SerializeField]
        private List<GameObject> playerObjectsToTurnEnable = new List<GameObject>();
        [SerializeField]
        private List<GameObject> playerObjectsToTurnDisable = new List<GameObject>();
        [SerializeField]
        private UnityEvent playerTurnEndEvent = new UnityEvent();


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
            /*for (int i = 0; i < playerObjectsToTurnEnable.Count; i++)
            {
                playerObjectsToTurnEnable[i].SetActive(true);
            }*/


            for (int i = 0; i < playerObjectsToTurnDisable.Count; i++)
            {
                playerObjectsToTurnDisable[i].SetActive(false);
            }

            playerTurnEndEvent?.Invoke();
        }

        public void CleanUpAbilityRadius(bool value)
        {
            if (LocalStoredNetworkData.localPlayerSelectAbilityToCast)
            {
                // LocalStoredNetworkData.localPlayerSelectAbilityToCast.ToggleAbilityRadius(value);
            }
        }
    }
}
