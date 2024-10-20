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


        private CameraShakeParameters level1PunchCameraShakeParameters;
        private CameraShakeParameters level2PunchCameraShakeParameters;
        private CameraShakeParameters level3PunchCameraShakeParameters;
        private CameraShakeParameters currentCameraShakeParameters;


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
            level1PunchCameraShakeParameters = AssignCameraShakeParameterValues(0.3f, 0.1f);
            level2PunchCameraShakeParameters = AssignCameraShakeParameterValues(0.5f, 0.3f);
            level3PunchCameraShakeParameters = AssignCameraShakeParameterValues(1.0f, 0.5f);
        }


        public override void CastAbility()
        {
            if (momentumREF.StatusActive)
            {
                ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating(LocalStoredNetworkData.GetLocalCharacter().CharacterAnimationReferences.CharacterAnimator, DeterminePunchAnim(momentumREF.StoredMomentum), currentCameraShakeParameters);
            }
            else
            {
                ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating(LocalStoredNetworkData.GetLocalCharacter().CharacterAnimationReferences.CharacterAnimator, DeterminePunchAnim(1), level1PunchCameraShakeParameters);
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
                case > 24: //24
                    animTrigger = "Level-3-Punch";
                    currentCameraShakeParameters = level3PunchCameraShakeParameters;
                    break;

                case > 14: //14
                    animTrigger = "Level-2-Punch";
                    currentCameraShakeParameters = level2PunchCameraShakeParameters;
                    break;

                case > 0:
                    animTrigger = "Level-1-Punch";
                    currentCameraShakeParameters = level1PunchCameraShakeParameters;
                    break;

                default:
                    Debug.Log("Negative int passed !");
                    animTrigger = "";
                    break;
            }

            return animTrigger;
        }

        public override void ShakeCamera()
        {
            CameraControls.Instance.StartShake(currentCameraShakeParameters);
        }

        public override CameraShakeParameters AssignCameraShakeParameterValues(float duration, float magnitude)
        {
            CameraShakeParameters parameters = new CameraShakeParameters();

            parameters.duration = duration;
            parameters.magnitude = magnitude;

            return parameters;
        }
    }
}