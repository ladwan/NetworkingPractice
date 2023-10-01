using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.Ui;
using ForverFight.Interactable.Abilities;
using TMPro;

namespace ForverFight.Interactable.Abilities
{
    public class StatusEffect : CharAbility
    {
        public enum StatusEffectType
        {
            None = 0,
            Momentum = 1,
            Haste = 2,
            Stunned = 3,
            OffBalance = 4,
            Ire = 5,
            StunImmunity = 6,
        }


        [Serializable]
        public struct StatusEffectStruct
        {
            public StatusEffectType effectType;
            public GameObject characterSpecificUi;
            public StatusEffectDisplayFormatter formatter;
            public int duration;
        }


        [SerializeField]
        private StatusEffectType currentStatusEffectType = StatusEffectType.None;
        [SerializeField]
        private int maxAbilityDuration = 0;
        [SerializeField]
        private int currentAbilityDuration = 0;


        private Action<StatusEffectType> onStatusEffectFormatted = null;
        private Action<StatusEffectType> onStatusEffectEnded = null;
        private StatusEffectStruct formattedStatusEffectData = new();


        public Action<StatusEffectType> OnStatusEffectFormatted { get => onStatusEffectFormatted; set => onStatusEffectFormatted = value; }

        public Action<StatusEffectType> OnStatusEffectEnded { get => onStatusEffectEnded; set => onStatusEffectEnded = value; }

        public StatusEffectStruct FormattedStatusEffectData => formattedStatusEffectData;

        public StatusEffectType CurrentStatusEffectType { get => currentStatusEffectType; set => currentStatusEffectType = value; }

        public int MaxAbilityDuration { get => maxAbilityDuration; set => maxAbilityDuration = value; }

        public int CurrentAbilityDuration { get => currentAbilityDuration; set => currentAbilityDuration = value; }


        public void FormatStatusEffectDisplayData(GameObject uiToBeInstantiated, int currentDuration, StatusEffectType abilityStatusEffectType, bool isCalledFromRemote)
        {
            var instantiatedUi = InstantiateStatusEffectUi(uiToBeInstantiated, transform);
            formattedStatusEffectData.effectType = abilityStatusEffectType;
            formattedStatusEffectData.characterSpecificUi = instantiatedUi;
            formattedStatusEffectData.duration = currentDuration;
            formattedStatusEffectData.formatter = instantiatedUi.GetComponent<StatusEffectDisplayFormatter>();

            if (formattedStatusEffectData.formatter && !isCalledFromRemote)
            {
                onStatusEffectFormatted?.Invoke(abilityStatusEffectType);
                SendFormattedDataToManager();
            }
            if (!formattedStatusEffectData.formatter)
            {
                Debug.LogError("A 'StatusEffectDisplayFormatter' component could not be found !");
            }
        }

        public int UpdateStatusEffectDuration(int abilityIndex, int currentAbilityDuration, int maxAbilityDuration, StatusEffectType type, bool hasAbilityDisplay)
        {
            if (currentAbilityDuration > 0)
            {
                currentAbilityDuration--;
                if (currentAbilityDuration == 0)
                {
                    onStatusEffectEnded?.Invoke(type);
                    currentAbilityDuration = maxAbilityDuration;
                    if (hasAbilityDisplay)
                    {
                        AbilitySelectionUiManager.Instance.ToggleAbilityDisplay(abilityIndex, true, type);
                    }
                    return currentAbilityDuration;
                }
                else
                {
                    return currentAbilityDuration;
                }
            }

            Debug.Log("Status was not active or duration is already 0!");
            return 0;
        }

        public bool IsTheMostRecentlyClickedAbilty(int statusEffectIdentiferInt)
        {
            return AbilitySelectionUiManager.Instance.SelectedAbilityInt == statusEffectIdentiferInt;
        }


        private GameObject InstantiateStatusEffectUi(GameObject uiPrefab, Transform parentTransform)
        {
            var tempUiDisplay = Instantiate(uiPrefab, parentTransform);
            return tempUiDisplay;
        }

        private void SendFormattedDataToManager()
        {
            StatusEffectStaticManager.Instance.LocalStatusEffectDisplayManager.AddStatusEffectDisplay(formattedStatusEffectData);
        }
    }
}
