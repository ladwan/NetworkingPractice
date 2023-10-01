using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForverFight.Ui;
using ForverFight.FlowControl;

namespace ForverFight.Interactable.Abilities
{
    public class StunImmunity : StatusEffect
    {
        private bool statusActive = false;


        public bool StatusActive { get => statusActive; set => statusActive = value; }


        protected StunImmunity()
        {
            AbilityName = "Stun Immunity";
            AbilityDescription = "Caint touch this !?";
            AbilityDamage = 0;
            MaxAbilityDuration = 3;
            CurrentAbilityDuration = MaxAbilityDuration;
            AbilityCost = 0;
            CurrentStatusEffectType = StatusEffectType.StunImmunity;
        }

        protected void OnEnable()
        {
            PlayerTurnManager.Instance.OnTurnEnd += UpdateAbilityDuration;
            AbilitySelectionUiManager.Instance.OnSpawnButtonUi += InstantiateStatusEffectUiOnButton;
            AbilitySelectionUiManager.Instance.OnReadyToBeFormatted += SendStatusEffectDataToBeFormatted;
            OnStatusEffectEnded += CleanUp;
        }

        protected void OnDisable()
        {
            PlayerTurnManager.Instance.OnTurnEnd -= UpdateAbilityDuration;
            AbilitySelectionUiManager.Instance.OnSpawnButtonUi -= InstantiateStatusEffectUiOnButton;
            AbilitySelectionUiManager.Instance.OnReadyToBeFormatted -= SendStatusEffectDataToBeFormatted;
            OnStatusEffectEnded -= CleanUp;
        }


        public override void CastAbility()
        {
            statusActive = true;
            StatusEffectStaticManager.Instance.UpdateNetworkedStatusEffectDisplay(6, CurrentAbilityDuration, 0, false);
            ClientSend.SendStatusEffectData(StatusEffect.StatusEffectType.StunImmunity, CurrentAbilityDuration, 1, false);
        }

        public void StopAbility()
        {
            if (statusActive)
            {
                CurrentAbilityDuration = 1;
                CurrentAbilityDuration = UpdateStatusEffectDuration(000, CurrentAbilityDuration, MaxAbilityDuration, CurrentStatusEffectType, false);
                var remoteStatusEffectDisplayManager = StatusEffectStaticManager.Instance.RemoteStatusEffectDisplayManager;
                remoteStatusEffectDisplayManager.CleanUpExpiredStatusEffect(remoteStatusEffectDisplayManager.GetMatchingStatusEffectSlot(CurrentStatusEffectType));
                ClientSend.SendStatusEffectData(StatusEffect.StatusEffectType.StunImmunity, CurrentAbilityDuration, 1, true);
            }
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
            if (statusActive)
            {
                CurrentAbilityDuration = UpdateStatusEffectDuration(2, CurrentAbilityDuration, MaxAbilityDuration, CurrentStatusEffectType, true);
            }
        }

        private void CleanUp(StatusEffectType type)
        {
            if (type == CurrentStatusEffectType && CurrentAbilityDuration <= 1)
            {
                statusActive = false;
            }
        }
    }
}