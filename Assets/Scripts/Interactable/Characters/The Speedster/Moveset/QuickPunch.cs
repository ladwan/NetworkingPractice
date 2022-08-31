using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.GameMechanics;

namespace ForverFight.Interactable.Abilities
{
    public class QuickPunch : CharAbility
    {
        [SerializeField]
        private int abilityDamage = 0;


        protected void OnEnable()
        {
            AbilityDescription = "Dayum sun where'd you find this !?";
        }

        public override void CastAbility()
        {
            AbilityRadius.SetActive(false);
            DamageManager.Instance.DealDamage(abilityDamage);
            //Do animation
            //Screen shake?
            //Deal damage to enemies health bar ui
            //End turn
        }
    }
}
