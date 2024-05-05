using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Interactable.Abilities;

namespace ForeverFight.Ui
{
    public class UiBlockers : MonoBehaviour
    {
        [SerializeField]
        private List<StatusEffect.StatusEffectType> blockerTypes = new List<StatusEffect.StatusEffectType>();
        [SerializeField]
        private List<GameObject> uiBlockerDisplays = new List<GameObject>();


        private Dictionary<StatusEffect.StatusEffectType, GameObject> keyValuePairTypeToUiBlocker = new Dictionary<StatusEffect.StatusEffectType, GameObject>();


        public List<StatusEffect.StatusEffectType> BlockerTypes => blockerTypes;

        public List<GameObject> UiBlockerDisplays { get => uiBlockerDisplays; set => uiBlockerDisplays = value; }

        public Dictionary<StatusEffect.StatusEffectType, GameObject> KeyValuePairTypeToUiBlocker { get => keyValuePairTypeToUiBlocker; set => keyValuePairTypeToUiBlocker = value; }


        protected void OnEnable()
        {
            for (int i = 0; i < blockerTypes.Count && i < uiBlockerDisplays.Count; i++)
            {
                keyValuePairTypeToUiBlocker.Add(blockerTypes[i], uiBlockerDisplays[i]);
            }
        }


        public void IsThisTypeAUiBlocker(StatusEffect.StatusEffectType type)
        {
            if (blockerTypes.Contains(type))
            {
                EnableUiBlocker(type);
            }
        }

        public void EnableUiBlocker(StatusEffect.StatusEffectType type)
        {
            if (keyValuePairTypeToUiBlocker.TryGetValue(type, out GameObject uiBlocker))
            {
                uiBlocker.SetActive(true);
            }

        }
    }
}
