using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.Interactable.Abilities
{
    public interface IPassiveAbility
    {
        public string PassiveAbilityName { get; set; }

        public string PassiveAbilityDescription { get; set; }


        public void ApplyPassive()
        {
        }
    }
}
