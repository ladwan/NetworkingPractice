using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForverFight.Interactable.Abilities
{
    public class CharAbility : MonoBehaviour
    {
        [SerializeField]
        private string abilityName = "";
        [SerializeField]
        private string abilityDescription = "";
        [SerializeField]
        private int abilityDamage = 0;
        [SerializeField]
        private int abilityCost = 0;
        [SerializeField]
        private GameObject abilityRadius = null;
        [SerializeField]
        private List<int> intToTriggerThisAbility = new List<int>();


        public string AbilityName { get => abilityName; set => abilityName = value; }

        public string AbilityDescription { get => abilityDescription; set => abilityDescription = value; }

        public int AbilityDamage { get => abilityDamage; set => abilityDamage = value; }

        public int AbilityCost { get => abilityCost; set => abilityCost = value; }

        public GameObject AbilityRadius { get => abilityRadius; set => abilityRadius = value; }

        public List<int> IntToTriggerThisAbility { get => intToTriggerThisAbility; set => intToTriggerThisAbility = value; }


        public virtual void CastAbility()
        {
        }

        public void ToggleTargeting(bool value)
        {
            //AbilityRadius.SetActive(value);
        }
    }
}
