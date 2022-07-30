using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ForverFight.Ui.CharacterSelection
{
    public class CharacterSelect : MonoBehaviour
    {
        [SerializeField]
        private static CharacterSelect instance;
        [SerializeField]
        private DisplayCharacterInfo infoDisplay;
        [SerializeField]
        private List<CharacterPanel> characterPanelsList = new List<CharacterPanel>();
        [SerializeField]
        private Dictionary<int, CharacterPanel> characterPanels = new Dictionary<int, CharacterPanel>();
        [SerializeField]
        private Text username;
        [SerializeField]
        private Text otherUsername;
        [SerializeField]
        private string otherUsernameString;
        [SerializeField]
        private Text abilityDescriptionTextBox;

        [NonSerialized]
        private CharacterPanel currentlySelectedPanel;
        [NonSerialized]
        private CharacterPanel otherPlayerCurrentPanel;
        [NonSerialized]
        private CharacterPanel otherPlayerOldPanel;


        public static CharacterSelect Instance { get => instance; set => instance = value; }
        public DisplayCharacterInfo InfoDisplay { get => infoDisplay; set => infoDisplay = value; }
        public CharacterPanel CurrentlySelectedPanel { get => currentlySelectedPanel; set => currentlySelectedPanel = value; }
        public CharacterPanel OtherPlayerOldPanel { get => otherPlayerOldPanel; set => otherPlayerOldPanel = value; }
        public List<CharacterPanel> CharacterPanelsList { get => characterPanelsList; set => characterPanelsList = value; }
        public Dictionary<int, CharacterPanel> CharacterPanels { get => characterPanels; set => characterPanels = value; }
        public Text OtherUsername { get => otherUsername; set => otherUsername = value; }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("Instance already exsists, destroying object!");
                Destroy(this);
            }
        }

        private void Start()
        {
            PopulateDictionary();
            PopulateUsernameFields();
        }

        private void PopulateUsernameFields()
        {
            username.text = ClientInfo.username;
            otherUsername.text = ClientInfo.otherUsername;
        }

        private void PopulateDictionary()
        {
            for (int i = 0; i < characterPanelsList.Count; i++)
            {
                characterPanels.Add(i, characterPanelsList[i]);
            }
        }

        public void UpdateSelection(CharacterPanel selectedCharPanel)
        {
            currentlySelectedPanel = selectedCharPanel;
            infoDisplay.Info = selectedCharPanel.Info;
            infoDisplay.UpdateDisplayInfo();
            selectedCharPanel.Info.AbilityDescription.PopulateInfoDisplay();
            abilityDescriptionTextBox.text = selectedCharPanel.Info.AbilityDescriptionText;

            for (int i = 0; i < characterPanels.Count; i++)
            {
                if (characterPanels[i] != currentlySelectedPanel)
                {
                    if (otherPlayerCurrentPanel != null)
                    {
                        if (characterPanels[i] != otherPlayerCurrentPanel)
                        {
                            characterPanels[i].Parent.SetActive(false);
                        }
                    }
                    else
                    {
                        characterPanels[i].Parent.SetActive(false);
                    }
                }
                else
                {
                    if (ClientInfo.playerNumber == 1)
                    {
                        characterPanels[i].Highlight.GetComponent<Image>().color = Color.red;
                    }
                    else if (ClientInfo.playerNumber == 2)
                    {
                        characterPanels[i].Highlight.GetComponent<Image>().color = Color.blue;

                    }
                    characterPanels[i].Parent.SetActive(true);
                    ClientSend.SendSelectionData(i, ClientInfo.playerNumber, characterPanels[i].Info.CharName);
                }
            }
        }

        public void UpdateOtherPlayerSelection(int _panelIndex, int _playerIndex)
        {
            if (_playerIndex == 1)
            {
                characterPanels[_panelIndex].Highlight.GetComponent<Image>().color = Color.red;
            }
            else if (_playerIndex == 2)
            {
                characterPanels[_panelIndex].Highlight.GetComponent<Image>().color = Color.blue;
            }
            infoDisplay.Info = characterPanels[_panelIndex].Info;
            infoDisplay.UpdateOtherDisplayInfo();
            characterPanels[_panelIndex].Parent.SetActive(true);
            otherPlayerCurrentPanel = characterPanels[_panelIndex];

            if (otherPlayerOldPanel == null)
            {
                otherPlayerOldPanel = characterPanels[_panelIndex];
            }
            else
            {
                otherPlayerOldPanel.Parent.SetActive(false);
                otherPlayerOldPanel = characterPanels[_panelIndex];
            }
        }
    }
}
