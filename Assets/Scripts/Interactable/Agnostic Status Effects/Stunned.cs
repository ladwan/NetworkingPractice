using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForverFight.Ui;
using ForverFight.FlowControl;

namespace ForverFight.Interactable.Abilities
{
    public class Stunned : StatusEffect
    {
        [SerializeField]
        private GameObject stunnedPopup = null;
        [SerializeField]
        private StunImmunity stunImmunityREF = null;


        private bool statusActive = false;


        protected Stunned()
        {
            AbilityName = "Stunned";
            AbilityDescription = "Yolo !?";
            AbilityDamage = 0;
            MaxAbilityDuration = 1;
            CurrentAbilityDuration = MaxAbilityDuration;
            AbilityCost = 0;
            CurrentStatusEffectType = StatusEffectType.Stunned;
        }

        public void StunHasEnded()
        {
            stunImmunityREF.CastAbility();
        }

        public override void CastAbility()
        {
        }

        public void StopAbility()
        {
        }

        private void AbilityFunctionality()
        {
        }

        private void InstantiateStatusEffectUiOnButton(int index, StatusEffectType type)
        {
        }

        private void SendStatusEffectDataToBeFormatted(StatusEffectType type)
        {
        }

        private void UpdateAbilityDuration()
        {
        }

        private void CleanUp(StatusEffectType type)
        {
        }
    }
}