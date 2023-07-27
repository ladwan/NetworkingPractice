using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.GameMechanics;

namespace ForverFight.Interactable.Abilities
{
    public class QuickPunch : CharAbility
    {
        [SerializeField]
        private Momentum momentumREF = null;

        protected QuickPunch()
        {
            AbilityName = "Quick Punch";
            AbilityDescription = "Dayum sun where'd you find this !?";
            AbilityDamage = 5;
            AbilityCost = 2;
        }


        public override void CastAbility()
        {
            DamageManager.Instance.DealDamage(AbilityDamage + momentumREF.Product);
            momentumREF.StopAbility();
            //AbilityRadius.SetActive(false);
            //Do animation
            //Screen shake?
            //Deal damage to enemies health bar ui
            //End turn
        }
    }
}
