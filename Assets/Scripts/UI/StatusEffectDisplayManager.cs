using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForverFight.Ui
{
    public class StatusEffectDisplayManager : MonoBehaviour
    {
        [SerializeField]
        private List<StatusEffectDisplay> statusEffectDisplays = new List<StatusEffectDisplay>();


        private static StatusEffectDisplayManager instance = null;


        public List<StatusEffectDisplay> StatusEffectDisplays { get => statusEffectDisplays; set => statusEffectDisplays = value; }

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


        public void AddStatusEffectDisplay(GameObject uiToDisplay, int duration)
        {
            for (int i = 0; i < statusEffectDisplays.Count; i++)
            {
                if (!statusEffectDisplays[i].IsOccupied)
                {
                    HandleIncomingStatusEffectUi(uiToDisplay, i, duration);
                    break;
                }
            }
        }

        public void UpdateDuration(StatusEffectDisplay display, int value)
        {
            display.StatusEffectDurationTmp.text = value.ToString();
        }

        public void ReduceDurations()
        {
            for (int i = 0; i < statusEffectDisplays.Count; i++)
            {
                if (Int32.TryParse(statusEffectDisplays[i].StatusEffectDurationTmp.text, out int n))
                {
                    n--;
                    if (n <= 0)
                    {
                        CleanUpExpiredStatusEffect(statusEffectDisplays[i]);
                        continue;
                    }
                    statusEffectDisplays[i].StatusEffectDurationTmp.text = n.ToString();
                }
            }
        }

        public void CleanUpExpiredStatusEffect(StatusEffectDisplay display)
        {
            Destroy(display.CharacterSpecificUi);
            display.StatusEffectDurationTmp.text = "";
            display.IsOccupied = false;
        }


        //This is populating a StatusEffectDisplay(class) with values passed in from the Ui taken in from the characther
        private void HandleIncomingStatusEffectUi(GameObject uiToDisplay, int i, int duration)
        {
            var statusEffectDisplay = statusEffectDisplays[i];
            statusEffectDisplay.CharacterSpecificUi = uiToDisplay;
            uiToDisplay.transform.SetParent(statusEffectDisplay.ParentTransform);
            statusEffectDisplay.CharacterSpecificUi.transform.SetAsFirstSibling();
            statusEffectDisplay.Formatter = uiToDisplay.GetComponent<StatusEffectDisplayFormatter>();

            if (statusEffectDisplay.Formatter)
            {
                uiToDisplay.transform.localScale = statusEffectDisplay.Formatter.LocalScale;
                uiToDisplay.transform.localEulerAngles = statusEffectDisplay.Formatter.LocalEulerAngles;
            }

            UpdateDuration(statusEffectDisplay, duration);
            statusEffectDisplay.IsOccupied = true;
        }
    }
}
