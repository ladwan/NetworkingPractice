using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ForverFight.Ui.CharacterSelection
{
    public class DisplayCharacterInfo : MonoBehaviour
    {
        [SerializeField]
        private CharacterInfo info;
        [SerializeField]
        private Text nameField;
        [SerializeField]
        private Text abiliitesField;
        [SerializeField]
        private Text otherNameField;
        [SerializeField]
        private Text otherAbiliitesField;


        public CharacterInfo Info { get => info; set => info = value; }

        public Text NameFeild { get => nameField; set => nameField = value; }

        public Text AbiliitesFeild { get => abiliitesField; set => abiliitesField = value; }

        public void UpdateDisplayInfo()
        {
            nameField.text = info.CharName;
            info.Abilities = info.Abilities.Replace("\\n", "\n");
            abiliitesField.text = info.Abilities;

        }
        public void UpdateOtherDisplayInfo()
        {
            otherNameField.text = info.CharName;
            info.Abilities = info.Abilities.Replace("\\n", "\n");
            otherAbiliitesField.text = info.Abilities;

        }
    }

}
