using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForverFight.Ui.CharacterSelection
{
    public class CharacterInfo : MonoBehaviour
    {
        [SerializeField]
        private string charName = null;
        [SerializeField]
        private string abilities = null;
        [SerializeField]
        private AbilityDescripton abilityDescription;
        [SerializeField]
        private string abilityDescriptionText = "";

        public string CharName { get => charName; set => charName = value; }

        public string Abilities { get => abilities; set => abilities = value; }

        public string AbilityDescriptionText { get => abilityDescriptionText; set => abilityDescriptionText = value; }

        public AbilityDescripton AbilityDescription { get => abilityDescription; set => abilityDescription = value; }
    }
}

