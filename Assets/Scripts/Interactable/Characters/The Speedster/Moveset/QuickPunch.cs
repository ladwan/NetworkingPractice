using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Networking;
using ForeverFight.FlowControl;
using ForeverFight.GameMechanics;
using UnityEngine.UIElements;
using ForeverFight.Interactable.Characters;

namespace ForeverFight.Interactable.Abilities
{
    public class QuickPunch : CharAbility
    {
        [SerializeField]
        private Momentum momentumREF = null;
        [SerializeField]
        private Haste hasteREF = null;

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

            shakeParametersSettings = new List<Vector2>()
            {
                new Vector2(0.3f, 0.1f),
                new Vector2(0.5f, 0.3f),
                new Vector2(1.0f, 0.5f),
            };

            shakeParameters = ReturnParamsBasedOnSettings(shakeParametersSettings);
        }


        public override void CastAbility()
        {
            animREF = AttemptAbility(animREF);
            if (animREF == null) return;

            if (hasteREF.StatusActive)
            {
                CameraShakeParameters tempParams = new CameraShakeParameters();
                ToggleTimerAndUi.Instance.SetTriggerWithoutListeningForAnimEnd(animREF, "Haste", tempParams);
            }

            if (momentumREF.StatusActive)
            {
                ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating(animREF, DeterminePunchAnim(momentumREF.StoredMomentum), currentCameraShakeParameters);
            }
            else
            {
                ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating(animREF, DeterminePunchAnim(1), shakeParameters[0]);
            }

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
                case > 24: //24
                    animTrigger = "Level-3-Punch";
                    currentCameraShakeParameters = shakeParameters[2];
                    break;

                case > 14: //14
                    animTrigger = "Level-2-Punch";
                    currentCameraShakeParameters = shakeParameters[1];
                    break;

                case >= 0:
                    animTrigger = "Level-1-Punch";
                    currentCameraShakeParameters = shakeParameters[0];
                    break;

                default:
                    Debug.LogError("Negative int passed !");
                    animTrigger = "";
                    break;
            }

            return animTrigger;
        }

        public override void HandleAbilityResponses(Character characterREF)
        {
            var localPlayer = LocalCharacterIsCallingAbilityResponse(currentCameraShakeParameters, characterREF);
            if (!localPlayer) return;

            DamageManager.Instance.DealDamage(AbilityDamage + momentumREF.Product);
            momentumREF.StopAbility();
        }
    }
}