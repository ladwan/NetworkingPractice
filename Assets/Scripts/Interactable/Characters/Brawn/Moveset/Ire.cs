using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForeverFight.Ui;
using ForeverFight.FlowControl;
using ForeverFight.Interactable.Characters;
using ForeverFight.GameMechanics.Movement;
using ForeverFight.HelperScripts;

namespace ForeverFight.Interactable.Abilities
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
        [SerializeField] private IreVFXManager ireVFXManagerREF = null;


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

        protected void Awake()
        {
            shakeParametersSettings = new List<Vector2>()
            {
                new Vector2(1f, 0.25f),
            };

            shakeParameters = ReturnParamsBasedOnSettings(shakeParametersSettings);
            currentCameraShakeParameters = shakeParameters[0];
        }


        protected void OnEnable()
        {
            FloorGrid.Instance.OnMoveConfirmed += SmallIreMovement;
            PlayerTurnManager.Instance.OnTurnEnd += UpdateAbilityDuration;
            AbilitySelectionUiManager.Instance.OnSpawnButtonUi += InstantiateStatusEffectUiOnButton;
            AbilitySelectionUiManager.Instance.OnReadyToBeFormatted += SendStatusEffectDataToBeFormatted;
            OnStatusEffectEnded += CleanUp;
        }

        protected void OnDisable()
        {
            FloorGrid.Instance.OnMoveConfirmed -= SmallIreMovement;
            PlayerTurnManager.Instance.OnTurnEnd -= UpdateAbilityDuration;
            AbilitySelectionUiManager.Instance.OnSpawnButtonUi -= InstantiateStatusEffectUiOnButton;
            AbilitySelectionUiManager.Instance.OnReadyToBeFormatted -= SendStatusEffectDataToBeFormatted;
            OnStatusEffectEnded -= CleanUp;
        }


        public override void CastAbility()
        {
            animREF = AttemptAbility(animREF);
            if (animREF == null) return;

            StatusActive = true;
            //ireVFXManagerREF.BeginCoroutine(this);
            OwningCharacter.MovementIndex = 1;
            AbilitySelectionUiManager.Instance.ToggleAbilityDisplay(2, false, CurrentStatusEffectType); // Pass a 2 because you want the third index of the list because this is the third ability
            AbilityFunctionality();
            ClientSend.SendStatusEffectData(StatusEffect.StatusEffectType.Ire, CurrentAbilityDuration, 0, false);
            ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating(animREF, "Ire", shakeParameters[0]);
        }

        public void StopAbility()
        {
            if (StatusActive)
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
            if (StatusActive)
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
            if (StatusActive)
            {
                CurrentAbilityDuration = UpdateStatusEffectDuration(2, CurrentAbilityDuration, MaxAbilityDuration, CurrentStatusEffectType, true);
            }
        }

        public override void HandleAbilityResponses(Character characterREF)
        {
            var localPlayer = LocalCharacterIsCallingAbilityResponse(currentCameraShakeParameters, characterREF);
            ireVFXManagerREF.BeginCoroutine(this);
            if (!localPlayer) return;
        }

        private void CleanUp(StatusEffectType type)
        {
            if (type == CurrentStatusEffectType && CurrentAbilityDuration <= 1)
            {
                StatusActive = false;
                groundPoundREF.SetAbilityRadius(groundPoundREF.OriginalRadius);
                groundPoundREF.AbilityDamage = 10;
                haymakerREF.AbilityDamage = 15;
                OwningCharacter.MovementIndex = 0;
                var shake = new CameraShakeParameters();
                ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating(animREF, "Ire Idle to Idle", shake);
            }
        }

        private void SmallIreMovement(int sqs)
        {
            if (!StatusActive) return;
            if (sqs >= 4) return;

            ToggleTimerAndUi.Instance.SetTriggerWithoutListeningForAnimEnd(animREF, "Small", shakeParameters[0]);
            //ExecuteMethodAfterDelay.Instance.BeginDelay(1.5f,ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating);
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