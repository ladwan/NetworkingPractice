using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Ui;
using ForeverFight.GameMechanics;

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
        }


        public override void CastAbility()
        {
            if (ireREF.StatusActive)
            {
                FloorGrid.Instance.ProceduralGridManipulationREF.KnockbackEnemy(3);
            }
            DamageManager.Instance.DealDamage(AbilityDamage);
            offBalanceREF.CastAbility();
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
