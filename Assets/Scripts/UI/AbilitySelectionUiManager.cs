using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ForverFight.Networking;
using ForverFight.Interactable;


namespace ForverFight.Ui
{
    public class AbilitySelectionUiManager : MonoBehaviour
    {
        [SerializeField]
        private List<Button> abilityButtons = new List<Button>();
        [SerializeField]
        private List<TMP_Text> abilityTexts = new List<TMP_Text>();
        [SerializeField]
        private List<TMP_Text> abilityCostTmps = new List<TMP_Text>();
        [SerializeField]
        List<GameObject> abilityBlockers = new List<GameObject>();
        [SerializeField]
        List<GameObject> apCostDisplays = new List<GameObject>();
        [SerializeField]
        List<GameObject> characterSpecificUiDisplays = new List<GameObject>();
        [SerializeField]
        private TMP_Text abilityDescriptionText = null;


        private Action<int> onSpawnButtonUi = null;
        private Action onReadyToBeFormatted = null;
        private static AbilitySelectionUiManager instance = null;


        public Action<int> OnSpawnButtonUi { get => onSpawnButtonUi; set => onSpawnButtonUi = value; }
        public Action OnReadyToBeFormatted { get => onReadyToBeFormatted; set => onReadyToBeFormatted = value; }
        public List<Button> AbilityButtons => abilityButtons;

        public List<TMP_Text> AbilityTexts => abilityTexts;

        public List<TMP_Text> AbilityCostTmps => abilityCostTmps;

        public List<GameObject> AbilityBlockers => abilityBlockers;

        public List<GameObject> ApCostDisplays => apCostDisplays;

        public List<GameObject> CharacterSpecificUiDisplays => characterSpecificUiDisplays;

        public static AbilitySelectionUiManager Instance => instance;


        private Coroutine delaySub = null;
        private Character charReference = null;
        private GameObject currentAbilityRadius = null;
        private Button currentlyClickedButton = null;
        private int abilityValue = 99; // 99 is an arbitary value, we set it to this by default so we know we will get an error if we dont properly set this value before using it
        private string infoPlaceholderText = "Please select an ability to display its info!";


        protected AbilitySelectionUiManager()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.Log("More Than 1 AbilitySelectionUiManager detected, Destroying self...");
                Destroy(instance);
            }
        }


        public void PopulateAbilityData()
        {
            charReference = LocalStoredNetworkData.GetLocalCharacter();
            if (charReference)
            {
                for (int i = 0; i < charReference.Moveset.Count && i < abilityTexts.Count; i++)
                {
                    abilityTexts[i].text = charReference.Moveset[i].AbilityName;
                    abilityCostTmps[i].text = charReference.Moveset[i].AbilityCost.ToString();
                    abilityButtons[i].interactable = true;
                }
            }
        }

        public void UpdateAbilityUi(Button clickedButton)
        {
            foreach (Button button in abilityButtons)
            {
                if (button != clickedButton)
                {
                    button.interactable = true;
                }
            }
            clickedButton.interactable = false;
            currentlyClickedButton = clickedButton;
        }

        public void UpdateAbilityDescriptionInfo(int value)
        {
            abilityDescriptionText.text = charReference.Moveset[value].AbilityDescription;
            abilityValue = value;

            //When you click an ability, it triggers the AP to blink, once blinking in the background the AP value is decremented. So we reset that unused AP each time we click an ability to give player back ap they didnt spend
            ResetApUsageUiManager();

            if (!currentAbilityRadius)
            {
                currentAbilityRadius = charReference.Moveset[value].AbilityRadius;
                ToggleAbilityRadius(true, Delay());
            }
            else
            {
                ToggleAbilityRadius(false);
                currentAbilityRadius = charReference.Moveset[value].AbilityRadius;
                ToggleAbilityRadius(true, Delay());
            }
        }

        public void ResetCombatUi()
        {
            abilityDescriptionText.text = infoPlaceholderText;

            if (currentlyClickedButton)
            {
                if (currentlyClickedButton.interactable == false)
                {
                    currentlyClickedButton.interactable = true;
                }
            }

            if (currentAbilityRadius)
            {
                ToggleAbilityRadius(false);
            }

            ResetApUsageUiManager();
            DisableConfirmButtonInteractable();
        }

        public void CastSelectedAbility()
        {
            if (abilityValue < 3 && abilityValue >= 0)
            {
                charReference.Moveset[abilityValue].CastAbility();
                ActionPointsManager.Instance.ResetApUsage(ActionPointsManager.Instance.MainApLists);
                ActionPointsManager.Instance.UpdateAP(ActionPointsManager.Instance.MainApLists, -charReference.Moveset[abilityValue].AbilityCost);
            }
            else
            {
                Debug.Log($"Abnormal ability value !  {abilityValue}");
            }
        }

        public void ResetApUsageUiManager()
        {
            ActionPointsManager.Instance.ResetApUsage(ActionPointsManager.Instance.MainApLists);
        }

        public void ToggleAbilityDisplay(int index, bool toggle)
        {
            apCostDisplays[index].SetActive(toggle);
            abilityBlockers[index].SetActive(!toggle);
            characterSpecificUiDisplays[index].SetActive(!toggle);


            if (!toggle)
            {
                if (GetTransformOfCharacterSpecificUiAtIndex(index).childCount == 0)
                {
                    onSpawnButtonUi?.Invoke(index);
                }

                onReadyToBeFormatted?.Invoke();
            }
        }

        public Transform GetTransformOfCharacterSpecificUiAtIndex(int index) => characterSpecificUiDisplays[index].transform;


        private void ToggleAbilityRadius(bool toggle)
        {
            currentAbilityRadius.SetActive(toggle);
        }

        private void ToggleAbilityRadius(bool toggle, IEnumerator delayCallback)
        {
            currentAbilityRadius.SetActive(toggle);
            if (delaySub != null)
            {
                StopCoroutine(delaySub);
                delaySub = null;
            }
            delaySub = StartCoroutine(delayCallback);
        }

        private bool EnoughApToCastAbility()
        {
            if (LocalStoredNetworkData.localPlayerCurrentAP >= charReference.Moveset[abilityValue].AbilityCost)
            {
                return true;
            }
            return false;
        }

        private void DisableConfirmButtonInteractable()
        {
            if (LocalStoredNetworkData.localPlayerAttackConfirmButton.interactable)
            {
                LocalStoredNetworkData.localPlayerAttackConfirmButton.interactable = false;
            }
        }

        private IEnumerator Delay()
        {
            yield return new WaitForSecondsRealtime(0.1f);

            if (EnoughApToCastAbility())
            {
                if (LocalStoredNetworkData.damageableObjectDetected)
                {
                    LocalStoredNetworkData.localPlayerAttackConfirmButton.interactable = true;
                    for (int i = 0; i < charReference.Moveset[abilityValue].AbilityCost; i++)
                    {
                        ActionPointsManager.Instance.ApMovementBlink(ActionPointsManager.Instance.MainApLists);
                    }

                    //Debug.Log("Damageable Object Detected , Turning Button On!");
                }
                else
                {
                    DisableConfirmButtonInteractable();
                    //Debug.Log("No Damageable Object Detected !");
                }
            }
            else
            {
                DisableConfirmButtonInteractable();
                //Debug.Log($"Not Enough AP!  {LocalStoredNetworkData.localPlayerCurrentAP}");
            }
        }
    }
}
