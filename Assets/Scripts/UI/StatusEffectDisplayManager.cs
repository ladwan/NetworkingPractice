using System;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.FlowControl;
using ForverFight.Interactable.Abilities;

namespace ForverFight.Ui
{
    public class StatusEffectDisplayManager : MonoBehaviour
    {
        public enum StatusEffectDisplayManagerType
        {
            None = 0,
            Local = 1,
            Remote = 2,
        }

        [SerializeField]
        private StatusEffectDisplayManagerType type = StatusEffectDisplayManagerType.None;
        [SerializeField]
        private List<StatusEffectDisplay> statusEffectDisplaySlots = new List<StatusEffectDisplay>();

        private bool rightSideIsBeingUpdated;


        public StatusEffectDisplayManagerType Type { get => type; set => type = value; }

        public List<StatusEffectDisplay> StatusEffectDisplaySlots { get => statusEffectDisplaySlots; set => statusEffectDisplaySlots = value; }

        public bool RightSideIsBeingUpdated { get => rightSideIsBeingUpdated; set => rightSideIsBeingUpdated = value; }



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
            // UpdateStatusEffectDisplayOrder();
        }

        public StatusEffectDisplay GetMatchingStatusEffectSlot(StatusEffect.StatusEffectType type)
        {
            for (int i = 0; i < statusEffectDisplaySlots.Count; i++)
            {
                var displaySlot = StatusEffectDisplaySlots[i];

                if (displaySlot.DisplayedStatusEffectType == type)
                {
                    return displaySlot;
                }
            }

            Debug.LogError("No slot with matching type was found !");
            return null;
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

        public void ReduceDurations()  // This does not actually decrement the duration of a status effect. It parses its duration into an int, then decrements that int in the local scope of this method
        {
            if (type == StatusEffectDisplayManagerType.Local || rightSideIsBeingUpdated)
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

                if (rightSideIsBeingUpdated)
                {
                    rightSideIsBeingUpdated = false;
                }
            }

        }

        /* // Finish this system later
         * 
         * 
        private List<StatusEffectDisplay> GetAllActiveStatusEffectDisplays(int index)
        {
            var slots = new List<StatusEffectDisplay>();
            for (int i = index; i < statusEffectDisplaySlots.Count; i++)
            {
                if (statusEffectDisplaySlots[i].IsOccupied)
                {
                    slots.Add(statusEffectDisplaySlots[i]);
                }
            }

            return slots;
        }


        private void UpdateStatusEffectDisplayOrder()
        {
            if (indexOfDisplayToBeDeleted + 1 <= statusEffectDisplaySlots.Count)
            {
                var slots = GetAllActiveStatusEffectDisplays(indexOfDisplayToBeDeleted + 1);
                for (int i = 0; i < statusEffectDisplaySlots.Count; i++)
                {
                    //we get indexOfDisplayToBeDeleted because it will be the first empty slot base on the logic this far, then we increment it by i so it will work with the rest of the slots to come
                    //basically the flow will be to check if the slot before you is empty , then move your logic there, then empty your old slot, increment the list, then repeat
                    if (!statusEffectDisplaySlots[indexOfDisplayToBeDeleted + i].IsOccupied)
                    {
                        TransferSlotInformationToPreviousSlot(indexOfDisplayToBeDeleted + i);
                    }
                    else
                    {
                        Debug.Log($"Previous Slot was occuiped at index {indexOfDisplayToBeDeleted + i}  Ended loop!");
                    }
                }

            }
        }


        private void TransferSlotInformationToPreviousSlot(int indexAddend)
        {
            statusEffectDisplaySlots[indexAddend + 1].CharacterSpecificUi.transform.SetParent(statusEffectDisplaySlots[indexAddend].ParentTransform);
            statusEffectDisplaySlots[indexAddend + 1].transform.SetAsFirstSibling();

            //display.StatusEffectDurationTmp.text = "";
            //display.DisplayedStatusEffectType = StatusEffect.StatusEffectType.None;
            //display.IsOccupied = false;
        }

         * Status effect displays will update themselves to go to the first open empty slot, when a status effect is deleted
         * Make sure slot before your index is empty
         * move your data to the slot before you
         * clear out the preivous slot you occupied
         * repeat on the next index
         * 
         * 
         * check to see if there are status effects slot taken after your index
         * 
         */
    }
}
