using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ForverFight.Ui;
using ForverFight.Networking;
using ForverFight.Interactable;

public class AbilitySelectionUiManager : MonoBehaviour
{
    [SerializeField]
    private List<Button> abilityButtons = new List<Button>();
    [SerializeField]
    private List<TMP_Text> abilityTexts = new List<TMP_Text>();
    [SerializeField]
    private List<TMP_Text> abilityCostTmps = new List<TMP_Text>();
    [SerializeField]
    private TMP_Text abilityDescriptionText = null;


    private Coroutine delaySub = null;
    private Character charReference = null;
    private GameObject currentAbilityRadius = null;
    private Button currentlyClickedButton = null;
    private int abilityValue = 99; // 99 is an arbitary value, we set it to this by default so we know we will get an error if we dont properly set this value before using it
    private string infoPlaceholderText = "Please selcet an ability to display its info!";


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

    public void UpdateAbilityDescriptionIfno(int value)
    {
        abilityDescriptionText.text = charReference.Moveset[value].AbilityDescription;
        abilityValue = value;

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
            ActionPointsManager.Instance.ResetApUsage();
            ActionPointsManager.Instance.UpdateAP(-charReference.Moveset[abilityValue].AbilityCost);
        }
        else
        {
            Debug.Log($"Abnormal ability value !  {abilityValue}");
        }
    }

    public void ResetApUsageUiManager()
    {
        ActionPointsManager.Instance.ResetApUsage();
    }


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
                    ActionPointsManager.Instance.ApMovementBlink();
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
