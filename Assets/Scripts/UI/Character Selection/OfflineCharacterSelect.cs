using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ForeverFight.GameMechanics.Timers;
using TMPro;

namespace ForeverFight.Ui.CharacterSelection
{
    public class OfflineCharacterSelect : BaseCharacterSelect
    {
        public override void UpdateSelection(CharacterPanel selectedCharPanel)
        {
            //To prevent double clicking
            selectedCharPanel.PanelButton.interactable = false;

            currentlySelectedPanel = selectedCharPanel;
            infoDisplay.Info = selectedCharPanel.Info;
            infoDisplay.UpdateDisplayInfo();
            selectedCharPanel.Info.AbilityDescription.PopulateInfoDisplay();
            abilityDescriptionTextBox.text = selectedCharPanel.Info.AbilityDescriptionText;

            for (int i = 0; i < characterPanels.Count; i++)
            {
                if (characterPanels[i] != currentlySelectedPanel)
                {
                    characterPanels[i].Parent.SetActive(false);
                    if (characterPanels[i].Active)
                    {
                        characterPanels[i].PanelButton.interactable = true;
                    }
                }
                else
                {
                    characterPanels[i].Highlight.GetComponent<Image>().color = Color.red;

                    characterPanels[i].Parent.SetActive(true);
                    charIdentityManagerREF.SetSelectedIdentity(selectedCharPanel.Identity);
                }
            }
        }
    }
}
