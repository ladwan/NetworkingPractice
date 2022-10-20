using ForverFight.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ForverFight.Ui
{
    public class ActionPointsManager : MonoBehaviour
    {
        [SerializeField]
        private List<ApLight> apLights = new List<ApLight>();


        [NonSerialized]
        private int maxAP = 9;


        public List<ApLight> ApLights { get => apLights; set => apLights = value; }


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
                apLights[i].gameObject.SetActive(true);
            }
        }

        private void EmptyAllAP()
        {
            for (int i = 0; i < apLights.Count; i++)
            {
                apLights[i].gameObject.SetActive(false);
            }
        }

        public bool YouHaveEnoughAp(int value)
        {
            if (value < LocalStoredNetworkData.localPlayerCurrentAP)
            {
                return true;
            }
            else
            {
                Debug.Log("Insufficient AP");
                return false;
            }
        }

        public void ApMovementBlink()
        {
            apLights[LocalStoredNetworkData.localPlayerCurrentAP - 1].StartBlink();
            LocalStoredNetworkData.localPlayerCurrentAP--;
        }
    }

    //TODO: Connect die roll to apLights, also add in a way to 'spend' AP's with movement or combat.

    //TODO: Take out distributed die roll when highlighting sq's for movement and connect AP to that system
    // a) Find a way to make the AP used in highlighting blink indicating that they are about to be used.
    // b) Make AP lights stop blinking and update current AP amount after  movement is confirmed
    // c) Stop ending turn after confirmed move.
}