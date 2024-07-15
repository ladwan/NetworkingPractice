using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Interactable.Characters;

namespace ForeverFight.Ui.CharacterSelection
{
    public class CharacterInfo : MonoBehaviour
    {
        [SerializeField]
        private Character.Identity identity;
        [SerializeField]
        private string charName = null;
        [SerializeField]
        private string abilities = null;
        [SerializeField]
        private AbilityDescripton abilityDescription;
        [SerializeField]
        private string abilityDescriptionText = "";
        [SerializeField]
        private CharacterIdentityManager charIdentityManagerREF;

        public string CharName { get => charName; set => charName = value; }

        public string Abilities { get => abilities; set => abilities = value; }

        public string AbilityDescriptionText { get => abilityDescriptionText; set => abilityDescriptionText = value; }

        public AbilityDescripton AbilityDescription { get => abilityDescription; set => abilityDescription = value; }


        public void SelectIdentity()
        {
            charIdentityManagerREF.SetSelectedIdentity(identity);
        }
    }
}

