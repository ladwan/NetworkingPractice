using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForverFight.Ui;
using ForverFight.FlowControl;

namespace ForverFight.Interactable.Abilities
{
    public class Momentum : CharAbility
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
        private MomentumDisplayReferences momentumDisplayReferencesREF = null;
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
        }

        protected void OnEnable()
        {
            FloorGrid.Instance.OnMoveConfirmed += GetHoveredOverGridPointsCount;
            PlayerTurnManager.Instance.OnTurnEnd += UpdateAbilityDuration;
        }

        protected void OnDisable()
        {
            FloorGrid.Instance.OnMoveConfirmed -= GetHoveredOverGridPointsCount;
            PlayerTurnManager.Instance.OnTurnEnd -= UpdateAbilityDuration;
        }


        public override void CastAbility()
        {
            statusActive = true;
            ToggleAbilityDisplay(1, false);
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


        private void ToggleAbilityDisplay(int index, bool toggle)
        {
            AbilitySelectionUiManager.Instance.ApCostDisplays[index].SetActive(toggle);
            AbilitySelectionUiManager.Instance.AbilityBlockers[index].SetActive(!toggle);
            AbilitySelectionUiManager.Instance.CharacterSpecificUiDisplays[index].SetActive(!toggle);

            if (toggle == false)
            {
                if (AbilitySelectionUiManager.Instance.CharacterSpecificUiDisplays[index].transform.childCount == 0)
                {
                    InstantiateMomentumUi(AbilitySelectionUiManager.Instance.CharacterSpecificUiDisplays[index].transform);
                }

                StatusEffectDisplayManager.Instance.AddStatusEffectDisplay(InstantiateMomentumUi(transform), AbilityDuration);
            }
        }

        private GameObject InstantiateMomentumUi(Transform parentTransform)
        {
            var tempMomentumDisplay = Instantiate(momentumDisplayUi, parentTransform);
            var momentumDisplayReferencesREF = tempMomentumDisplay.GetComponent<MomentumDisplayReferences>();
            momentumDisplayReferencesREF.MomentumREF = this;
            return tempMomentumDisplay;
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
                    ToggleAbilityDisplay(1, true);
                    CheckForStatusEffectCleanup();
                }
            }
        }

        private void CheckForStatusEffectCleanup()
        {
            for (int i = 0; i < StatusEffectDisplayManager.Instance.StatusEffectDisplays.Count; i++)
            {
                var display = StatusEffectDisplayManager.Instance.StatusEffectDisplays[i];
                if (display.IsOccupied)
                {
                    if (display.Formatter.CurrentStatusEffectType == StatusEffectDisplayFormatter.StatusEffectType.Momentum)
                    {
                        StatusEffectDisplayManager.Instance.CleanUpExpiredStatusEffect(display);
                    }
                }
            }
        }

        //momentum is lost if immobilized
    }
}
