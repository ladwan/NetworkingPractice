using ForverFight.Networking;
using UnityEngine;

namespace ForverFight.Interactable.Abilities
{
    public class SelectAbilityToCast : MonoBehaviour
    {
        [SerializeField]
        private CharAbility standardAbility = null;
        [SerializeField]
        private CharAbility powerfulAbility = null;
        [SerializeField]
        private CharAbility ultimateAbility = null;

        protected void OnEnable()
        {
            LocalStoredNetworkData.localPlayerSelectAbilityToCast = this;
        }
    }
}