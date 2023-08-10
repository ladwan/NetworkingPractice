using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForverFight.Ui;
using ForverFight.FlowControl;

namespace ForverFight.Interactable.Abilities
{
    public class Momentum : StatusEffect
    {
        [SerializeField]
        private int storedMomentum = 0;
        [SerializeField]
        private float multiplier = 2f;
        [SerializeField]
        private int product = 0;
        [SerializeField]
        private GameObject momentumDisplayUi = null;
        [SerializeField]
        private Action onMoveConfirmed = null;


        private bool statusActive = false;


        public int StoredMomentum { get => storedMomentum; set => storedMomentum = value; }

        public Action OnMoveConfirmed { get => onMoveConfirmed; set => onMoveConfirmed = value; }

        public int Product
        {
            get
            {
                return storedMomentum * (int)multiplier;
            }
        }


        protected Momentum()
        {
            AbilityName = "Momentum";
            AbilityDescription = "Im fast as fuck boiii !?";
            AbilityDamage = 0;
            AbilityDuration = 3;
            AbilityCost = 4;
            CurrentStatusEffectType = StatusEffectType.Momentum;
        }

        protected void OnEnable()
        {
            FloorGrid.Instance.OnMoveConfirmed += GetHoveredOverGridPointsCount;
            PlayerTurnManager.Instance.OnTurnEnd += UpdateAbilityDuration;
            AbilitySelectionUiManager.Instance.OnSpawnButtonUi += InstantiateStatusEffectUiOnButton;
            AbilitySelectionUiManager.Instance.OnReadyToBeFormatted += SendStatusEffectDataToBeFormatted;
            OnStatusEffectFormatted += PassReferences;
        }

        protected void OnDisable()
        {
            FloorGrid.Instance.OnMoveConfirmed -= GetHoveredOverGridPointsCount;
            PlayerTurnManager.Instance.OnTurnEnd -= UpdateAbilityDuration;
            AbilitySelectionUiManager.Instance.OnSpawnButtonUi -= InstantiateStatusEffectUiOnButton;
            AbilitySelectionUiManager.Instance.OnReadyToBeFormatted -= SendStatusEffectDataToBeFormatted;
            OnStatusEffectFormatted -= PassReferences;
        }


        public override void CastAbility()
        {
            statusActive = true;
            AbilitySelectionUiManager.Instance.ToggleAbilityDisplay(1, false); // Pass a 1 because you want the second index of the list because this is the second ability
        }

        public void StopAbility()
        {
            if (statusActive)
            {
                AbilityDuration = 1;
                UpdateAbilityDuration();
            }
        }

        public void GetHoveredOverGridPointsCount(int value)
        {
            if (statusActive)
            {
                storedMomentum += value - 1;
                onMoveConfirmed?.Invoke();
            }
        }


        private void InstantiateStatusEffectUiOnButton(int index)
        {
            var tempMomentumDisplay = Instantiate(momentumDisplayUi, AbilitySelectionUiManager.Instance.GetTransformOfCharacterSpecificUiAtIndex(index));
            var momentumDisplayReferencesREF = tempMomentumDisplay.GetComponent<MomentumDisplayReferences>();
            momentumDisplayReferencesREF.SubscribeToOnMoveConfirmed(this);
        }

        private void SendStatusEffectDataToBeFormatted()
        {
            FormatStatusEffectDisplayData(momentumDisplayUi, AbilityDuration, CurrentStatusEffectType);
        }

        private void PassReferences()
        {
            var momentumDisplayReferencesREF = FormattedStatusEffectData.characterSpecificUi.GetComponent<MomentumDisplayReferences>();
            try
            {
                momentumDisplayReferencesREF.SubscribeToOnMoveConfirmed(this);
            }
            catch (Exception error)
            {
                Debug.LogError($"No 'MomentumDisplayReferences' component found! {error}");
            }
        }

        private void UpdateAbilityDuration()
        {
            if (statusActive && AbilityDuration > 0)
            {
                StatusEffectDisplayManager.Instance.ReduceDurations();
                AbilityDuration--;
                if (AbilityDuration == 0)
                {
                    statusActive = false;
                    storedMomentum = 0;
                    AbilityDuration = 3;
                    AbilitySelectionUiManager.Instance.ToggleAbilityDisplay(1, true);
                }
            }
        }

        //momentum is lost if immobilized
    }
}
