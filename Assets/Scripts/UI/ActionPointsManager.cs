using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.Networking;

namespace ForverFight.Ui
{
    public class ActionPointsManager : MonoBehaviour
    {
        [SerializeField]
        private List<GameObject> apLights = new List<GameObject>();


        [NonSerialized]
        private int maxAP = 9;


        public List<GameObject> ApLights { get => apLights; set => apLights = value; }

        protected void OnEnable()
        {
            UpdateAP(0);
        }

        public void UpdateAP(int value)
        {
            EmptyAllAP();
            LocalStoredNetworkData.localPlayerCurrentAP += value;

            if (LocalStoredNetworkData.localPlayerCurrentAP < 0)
            {
                LocalStoredNetworkData.localPlayerCurrentAP = 0;
            }
            if (LocalStoredNetworkData.localPlayerCurrentAP > maxAP)
            {
                LocalStoredNetworkData.localPlayerCurrentAP = maxAP;
            }
            for (int i = 0; i < LocalStoredNetworkData.localPlayerCurrentAP; i++)
            {
                apLights[i].SetActive(true);
            }
        }

        private void EmptyAllAP()
        {
            for (int i = 0; i < apLights.Count; i++)
            {
                apLights[i].SetActive(false);
            }
        }
    }

    //TODO: Connect die roll to apLights, also add in a way to 'spend' AP's with movement or combat.
}