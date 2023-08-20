using System;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.FlowControl;
using ForverFight.Interactable.Abilities;

namespace ForverFight.Ui
{
    public class StatusEffectDisplayManager : MonoBehaviour
    {
        [SerializeField]
        private List<StatusEffectDisplay> statusEffectDisplaySlots = new List<StatusEffectDisplay>();


        private static StatusEffectDisplayManager instance = null;


        public List<StatusEffectDisplay> StatusEffectDisplaySlots { get => statusEffectDisplaySlots; set => statusEffectDisplaySlots = value; }

        public static StatusEffectDisplayManager Instance { get => instance; set => instance = value; }


        protected StatusEffectDisplayManager()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Debug.Log("More Than 1 Status Effect Display Manager detected, Destroying self...");
                Destroy(instance);
            }
        }

        protected void OnEnable()
        {
            PlayerTurnManager.Instance.OnTurnEnd += ReduceDurations;
        }
        protected void OnDisable()
        {
            PlayerTurnManager.Instance.OnTurnEnd -= ReduceDurations;
        }


        public void AddStatusEffectDisplay(StatusEffect.StatusEffectStruct formattedStatusEffectData)
        {
            for (int i = 0; i < statusEffectDisplaySlots.Count; i++)
            {
                if (!statusEffectDisplaySlots[i].IsOccupied)
                {
                    HandleIncomingStatusEffectUi(formattedStatusEffectData, i);
                    break;
                }
            }
        }

        public void UpdateDuration(StatusEffectDisplay display, int value)
        {
            display.StatusEffectDurationTmp.text = value.ToString();
        }

        public void CleanUpExpiredStatusEffect(StatusEffectDisplay display)
        {
            Destroy(display.CharacterSpecificUi);
            display.StatusEffectDurationTmp.text = "";
            display.DisplayedStatusEffectType = StatusEffect.StatusEffectType.None;
            display.IsOccupied = false;
        }


        //This is populating a StatusEffectDisplay(class) with values passed in from the formattedStatusEffectData taken in from the status effect.cs
        private void HandleIncomingStatusEffectUi(StatusEffect.StatusEffectStruct formattedStatusEffectData, int i)
        {
            var statusEffectDisplaySlot = statusEffectDisplaySlots[i];
            statusEffectDisplaySlot.DisplayedStatusEffectType = formattedStatusEffectData.effectType;
            statusEffectDisplaySlot.CharacterSpecificUi = formattedStatusEffectData.characterSpecificUi;
            statusEffectDisplaySlot.CharacterSpecificUi.transform.SetParent(statusEffectDisplaySlot.ParentTransform);
            statusEffectDisplaySlot.CharacterSpecificUi.transform.SetAsFirstSibling();
            statusEffectDisplaySlot.CharacterSpecificUi.transform.localScale = formattedStatusEffectData.formatter.LocalScale;
            statusEffectDisplaySlot.CharacterSpecificUi.transform.localEulerAngles = formattedStatusEffectData.formatter.LocalEulerAngles;
            UpdateDuration(statusEffectDisplaySlot, formattedStatusEffectData.duration);
            statusEffectDisplaySlot.IsOccupied = true;
        }

        private void ReduceDurations()  // This does not actually decrement the duration of a status effect. It parses its duration into an int, then decrements that int in the local scope of this method
        {
            for (int i = 0; i < statusEffectDisplaySlots.Count; i++)
            {
                if (Int32.TryParse(statusEffectDisplaySlots[i].StatusEffectDurationTmp.text, out int n))
                {
                    n--;
                    if (n <= 0)
                    {
                        CleanUpExpiredStatusEffect(statusEffectDisplaySlots[i]);
                        continue;
                    }
                    statusEffectDisplaySlots[i].StatusEffectDurationTmp.text = n.ToString();
                }
            }
        }
    }
}
