using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForeverFight.Ui;
using ForeverFight.FlowControl;

namespace ForeverFight.Interactable.Abilities
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
            AbilityDescription = "Build Momentum by moving sq's. Quick Punch's damage will be increased by double the amount of stored Momentum";
            AbilityDamage = 0;
            MaxAbilityDuration = 3;
            CurrentAbilityDuration = MaxAbilityDuration;
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
            OnStatusEffectEnded += CleanUp;
        }

        protected void OnDisable()
        {
            FloorGrid.Instance.OnMoveConfirmed -= GetHoveredOverGridPointsCount;
            PlayerTurnManager.Instance.OnTurnEnd -= UpdateAbilityDuration;
            AbilitySelectionUiManager.Instance.OnSpawnButtonUi -= InstantiateStatusEffectUiOnButton;
            AbilitySelectionUiManager.Instance.OnReadyToBeFormatted -= SendStatusEffectDataToBeFormatted;
            OnStatusEffectFormatted -= PassReferences;
            OnStatusEffectEnded -= CleanUp;
        }


        public override void CastAbility()
        {
            StatusActive = true;
            AbilitySelectionUiManager.Instance.ToggleAbilityDisplay(1, false, CurrentStatusEffectType); // Pass a 1 because you want the second index of the list because this is the second ability
            ClientSend.SendStatusEffectData(StatusEffect.StatusEffectType.Momentum, CurrentAbilityDuration, 0, false);
        }

        public void StopAbility()
        {
            if (StatusActive)
            {
                CurrentAbilityDuration = 1;
                CurrentAbilityDuration = UpdateStatusEffectDuration(1, CurrentAbilityDuration, MaxAbilityDuration, CurrentStatusEffectType, true);
                var localStatusEffectDisplayManager = StatusEffectStaticManager.Instance.LocalStatusEffectDisplayManager;
                localStatusEffectDisplayManager.CleanUpExpiredStatusEffect(localStatusEffectDisplayManager.GetMatchingStatusEffectSlot(CurrentStatusEffectType));
                ClientSend.SendStatusEffectData(StatusEffect.StatusEffectType.Momentum, CurrentAbilityDuration, 0, true);
            }
        }

        public void GetHoveredOverGridPointsCount(int value)
        {
            if (StatusActive)
            {
                storedMomentum += value - 1;
                onMoveConfirmed?.Invoke();
                ClientSend.SendStoredMomentumValue(storedMomentum);
            }
        }


        private void InstantiateStatusEffectUiOnButton(int index, StatusEffectType type)
        {
            if (type == CurrentStatusEffectType)
            {
                var tempMomentumDisplay = Instantiate(momentumDisplayUi, AbilitySelectionUiManager.Instance.GetTransformOfCharacterSpecificUiAtIndex(index));
                var momentumDisplayReferencesREF = tempMomentumDisplay.GetComponent<MomentumDisplayReferences>();
                momentumDisplayReferencesREF.SubscribeToOnMoveConfirmed(this);
            }
        }

        private void SendStatusEffectDataToBeFormatted(StatusEffectType type)
        {
            if (type == CurrentStatusEffectType)
            {
                FormatStatusEffectDisplayData(momentumDisplayUi, CurrentAbilityDuration, CurrentStatusEffectType, false);
            }
        }

        private void PassReferences(StatusEffectType type)
        {
            if (type == CurrentStatusEffectType)
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
        }

        private void UpdateAbilityDuration()
        {
            if (StatusActive)
            {
                CurrentAbilityDuration = UpdateStatusEffectDuration(1, CurrentAbilityDuration, MaxAbilityDuration, CurrentStatusEffectType, true);
            }
        }

        private void CleanUp(StatusEffectType type)
        {
            if (type == CurrentStatusEffectType)
            {
                if (CurrentAbilityDuration <= 1)
                {
                    storedMomentum = 0;
                    StatusActive = false;
                }
            }
        }
        //momentum is lost if immobilized
    }
}
