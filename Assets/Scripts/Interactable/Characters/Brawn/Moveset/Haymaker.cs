using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Ui;
using ForeverFight.GameMechanics;
using ForeverFight.GameMechanics.Movement;
using ForeverFight.Networking;
using ForeverFight.FlowControl;
using ForeverFight.Interactable.Characters;

namespace ForeverFight.Interactable.Abilities
{
    public class Haymaker : CharAbility
    {
        [SerializeField]
        private OffBalance offBalanceREF = null;
        [SerializeField]
        private Ire ireREF = null;
        private GameObject originalRadius = null;


        public GameObject OriginalRadius { get => originalRadius; set => originalRadius = value; }


        protected Haymaker()
        {
            AbilityName = "Haymaker";
            AbilityDescription = "Ohhhh yeahhh !?";
            AbilityDamage = 15;
            AbilityCost = 3;
        }

        protected void OnEnable()
        {
            FloorGrid.Instance.ProceduralGridManipulationREF.EnemyHitAWallAction += EnemyHitWall;
        }

        protected void OnDisable()
        {
            FloorGrid.Instance.ProceduralGridManipulationREF.EnemyHitAWallAction -= EnemyHitWall;
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
            if(!localPlayer) return;

            if (ireREF.StatusActive)
            {
                FloorGrid.Instance.ProceduralGridManipulationREF.KnockbackEnemy(3);
            }

            DamageManager.Instance.DealDamage(AbilityDamage);
            offBalanceREF.CastAbility();
        }

        private string DetermineAbilityAnim()
        {
            string animTrigger = ireREF.StatusActive ? "Ire Haymaker" : "Haymaker";
            currentCameraShakeParameters = ireREF.StatusActive ? shakeParameters[2] : shakeParameters[0];

            return animTrigger;
        }


        public void SetAbilityRadius(GameObject radius)
        {
            AbilityRadius = radius;
        }

        private void EnemyHitWall()
        {
            offBalanceREF.MaxiumOffBalanceStacks();
        }
    }
}
