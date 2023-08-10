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


        private Action onStatusEffectFormatted = null;
        private StatusEffectStruct formattedStatusEffectData = new();


        public Action OnStatusEffectFormatted { get => onStatusEffectFormatted; set => onStatusEffectFormatted = value; }

        public StatusEffectStruct FormattedStatusEffectData => formattedStatusEffectData;

        public StatusEffectType CurrentStatusEffectType { get => currentStatusEffectType; set => currentStatusEffectType = value; }


        public void FormatStatusEffectDisplayData(GameObject uiToBeInstantiated, int currentDuration, StatusEffectType abilityStatusEffectType)
        {
            var instantiatedUi = InstantiateStatusEffectUi(uiToBeInstantiated, transform);
            formattedStatusEffectData.effectType = abilityStatusEffectType;
            formattedStatusEffectData.characterSpecificUi = instantiatedUi;
            formattedStatusEffectData.duration = currentDuration;
            formattedStatusEffectData.formatter = instantiatedUi.GetComponent<StatusEffectDisplayFormatter>();

            if (formattedStatusEffectData.formatter)
            {
                onStatusEffectFormatted?.Invoke();
                SendFormattedDataToManager();
            }
            else
            {
                Debug.LogError("A 'StatusEffectDisplayFormatter' component could not be found !");
            }
        }


        private GameObject InstantiateStatusEffectUi(GameObject uiPrefab, Transform parentTransform)
        {
            var tempUiDisplay = Instantiate(uiPrefab, parentTransform);
            return tempUiDisplay;
        }

        private void SendFormattedDataToManager()
        {
            StatusEffectDisplayManager.Instance.AddStatusEffectDisplay(formattedStatusEffectData);
        }
    }
}
