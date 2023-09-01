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
        private GameObject stunnedDisplayUi = null;
        [SerializeField]
        private GameObject stunnedPopup = null;
        [SerializeField]
        private bool isStunned = false; // make a system for the remote character to set status effects on the local player , vice versa. Local stored networked data or a similar class seems like a good place to  put the logic 


        private bool statusActive = false;
        private int delayUntilStunEndsTurn = 3;

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
            AbilityFunctionality();
        }

        public void StopAbility()
        {
            if (statusActive)
            {
                CurrentAbilityDuration = 1;
                CurrentAbilityDuration = UpdateStatusEffectDuration(2, CurrentAbilityDuration, MaxAbilityDuration, CurrentStatusEffectType);
                var localStatusEffectDisplayManager = StatusEffectStaticManager.Instance.LocalStatusEffectDisplayManager;
                localStatusEffectDisplayManager.CleanUpExpiredStatusEffect(localStatusEffectDisplayManager.GetMatchingStatusEffectSlot(CurrentStatusEffectType));
            }
        }


        private void AbilityFunctionality()
        {
            if (statusActive)
            {
                stunnedPopup.SetActive(true);
                StartCoroutine(Delay(delayUntilStunEndsTurn));
            }
        }

        private void InstantiateStatusEffectUiOnButton(int index, StatusEffectType type)
        {
            if (type == CurrentStatusEffectType)
            {
                Instantiate(stunnedDisplayUi, AbilitySelectionUiManager.Instance.GetTransformOfCharacterSpecificUiAtIndex(index));
            }
        }

        private void SendStatusEffectDataToBeFormatted(StatusEffectType type)
        {
            if (type == CurrentStatusEffectType)
            {
                FormatStatusEffectDisplayData(stunnedDisplayUi, CurrentAbilityDuration, CurrentStatusEffectType);
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
                    stunnedPopup.SetActive(false);
                    PlayerTurnManager.Instance.EndTurn(false);
                }
            }
        }

        private IEnumerator Delay(int value)
        {
            yield return new WaitForSecondsRealtime(value);
            CleanUp(CurrentStatusEffectType);
        }
    }
}