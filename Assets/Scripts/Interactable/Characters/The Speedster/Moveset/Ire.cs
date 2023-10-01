using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForverFight.Ui;
using ForverFight.FlowControl;

namespace ForverFight.Interactable.Abilities
{
    public class Ire : StatusEffect
    {
        [SerializeField]
        private GroundPound groundPoundREF = null;
        [SerializeField]
        private Haymaker haymakerREF = null;
        [SerializeField]
        private GameObject ireDisplayUi = null;
        [SerializeField]
        private GameObject increasedGroundPoundRadius = null;


        private bool statusActive = false;


        public bool StatusActive { get => statusActive; set => statusActive = value; }


        protected Ire()
        {
            AbilityName = "Ire";
            AbilityDescription = "Its over 9000 !?";
            AbilityDamage = 0;
            MaxAbilityDuration = 4;
            CurrentAbilityDuration = MaxAbilityDuration;
            AbilityCost = 4;
            CurrentStatusEffectType = StatusEffectType.Ire;
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
            ClientSend.SendStatusEffectData(StatusEffect.StatusEffectType.Ire, CurrentAbilityDuration, 0, false);
        }

        public void StopAbility()
        {
            if (statusActive)
            {
                CurrentAbilityDuration = 1;
                CurrentAbilityDuration = UpdateStatusEffectDuration(2, CurrentAbilityDuration, MaxAbilityDuration, CurrentStatusEffectType, true);
                var localStatusEffectDisplayManager = StatusEffectStaticManager.Instance.LocalStatusEffectDisplayManager;
                localStatusEffectDisplayManager.CleanUpExpiredStatusEffect(localStatusEffectDisplayManager.GetMatchingStatusEffectSlot(CurrentStatusEffectType));
                ClientSend.SendStatusEffectData(StatusEffect.StatusEffectType.Ire, CurrentAbilityDuration, 0, true);
            }
        }


        private void AbilityFunctionality()
        {
            if (statusActive)
            {
                groundPoundREF.SetAbilityRadius(increasedGroundPoundRadius);
                groundPoundREF.AbilityDamage = 20;
                haymakerREF.AbilityDamage = 25;
            }
        }

        private void InstantiateStatusEffectUiOnButton(int index, StatusEffectType type)
        {
            if (type == CurrentStatusEffectType)
            {
                Instantiate(ireDisplayUi, AbilitySelectionUiManager.Instance.GetTransformOfCharacterSpecificUiAtIndex(index));
            }
        }

        private void SendStatusEffectDataToBeFormatted(StatusEffectType type)
        {
            if (type == CurrentStatusEffectType)
            {
                FormatStatusEffectDisplayData(ireDisplayUi, CurrentAbilityDuration, CurrentStatusEffectType, false);
            }
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
                groundPoundREF.SetAbilityRadius(groundPoundREF.OriginalRadius);
                groundPoundREF.AbilityDamage = 10;
                haymakerREF.AbilityDamage = 15;
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