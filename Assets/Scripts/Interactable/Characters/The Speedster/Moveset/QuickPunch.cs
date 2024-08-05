using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Networking;
using ForeverFight.FlowControl;
using ForeverFight.GameMechanics;

namespace ForeverFight.Interactable.Abilities
{
    public class QuickPunch : CharAbility
    {
        [SerializeField]
        private Momentum momentumREF = null;


        private GameObject originalRadius = null;


        public GameObject OriginalRadius { get => originalRadius; set => originalRadius = value; }


        protected QuickPunch()
        {
            AbilityName = "Quick Punch";
            AbilityDescription = $"Ability Damage : {AbilityDamage} \n\n This abilities damage scales with the Momentum ability";
            AbilityDamage = 5;
            AbilityCost = 2;
        }

        protected void Awake()
        {
            originalRadius = AbilityRadius;
        }


        public override void CastAbility()
        {
            if (momentumREF.StatusActive)
            {
                ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating(LocalStoredNetworkData.GetLocalCharacter().CharacterAnimator, DeterminePunchAnim(momentumREF.StoredMomentum));
            }
            else
            {
                ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating(LocalStoredNetworkData.GetLocalCharacter().CharacterAnimator, DeterminePunchAnim(1));
            }

            DamageManager.Instance.DealDamage(AbilityDamage + momentumREF.Product);
            momentumREF.StopAbility();
            //AbilityRadius.SetActive(false);
            //Do animation
            //Screen shake?
            //Deal damage to enemies health bar ui
            //End turn
        }

        public void SetAbilityRadius(GameObject radius)
        {
            AbilityRadius = radius;
        }


        private string DeterminePunchAnim(int quickPunchDamage)
        {
            string animTrigger = "";

            switch (quickPunchDamage)
            {
                case > 24:
                    animTrigger = "Level 3 Punch";
                    break;

                case > 14:
                    animTrigger = "Level 2 Punch";
                    break;

                case > 0:
                    animTrigger = "Level 3 Punch";
                    break;

                default:
                    Debug.Log("Negative int passed !");
                    animTrigger = "";
                    break;
            }

            return animTrigger;
        }
    }
}
