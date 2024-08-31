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

        //StartDelay == X  // Duration == Y // Magnitude == Z
        private Vector3 level1PunchCameraShakeParameters = new Vector3(0.25f, .4f, 0.05f);
        private Vector3 level2PunchCameraShakeParameters = new Vector3(.8f, 0.5f, 0.25f);
        private Vector3 level3PunchCameraShakeParameters = new Vector3(1.5f, 1f, 0.5f);
        private Vector3 currentCameraShakeParameters = Vector3.zero;


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
                ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating(LocalStoredNetworkData.GetLocalCharacter().CharacterAnimator, DeterminePunchAnim(momentumREF.StoredMomentum), currentCameraShakeParameters);
            }
            else
            {
                ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating(LocalStoredNetworkData.GetLocalCharacter().CharacterAnimator, DeterminePunchAnim(1), level3PunchCameraShakeParameters);
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
                    currentCameraShakeParameters = level3PunchCameraShakeParameters;
                    CameraControls.Instance.StartShake(level3PunchCameraShakeParameters.x, level3PunchCameraShakeParameters.y, level3PunchCameraShakeParameters.z);
                    CameraControls.Instance.Zoom(0, 3f, 1f, 1f);
                    break;

                case > 14:
                    animTrigger = "Level 2 Punch";
                    currentCameraShakeParameters = level2PunchCameraShakeParameters;
                    CameraControls.Instance.StartShake(level2PunchCameraShakeParameters.x, level2PunchCameraShakeParameters.y, level2PunchCameraShakeParameters.z);
                    break;

                case > 0:
                    animTrigger = "Level 3 Punch";
                    currentCameraShakeParameters = level3PunchCameraShakeParameters;
                    CameraControls.Instance.StartShake(level3PunchCameraShakeParameters.x, level3PunchCameraShakeParameters.y, level3PunchCameraShakeParameters.z);
                    CameraControls.Instance.Zoom(0, -2f, 1f, 1f);
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