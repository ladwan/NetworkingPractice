using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForeverFight.Ui;
using ForeverFight.FlowControl;

namespace ForeverFight.Interactable.Abilities
{
    public class OffBalance : StatusEffect
    {
        [SerializeField]
        private StunImmunity stunImmunityREF = null;
        [SerializeField]
        private int incrementAmount = 2;


        protected OffBalance()
        {
            AbilityName = "Off-Balance";
            AbilityDescription = "I can't think of a meme !?";
            AbilityDamage = 0;
            MaxAbilityDuration = 9;
            CurrentAbilityDuration = MaxAbilityDuration;
            AbilityCost = 0;
            CurrentStatusEffectType = StatusEffectType.OffBalance;
        }

        protected void OnEnable()
        {
            PlayerTurnManager.Instance.OnTurnEnd += UpdateAbilityDuration;
            OnStatusEffectEnded += CleanUp;
        }

        protected void OnDisable()
        {
            PlayerTurnManager.Instance.OnTurnEnd -= UpdateAbilityDuration;
            OnStatusEffectEnded -= CleanUp;
        }


        public override void CastAbility()
        {
            if (!stunImmunityREF.StatusActive)
            {
                StatusActive = true;
                StatusEffectStaticManager.Instance.UpdateNetworkedStatusEffectDisplay(4, incrementAmount, 0, false);
                ClientSend.SendStatusEffectData(StatusEffect.StatusEffectType.OffBalance, incrementAmount, 1, false);
            }
        }

        public void MaxiumOffBalanceStacks()
        {
            if (!stunImmunityREF.StatusActive)
            {
                StatusActive = true;
                StatusEffectStaticManager.Instance.UpdateNetworkedStatusEffectDisplay(4, MaxAbilityDuration, 0, false);
                ClientSend.SendStatusEffectData(StatusEffect.StatusEffectType.OffBalance, MaxAbilityDuration, 1, false);
            }
        }

        public void StopAbility()
        {
            CurrentAbilityDuration = 1;
            CurrentAbilityDuration = UpdateStatusEffectDuration(000, CurrentAbilityDuration, 1, CurrentStatusEffectType, false);
            var remoteStatusEffectDisplayManager = StatusEffectStaticManager.Instance.RemoteStatusEffectDisplayManager;
            remoteStatusEffectDisplayManager.CleanUpExpiredStatusEffect(remoteStatusEffectDisplayManager.GetMatchingStatusEffectSlot(CurrentStatusEffectType));
            ClientSend.SendStatusEffectData(StatusEffect.StatusEffectType.OffBalance, CurrentAbilityDuration, 1, true);
        }


        private void AbilityFunctionality()
        {
        }

        private void InstantiateStatusEffectUiOnButton(int index, StatusEffectType type)
        {
        }

        private void SendStatusEffectDataToBeFormatted(StatusEffectType type)
        {
        }

        private void UpdateAbilityDuration()
        {
            if (StatusActive)
            {
                CurrentAbilityDuration = UpdateStatusEffectDuration(000, CurrentAbilityDuration, 1, CurrentStatusEffectType, false);
                var slot = StatusEffectStaticManager.Instance.ReturnMatchingStatusEffectSlot(CurrentStatusEffectType, 0);
                if (slot)
                {
                    if (Int32.TryParse(slot.StatusEffectDurationTmp.text, out int currentDuration))
                    {
                        CurrentAbilityDuration = currentDuration;
                    }
                }

                if (CurrentAbilityDuration >= 9)
                {
                    StatusEffectStaticManager.Instance.UpdateNetworkedStatusEffectDisplay(3, 1, 0, false);
                    ClientSend.SendStatusEffectData(StatusEffect.StatusEffectType.Stunned, 1, 1, false);
                    PlayerTurnManager.Instance.OnTurnStart += GrantStunImmunity;
                    StopAbility();
                }
            }
        }

        private void GrantStunImmunity()
        {
            stunImmunityREF.CastAbility();
            PlayerTurnManager.Instance.OnTurnStart -= GrantStunImmunity;
        }

        private void CleanUp(StatusEffectType type)
        {
            if (type == CurrentStatusEffectType)
            {
                if (CurrentAbilityDuration <= 1)
                {
                    StatusActive = false;
                }
            }
        }
    }
}