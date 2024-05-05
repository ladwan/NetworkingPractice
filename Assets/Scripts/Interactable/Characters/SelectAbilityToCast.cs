using ForeverFight.Networking;
using UnityEngine;

namespace ForeverFight.Interactable.Abilities
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