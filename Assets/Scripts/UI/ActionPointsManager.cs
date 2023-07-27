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
        private ApReferenceLists mainApLists = null;
        [SerializeField]
        private ApReferenceLists speedsterPassiveApLists = null;
        [SerializeField]
        private int currentAp = 0;
        [SerializeField]
        private bool playerTurnHasEnded = false;


        [NonSerialized]
        private int maxAP = 9;
        [NonSerialized]
        private ApReferenceLists currentApReferenceListsREF = null;


        public static ActionPointsManager Instance { get => instance; set => instance = value; }

        public int CurrentAp { get => currentAp; set => currentAp = value; }

        public bool PlayerTurnHasEnded { get => playerTurnHasEnded; set => playerTurnHasEnded = value; }

        public int MaxAP { get => maxAP; set => maxAP = value; }

        public ApReferenceLists MainApLists { get => mainApLists; set => mainApLists = value; }

        public ApReferenceLists CurrentApReferenceListsREF { get => currentApReferenceListsREF; set => currentApReferenceListsREF = value; }

        public ApReferenceLists SpeedsterPassiveApLists { get => speedsterPassiveApLists; set => speedsterPassiveApLists = value; }


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

            UpdateAP(mainApLists, 0);
        }


        public void UpdateAP(ApReferenceLists referenceLists, int addend)
        {
            SetCurrentlyActiveReferenceList(referenceLists);
            referenceLists.HideAllApLights();
            DetermineIfUpdatedApShouldBeShown(referenceLists, addend);

            if (playerTurnHasEnded)
            {
                playerTurnHasEnded = false;

                if (referenceLists.ApLightsToBeBlinked.Count > 0)
                {
                    referenceLists.StopBlink();
                    referenceLists.ApLightsToBeBlinked.Clear();
                }
            }
        }

        public void UpdateBlinkingAP(ApReferenceLists referenceLists)
        {
            if (referenceLists.ApLightsToBeBlinked.Count > 0)
            {
                if (playerTurnHasEnded)
                {
                    referenceLists.StopBlink();
                    referenceLists.ApLightsToBeBlinked.Clear();
                    playerTurnHasEnded = false;
                }
                else
                {
                    referenceLists.ApLightsToBeBlinked.RemoveAt(referenceLists.ApLightsToBeBlinked.Count - 1);
                    if (referenceLists.ApLightsToBeBlinked.Count <= 0)
                    {
                        referenceLists.StopBlink();
                    };
                }
            }
        }

        public void MoveWasConfirmed(ApReferenceLists referenceLists)
        {
            referenceLists.StopBlink();
            referenceLists.ApLightsToBeBlinked.Clear();
            UpdateAP(referenceLists, 0);
        }

        public void MoveWasCanceled()
        {
            if (currentApReferenceListsREF)
            {
                ResetApUsage(currentApReferenceListsREF);
            }

            FloorGrid.Instance.EmptyGridPointList();
        }

        public bool YouHaveEnoughAp(int value)
        {
            if (value < LocalStoredNetworkData.localPlayerCurrentAP)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ApMovementBlink(ApReferenceLists referenceLists)
        {
            referenceLists.ApLightsToBeBlinked.Add(referenceLists.ApLights[referenceLists.UpdateValueOfRelevantAp(-1)]);
            if (!referenceLists.BlinkCoroutineIsRunning)
            {
                referenceLists.StartCoroutine(referenceLists.Blink());
            }
        }

        public void ResetApUsage(ApReferenceLists referenceLists)
        {
            if (referenceLists.ApLightsToBeBlinked.Count > 0)
            {
                referenceLists.StopBlink();
                UpdateAP(referenceLists, referenceLists.ApLightsToBeBlinked.Count);
                referenceLists.ApLightsToBeBlinked.Clear();
            }
            else
            {
                UpdateAP(referenceLists, 0);
            }
        }

        public void BlinkCurrentListReference()
        {
            switch (CurrentApReferenceListsREF.CurrentApDisplayType)
            {
                default:
                    Debug.Log($"CurrentApDisplayType returned an abnormal value : {CurrentApReferenceListsREF.CurrentApDisplayType}");
                    break;

                case ApReferenceLists.apDisplayTypes.main:
                    ApMovementBlink(MainApLists);
                    break;

                case ApReferenceLists.apDisplayTypes.speedster:
                    if (SpeedsterPassiveApLists)
                    {
                        ApMovementBlink(SpeedsterPassiveApLists);
                    }
                    break;
            }
        }


        private void DetermineIfUpdatedApShouldBeShown(ApReferenceLists referenceLists, int addend)
        {
            if (referenceLists.currentApDisplayType != ApReferenceLists.apDisplayTypes.speedster)
            {
                referenceLists.ShowAp(referenceLists.UpdateValueOfRelevantAp(addend)); //First UpdateValueOfRelevantAp() will return the sum of our current Ap and the added // Then ShowAp() will display aplights equal to that sum
            }
            else
            {
                if (CombatUiStatesManager.Instance.CurrentCombatUiState == CombatUiStatesManager.CombatUiState.movement)
                {
                    referenceLists.ShowAp(referenceLists.UpdateValueOfRelevantAp(addend));
                }
                else
                {
                    referenceLists.UpdateValueOfRelevantAp(addend);
                }
            }
        }

        private void SetCurrentlyActiveReferenceList(ApReferenceLists referenceLists)
        {
            if (referenceLists == mainApLists)
            {
                currentApReferenceListsREF = referenceLists;
            }
            if (referenceLists.currentApDisplayType == ApReferenceLists.apDisplayTypes.speedster)
            {
                currentApReferenceListsREF = referenceLists;
            }
        }
    }
}