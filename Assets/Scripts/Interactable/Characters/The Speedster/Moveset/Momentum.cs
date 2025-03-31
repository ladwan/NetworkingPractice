using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForeverFight.Ui;
using ForeverFight.FlowControl;
using ForeverFight.GameMechanics.Movement;
using ForeverFight.Networking;

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
        [SerializeField]
        private Haste hasteREF = null;


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

        protected void Awake()
        {
            for (int i = 0; i < OwningCharacter.Moveset.Count; i++)
            {
                if (OwningCharacter.Moveset[i] == this)
                {
                    AbilityIndex = i;
                }
            }
        }

        protected enum AbilityMethodMapping
        {
            ToggleParticles = 0,
        }


        public override void NetworkedMethodCall(int methodIndex)
        {
            switch (methodIndex)
            {
                case 0:
                    ToggleParticles(false);
                    break;
                default:
                    Debug.Log($"Something weird happend in ability: {name}");
                    break;
            }
        }

        public override void CastAbility()
        {
            StatusActive = true;
            ToggleParticles(true);
            AbilitySelectionUiManager.Instance.ToggleAbilityDisplay(1, false, CurrentStatusEffectType); // Pass a 1 because you want the second index of the list because this is the second ability
            CameraShakeParameters parameters = new CameraShakeParameters();
            ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating(LocalStoredNetworkData.GetLocalCharacter().CharacterAnimationReferences.CharacterAnimator, "Momentum", parameters);

            if (hasteREF.StatusActive)
            {
                ToggleTimerAndUi.Instance.SetTriggerWithoutListeningForAnimEnd(
                    LocalStoredNetworkData.GetLocalCharacter().CharacterAnimationReferences.CharacterAnimator,
                    "Haste",
                    parameters);
            }
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
            //storedMomentum should never go down, only up. There is a change odd values will be passed into this method because ConfirmMove() is called many times in many places
            //If any value is less than 1 set it to one, worst case scenerio stored momentum will not be affected
            if (value < 1)
            {
                value = 1;
            }

            if (StatusActive)
            {
                storedMomentum += value - 1;
                onMoveConfirmed?.Invoke();
                ClientSend.SendStoredMomentumValue(storedMomentum);
            }
        }



        public override void ToggleParticles(bool networkThisCall)
        {
            ParticleGameObject.SetActive(!ParticleGameObject.activeInHierarchy);

            if (networkThisCall)
            {
                ClientSend.SendNetworkedMethodIndex(AbilityIndex, (int)AbilityMethodMapping.ToggleParticles);
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
                    ToggleParticles(true);
                }
            }
        }
        //momentum is lost if immobilized
    }
}
