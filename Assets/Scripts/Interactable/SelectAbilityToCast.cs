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
            Debug.Log("Yooo! I Ran! : 01 ");
        }
        /*
        public void SelectAbility()
        {
            switch (DistributedDieValue.distributedDieRollValue)
            {
                case 1:
                    standardAbility.CastAbility();
                    break;

                case 2:
                    standardAbility.CastAbility();
                    break;

                case 3:
                    standardAbility.CastAbility();
                    break;

                case 4:
                    standardAbility.CastAbility();
                    break;

                case 5:
                    standardAbility.CastAbility();
                    break;

                case 6:
                    standardAbility.CastAbility();
                    break;

                default:
                    break;
            }
        }

        public void ToggleAbilityRadius(bool value)
        {
            switch (DistributedDieValue.distributedDieRollValue)
            //switch (Random.Range(1,6))
            {
                case 1:
                    if (standardAbility.AbilityRadius != null)
                    {
                        standardAbility.ToggleTargeting(value);
                    }
                    break;

                case 2:
                    if (standardAbility.AbilityRadius != null)
                    {
                        standardAbility.ToggleTargeting(value);
                    }
                    break;

                case 3:
                    if (standardAbility.AbilityRadius != null)
                    {
                        standardAbility.ToggleTargeting(value);
                    }
                    break;

                case 4:
                    if (standardAbility.AbilityRadius != null)
                    {
                        standardAbility.ToggleTargeting(value);
                    }
                    break;

                case 5:
                    if (standardAbility.AbilityRadius != null)
                    {
                        standardAbility.ToggleTargeting(value);
                    }
                    break;

                case 6:
                    if (standardAbility.AbilityRadius != null)
                    {
                        standardAbility.ToggleTargeting(value);
                    }
                    break;

                default:
                    break;
            }
        }*/
    }
}