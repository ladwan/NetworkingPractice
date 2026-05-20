using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ForeverFight.GameMechanics.Timers;
using TMPro;

namespace ForeverFight.Ui.CharacterSelection
{
    public class BaseCharacterSelect : MonoBehaviour
    {
        [SerializeField] protected static BaseCharacterSelect instance;
        [SerializeField] protected DisplayCharacterInfo infoDisplay;
        [SerializeField] protected List<CharacterPanel> characterPanelsList = new List<CharacterPanel>();
        [SerializeField] protected Dictionary<int, CharacterPanel> characterPanels = new Dictionary<int, CharacterPanel>();
        [SerializeField] protected Text abilityDescriptionTextBox;
        [SerializeField] protected CharacterIdentityManager charIdentityManagerREF;


        [NonSerialized] protected CharacterPanel currentlySelectedPanel = null;


        public static BaseCharacterSelect Instance  => instance;
        public DisplayCharacterInfo InfoDisplay { get => infoDisplay; set => infoDisplay = value; }
        public CharacterPanel CurrentlySelectedPanel => currentlySelectedPanel;
        public List<CharacterPanel> CharacterPanelsList { get => characterPanelsList; set => characterPanelsList = value; }
        public Dictionary<int, CharacterPanel> CharacterPanels { get => characterPanels; set => characterPanels = value; }


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Debug.Log("CharacterSelect instance already exsists, destroying object!");
                Destroy(this);
            }
        }

        private void Start()
        {
            PopulateDictionary();
        }

        private void PopulateDictionary()
        {
            for (int i = 0; i < characterPanelsList.Count; i++)
            {
                characterPanels.Add(i, characterPanelsList[i]);
            }
        }


        public virtual void UpdateSelection(CharacterPanel selectedCharPanel)
        {
        }
    }
}
