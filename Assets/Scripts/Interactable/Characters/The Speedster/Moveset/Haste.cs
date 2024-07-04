using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForeverFight.Ui;
using ForeverFight.Movement;
using ForeverFight.FlowControl;

namespace ForeverFight.Interactable.Abilities
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
        [SerializeField]
        private MoveToRandomGPs moveToRandomGPsREF = null;


        protected Haste()
        {
            AbilityName = "Haste";
            AbilityDescription = "Doubles the amount of movement AP gained from your passive. Increases the attack radius of Quick Punch";
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
            StatusActive = true;
            AbilitySelectionUiManager.Instance.ToggleAbilityDisplay(2, false, CurrentStatusEffectType); // Pass a 2 because you want the third index of the list because this is the third ability
            AbilityFunctionality();
            ClientSend.SendStatusEffectData(StatusEffect.StatusEffectType.Haste, CurrentAbilityDuration, 0, false);
        }

        public void StopAbility()
        {
            if (StatusActive)
            {
                CurrentAbilityDuration = 1;
                CurrentAbilityDuration = UpdateStatusEffectDuration(2, CurrentAbilityDuration, MaxAbilityDuration, CurrentStatusEffectType, true);
                var localStatusEffectDisplayManager = StatusEffectStaticManager.Instance.LocalStatusEffectDisplayManager;
                localStatusEffectDisplayManager.CleanUpExpiredStatusEffect(localStatusEffectDisplayManager.GetMatchingStatusEffectSlot(CurrentStatusEffectType));
                ClientSend.SendStatusEffectData(StatusEffect.StatusEffectType.Haste, CurrentAbilityDuration, 0, true);
            }
        }


        private void AbilityFunctionality()
        {
            if (StatusActive)
            {
                fasterPassiveREF.SetMaxPassiveApPool(6);
                quickPunchREF.SetAbilityRadius(increasedQuickPunchRadius);
                AugmentedMovementManager.Instance.ToggleAugmentMovement(moveToRandomGPsREF);
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
                FormatStatusEffectDisplayData(hasteDisplayUi, CurrentAbilityDuration, CurrentStatusEffectType, false);
            }
        }

        private void UpdateAbilityDuration()
        {
            if (StatusActive)
            {
                CurrentAbilityDuration = UpdateStatusEffectDuration(2, CurrentAbilityDuration, MaxAbilityDuration, CurrentStatusEffectType, true);
            }
        }

        private void CleanUp(StatusEffectType type)
        {
            if (type == CurrentStatusEffectType)
            {
                if (CurrentAbilityDuration <= 1)
                {
                    StatusActive = false;
                    fasterPassiveREF.SetMaxPassiveApPool(3);
                    quickPunchREF.SetAbilityRadius(quickPunchREF.OriginalRadius);
                    AugmentedMovementManager.Instance.ToggleAugmentMovement(moveToRandomGPsREF);
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