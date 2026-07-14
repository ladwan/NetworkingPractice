using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Ui;
using ForeverFight.GameMechanics;
using ForeverFight.GameMechanics.Movement;
using ForeverFight.FlowControl;
using ForeverFight.HelperScripts;
using System;
using ForeverFight.Interactable.Characters;

namespace ForeverFight.Interactable.Abilities
{
    public class GroundPound : CharAbility
    {
        [SerializeField]
        private OffBalance offBalanceREF = null;
        [SerializeField]
        private Ire ireREF = null;
        // Distance bands replacing the old grid path-count switch (counts 2/3/4 with
        // 1-unit cells = distances 1/2/3); midpoints preserve the old feel.
        [SerializeField]
        private float directHitRange = 1.5f;
        [SerializeField]
        private float midRange = 2.5f;
        [SerializeField]
        private float farRange = 3.5f;


        private GameObject originalRadius = null;


        public GameObject OriginalRadius { get => originalRadius; set => originalRadius = value; }


        protected GroundPound()
        {
            AbilityName = "Ground Pound";
            AbilityDescription = "Hulk SMASH!?";
            AbilityDamage = 10;
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

            var index = ireREF.StatusActive ? 2 : 0;
            var trigger = DetermineAbilityAnim();

            ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating(animREF, trigger, shakeParameters[index]);
        }

        public override void HandleAbilityResponses(Character characterREF)
        {
            var localPlayer = LocalCharacterIsCallingAbilityResponse(currentCameraShakeParameters, characterREF);
            vfx.BeginVFX();
            if (!localPlayer) return;

            AbilityAfterEffects();
        }


        private string DetermineAbilityAnim()
        {
            string animTrigger = ireREF.StatusActive ? "Ground Pound" : "Stomp";
            currentCameraShakeParameters = ireREF.StatusActive ? shakeParameters[2] : shakeParameters[0];

            return animTrigger;
        }

        private void AbilityAfterEffects()
        {
            float distanceToEnemy = ForcedDisplacement.Instance.DistanceBetweenPlayers();

            if (distanceToEnemy <= directHitRange)
            {
                DamageManager.Instance.DealDamage(AbilityDamage);
                offBalanceREF.CastAbility();
            }
            else if (distanceToEnemy <= midRange)
            {
                ForcedDisplacement.Instance.PullEnemyToMeleeRange();
                if (ireREF.StatusActive)
                {
                    DamageManager.Instance.DealDamage(AbilityDamage);
                    offBalanceREF.CastAbility();
                }
            }
            else if (distanceToEnemy <= farRange)
            {
                ForcedDisplacement.Instance.PullEnemyToMeleeRange();
                DamageManager.Instance.DealDamage(AbilityDamage);
                offBalanceREF.CastAbility();
            }
            else
            {
                Debug.LogError("Ground Pound fired outside its expected range");
            }
        }


        public void SetAbilityRadius(GameObject radius)
        {
            AbilityRadius = radius;
        }
    }
}
