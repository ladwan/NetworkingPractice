using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForverFight.Ui;
using ForverFight.FlowControl;

namespace ForverFight.Interactable.Abilities
{
    public class Haste : StatusEffect
    {
        [SerializeField]
        private FasterPassive fasterPassiveREF = null;
        [SerializeField]
        private QuickPunch quickPunchREF = null;
        [SerializeField]
        private GameObject hasteDisplayUi = null;
        [SerializeField]
        private GameObject increasedQuickPunchRadius = null;


        private bool statusActive = false;


        protected Haste()
        {
            AbilityName = "Haste";
            AbilityDescription = "Are you winning son !?";
            AbilityDamage = 0;
            MaxAbilityDuration = 4;
            CurrentAbilityDuration = MaxAbilityDuration;
            AbilityCost = 7;
            CurrentStatusEffectType = StatusEffectType.Haste;
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
            AbilitySelectionUiManager.Instance.ToggleAbilityDisplay(2, false, CurrentStatusEffectType); // Pass a 2 because you want the third index of the list because this is the third ability
            AbilityFunctionality();
        }

        public void StopAbility()
        {
            if (statusActive)
            {
                CurrentAbilityDuration = 1;
                CurrentAbilityDuration = UpdateStatusEffectDuration(2, CurrentAbilityDuration, MaxAbilityDuration, CurrentStatusEffectType);
                StatusEffectDisplayManager.Instance.CleanUpExpiredStatusEffect(GetMatchingStatusEffectSlot(CurrentStatusEffectType));
            }
        }


        private void AbilityFunctionality()
        {
            if (statusActive)
            {
                fasterPassiveREF.SetMaxPassiveApPool(6);
                quickPunchREF.SetAbilityRadius(increasedQuickPunchRadius);
            }
        }

        private void InstantiateStatusEffectUiOnButton(int index, StatusEffectType type)
        {
            if (type == CurrentStatusEffectType)
            {
                Instantiate(hasteDisplayUi, AbilitySelectionUiManager.Instance.GetTransformOfCharacterSpecificUiAtIndex(index));
            }
        }

        private void SendStatusEffectDataToBeFormatted(StatusEffectType type)
        {
            if (type == CurrentStatusEffectType)
            {
                FormatStatusEffectDisplayData(hasteDisplayUi, CurrentAbilityDuration, CurrentStatusEffectType);
            }
        }

        private void UpdateAbilityDuration()
        {
            if (statusActive)
            {
                CurrentAbilityDuration = UpdateStatusEffectDuration(2, CurrentAbilityDuration, MaxAbilityDuration, CurrentStatusEffectType);
            }
        }

        private void CleanUp(StatusEffectType type)
        {
            if (type == CurrentStatusEffectType)
            {
                if (CurrentAbilityDuration <= 1)
                {
                    statusActive = false;
                    fasterPassiveREF.SetMaxPassiveApPool(3);
                    quickPunchREF.SetAbilityRadius(quickPunchREF.OriginalRadius);
                }
            }
        }
    }

    /*
    The ult cost X amount of Ap
    After this turn for the next 3 turn you turn up!
    Passive action point pool doubles to 6
    attack radius doubles to 2sq's

    after those 3 turn, undo this effect
    */
}