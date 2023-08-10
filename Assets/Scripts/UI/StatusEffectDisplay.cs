using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForverFight.Interactable.Abilities;

namespace ForverFight.Ui
{
    public class StatusEffectDisplay : MonoBehaviour
    {
        [SerializeField]
        private Transform parentTransform = null;
        [SerializeField]
        private GameObject characterSpecificUi = null;
        [SerializeField]
        private TMP_Text statusEffectDurationTmp = null;
        [SerializeField]
        private StatusEffect.StatusEffectType displayedStatusEffectType = StatusEffect.StatusEffectType.None;
        [SerializeField]
        private StatusEffectDisplayFormatter formatter = null;
        [SerializeField]
        private bool isOccupied = false;


        public Transform ParentTransform => parentTransform;

        public GameObject CharacterSpecificUi { get => characterSpecificUi; set => characterSpecificUi = value; }

        public TMP_Text StatusEffectDurationTmp => statusEffectDurationTmp;

        public StatusEffect.StatusEffectType DisplayedStatusEffectType { get => displayedStatusEffectType; set => displayedStatusEffectType = value; }

        public StatusEffectDisplayFormatter Formatter { get => formatter; set => formatter = value; }

        public bool IsOccupied { get => isOccupied; set => isOccupied = value; }
    }
}
