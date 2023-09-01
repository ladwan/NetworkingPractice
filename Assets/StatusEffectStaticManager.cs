using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.FlowControl;
using ForverFight.Interactable.Abilities;

namespace ForverFight.Ui
{
    public class StatusEffectStaticManager : MonoBehaviour
    {
        [SerializeField]
        private StatusEffectDisplayManager localStatusEffectDisplayManager = null;
        [SerializeField]
        private StatusEffectDisplayManager remoteStatusEffectDisplayManager = null;
        [SerializeField]
        private List<GameObject> statusEffectDisplayPrefabs = null;


        private static StatusEffectStaticManager instance = null;


        public StatusEffectDisplayManager LocalStatusEffectDisplayManager { get => localStatusEffectDisplayManager; set => localStatusEffectDisplayManager = value; }

        public StatusEffectDisplayManager RemoteStatusEffectDisplayManager { get => remoteStatusEffectDisplayManager; set => remoteStatusEffectDisplayManager = value; }

        public List<GameObject> StatusEffectDisplayPrefabs { get => statusEffectDisplayPrefabs; set => statusEffectDisplayPrefabs = value; }

        public static StatusEffectStaticManager Instance { get => instance; set => instance = value; }


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


        public void Test(int statusEffectIdentifier, int duration, int ownership, bool endThisStatusEffect)
        {
            var type = (StatusEffect.StatusEffectType)statusEffectIdentifier;
            if (endThisStatusEffect)
            {
                StopLocalPlayerNetworkedStatusEffect(ownership, type);
                return;
            }
            var tempStatusEffect = new StatusEffect();
            tempStatusEffect.FormatStatusEffectDisplayData(statusEffectDisplayPrefabs[statusEffectIdentifier], duration, type);
            var formattedStatusEffectData = tempStatusEffect.FormattedStatusEffectData;
            //var formattedStatusEffectData = Test2(statusEffectDisplayPrefabs[statusEffectIdentifier], duration, type);
            GetCurrentDisplayManager(ownership).AddStatusEffectDisplay(formattedStatusEffectData);
        }

        private StatusEffect.StatusEffectStruct Test2(GameObject uiToBeInstantiated, int currentDuration, StatusEffect.StatusEffectType abilityStatusEffectType)
        {
            var formattedStatusEffectData = new StatusEffect.StatusEffectStruct();
            var instantiatedUi = InstantiateStatusEffectUi(uiToBeInstantiated, transform);
            formattedStatusEffectData.effectType = abilityStatusEffectType;
            formattedStatusEffectData.characterSpecificUi = instantiatedUi;
            formattedStatusEffectData.duration = currentDuration;
            formattedStatusEffectData.formatter = instantiatedUi.GetComponent<StatusEffectDisplayFormatter>();

            if (formattedStatusEffectData.formatter)
            {
                return formattedStatusEffectData;
            }
            else
            {
                Debug.LogError("A 'StatusEffectDisplayFormatter' component could not be found !");
                return formattedStatusEffectData;
            }
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


        private StatusEffectDisplayManager GetCurrentDisplayManager(int ownership)
        {
            var currentDisplayManager = ownership == 0 ? remoteStatusEffectDisplayManager : localStatusEffectDisplayManager;
            return currentDisplayManager;
        }

        private GameObject InstantiateStatusEffectUi(GameObject uiPrefab, Transform parentTransform)
        {
            var tempUiDisplay = Instantiate(uiPrefab, parentTransform);
            return tempUiDisplay;
        }
    }
}
