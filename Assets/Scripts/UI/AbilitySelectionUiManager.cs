using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using ForverFight.Networking;
using ForverFight.Interactable;


public class AbilitySelectionUiManager : MonoBehaviour
{
    [SerializeField]
    private List<Button> abilityButtons = new List<Button>();
    [SerializeField]
    private List<TMP_Text> abilityTexts = new List<TMP_Text>();
    [SerializeField]
    private TMP_Text abilityDescriptionText = null;


    private Character charReference = null;
    private GameObject currentAbilityRadius = null;
    private Button currentlyClickedButton = null;
    private string infoPlaceholderText = "Please selcet an ability to display its info!";


    public void PopulateAbilityData()
    {
        charReference = LocalStoredNetworkData.GetLocalCharacter();
        if (charReference)
        {
            for (int i = 0; i < charReference.Moveset.Count && i < abilityTexts.Count; i++)
            {
                abilityTexts[i].text = charReference.Moveset[i].AbilityName;
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

        if (!currentAbilityRadius)
        {
            currentAbilityRadius = charReference.Moveset[value].AbilityRadius;
            currentAbilityRadius.SetActive(true);
        }
        else
        {
            currentAbilityRadius.SetActive(false);
            currentAbilityRadius = charReference.Moveset[value].AbilityRadius;
            currentAbilityRadius.SetActive(true);
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
            currentAbilityRadius.SetActive(false);
        }
    }

    //if an abilty is selected, turn on ability radius and confirm button
}
