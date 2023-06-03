using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.Networking;
using FlowControl;
using ForverFight.Interactable;

namespace ForverFight.GameMechanics
{
    public class GameMechanicsManager : MonoBehaviour
    {
        [SerializeField]
        private SpawnOpponentScript spawnOpponentScriptREF = null;


        [NonSerialized]
        private static GameMechanicsManager instance = null;
        [NonSerialized]
        private string opponentsName = "";


        public static GameMechanicsManager Instance { get => instance; set => instance = value; }
        public string OpponentsName { get => opponentsName; set => opponentsName = value; }


        protected void Awake()
        {
            if (instance == null)
            {
                instance = this;
                opponentsName = LocalStoredNetworkData.locallyStoredOpponentsName;
            }
            else if (instance != this)
            {
                Debug.Log("Instance of GAME MANAGER already exsists, destroying object!");
                Destroy(this);
            }
        }

        public void HandleSpawningOpponent()
        {
            if (opponentsName != null && opponentsName != "")
            {
                spawnOpponentScriptREF.SpawnOpponent(StringToCharIdentity.IdentifyOponent(opponentsName));
            }
            else
            {
                Debug.Log("There was no name found for opponent, so they could not be spawned ! ");
            }
        }
    }
}
