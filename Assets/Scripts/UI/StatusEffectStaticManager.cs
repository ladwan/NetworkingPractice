using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.FlowControl;
using ForeverFight.Interactable.Abilities;

namespace ForeverFight.Ui
{
    public class StatusEffectStaticManager : MonoBehaviour
    {
        [SerializeField]
        private StatusEffectDisplayManager localStatusEffectDisplayManager = null;
        [SerializeField]
        private StatusEffectDisplayManager remoteStatusEffectDisplayManager = null;
        [SerializeField]
        private List<GameObject> statusEffectDisplayPrefabs = null;
        [SerializeField]
        private UiBlockers uiBlockersREF = null;


        private static StatusEffectStaticManager instance = null;


        public StatusEffectDisplayManager LocalStatusEffectDisplayManager { get => localStatusEffectDisplayManager; set => localStatusEffectDisplayManager = value; }

        public StatusEffectDisplayManager RemoteStatusEffectDisplayManager { get => remoteStatusEffectDisplayManager; set => remoteStatusEffectDisplayManager = value; }

        public List<GameObject> StatusEffectDisplayPrefabs { get => statusEffectDisplayPrefabs; set => statusEffectDisplayPrefabs = value; }

        public static StatusEffectStaticManager Instance { get => instance; set => instance = value; }

        public UiBlockers UiBlockersREF { get => uiBlockersREF; set => uiBlockersREF = value; }


        protected StatusEffectStaticManager()
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
            PlayerTurnManager.Instance.OnTurnStart += ReduceLocalPlayerNetworkedStatusEffectDurations;
        }
        protected void OnDisable()
        {
            PlayerTurnManager.Instance.OnTurnStart -= ReduceLocalPlayerNetworkedStatusEffectDurations;
        }


        public void UpdateNetworkedStatusEffectDisplay(int statusEffectIdentifier, int duration, int ownership, bool endThisStatusEffect)
        {
            var type = (StatusEffect.StatusEffectType)statusEffectIdentifier;
            if (endThisStatusEffect)
            {
                StopLocalPlayerNetworkedStatusEffect(ownership, type);
                return;
            }

            if (ownership == 1)
            {
                uiBlockersREF.IsThisTypeAUiBlocker(type);
            }

            var slot = ReturnMatchingStatusEffectSlot(type, ownership);
            if (slot)
            {
                if (Int32.TryParse(slot.StatusEffectDurationTmp.text, out int currentDuration))
                {
                    currentDuration += duration;
                    if (currentDuration > 9)
                    {
                        currentDuration = 9;
                    }
                    slot.StatusEffectDurationTmp.text = currentDuration.ToString();
                    return;
                }
            }

            var tempStatusEffect = gameObject.AddComponent<StatusEffect>();
            tempStatusEffect.FormatStatusEffectDisplayData(statusEffectDisplayPrefabs[statusEffectIdentifier], duration, type, true);
            var formattedStatusEffectData = tempStatusEffect.FormattedStatusEffectData;
            GetCurrentDisplayManager(ownership).AddStatusEffectDisplay(formattedStatusEffectData);
            Destroy(tempStatusEffect);

        }

        public StatusEffectDisplay ReturnMatchingStatusEffectSlot(StatusEffect.StatusEffectType type, int ownership)
        {
            var slot = GetCurrentDisplayManager(ownership).GetMatchingStatusEffectSlot(type);
            if (!slot)
            {
                return null;
            }

            return slot;
        }


        private void ReduceLocalPlayerNetworkedStatusEffectDurations()
        {
            if (remoteStatusEffectDisplayManager.StatusEffectDisplaySlots.Count > 0)
            {
                remoteStatusEffectDisplayManager.RightSideIsBeingUpdated = true;
                remoteStatusEffectDisplayManager.ReduceDurations();
            }
        }

        private void StopLocalPlayerNetworkedStatusEffect(int ownership, StatusEffect.StatusEffectType type)
        {
            var currentDisplayManager = GetCurrentDisplayManager(ownership);
            currentDisplayManager.CleanUpExpiredStatusEffect(currentDisplayManager.GetMatchingStatusEffectSlot(type));
        }

        //From local player to enemy = 1 // From local player to there remote selves = 0
        private StatusEffectDisplayManager GetCurrentDisplayManager(int ownership)
        {
            var currentDisplayManager = ownership == 0 ? remoteStatusEffectDisplayManager : localStatusEffectDisplayManager;
            return currentDisplayManager;
        }
    }
}
