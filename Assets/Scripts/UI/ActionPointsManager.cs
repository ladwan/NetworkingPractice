using ForverFight.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForverFight.Ui
{
    public class ActionPointsManager : MonoBehaviour
    {
        [SerializeField]
        private static ActionPointsManager instance = null;
        [SerializeField]
        private List<ApLight> apLights = new List<ApLight>();
        [SerializeField]
        private int currentAp = 0;
        [SerializeField]
        private List<ApLight> apLightsToBeBlinked = new List<ApLight>();
        [SerializeField]
        private bool playerTurnHasEnded = false;


        [NonSerialized]
        private int maxAP = 9;
        [NonSerialized]
        private bool blinkCoroutineIsRunning = false;


        public static ActionPointsManager Instance { get => instance; set => instance = value; }

        public List<ApLight> ApLights { get => apLights; set => apLights = value; }

        public int CurrentAp { get => currentAp; set => currentAp = value; }

        public List<ApLight> ApLightsToBeBlinked { get => apLightsToBeBlinked; set => apLightsToBeBlinked = value; }

        public bool PlayerTurnHasEnded { get => playerTurnHasEnded; set => playerTurnHasEnded = value; }


        protected void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.Log("More Than 1 AP Manager detected, Destroying self...");
                Destroy(instance);
                return;
            }
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

            if (playerTurnHasEnded)
            {
                if (apLightsToBeBlinked.Count > 0)
                {
                    apLightsToBeBlinked.Clear();
                    StopBlink();
                    playerTurnHasEnded = false;
                }
            }
        }

        private void EmptyAllAP()
        {
            for (int i = 0; i < apLights.Count; i++)
            {
                apLights[i].gameObject.SetActive(false);
            }
        }

        public void UpdateBlinkingAP()
        {
            if (apLightsToBeBlinked.Count > 0)
            {
                if (playerTurnHasEnded)
                {
                    apLightsToBeBlinked.Clear();
                    playerTurnHasEnded = false;
                }
                else
                {
                    apLightsToBeBlinked.RemoveAt(apLightsToBeBlinked.Count - 1);
                }
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
            apLightsToBeBlinked.Add(apLights[LocalStoredNetworkData.localPlayerCurrentAP - 1]);
            LocalStoredNetworkData.localPlayerCurrentAP--;
            if (!blinkCoroutineIsRunning)
            {
                StartCoroutine(Blink());
            }
        }

        private IEnumerator Blink()
        {
            blinkCoroutineIsRunning = true;
            yield return new WaitForSecondsRealtime(0.5f);

            for (int i = 0; i < apLightsToBeBlinked.Count; i++)
            {
                apLightsToBeBlinked[i].gameObject.SetActive(false);
            }

            yield return new WaitForSecondsRealtime(0.5f);

            for (int i = 0; i < apLightsToBeBlinked.Count; i++)
            {
                apLightsToBeBlinked[i].gameObject.SetActive(true);
            }

            StartCoroutine(Blink());
        }

        public void StopBlink()
        {
            StopAllCoroutines();
            blinkCoroutineIsRunning = false;
        }
    }
}