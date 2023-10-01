using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.Ui;
using ForverFight.GameMechanics;

namespace ForverFight.Interactable.Abilities
{
    public class GroundPound : CharAbility
    {
        [SerializeField]
        private OffBalance offBalanceREF = null;
        [SerializeField]
        private Ire ireREF = null;


        private GameObject originalRadius = null;


        public GameObject OriginalRadius { get => originalRadius; set => originalRadius = value; }


        protected GroundPound()
        {
            AbilityName = "Ground Pound";
            AbilityDescription = "HUlk SMASH!?";
            AbilityDamage = 10;
            AbilityCost = 2;
        }

        protected void Awake()
        {
            originalRadius = AbilityRadius;
        }


        public override void CastAbility()
        {
            var pathFromUsToEnemy = FloorGrid.Instance.ProceduralGridManipulationREF.ReturnProceduralPath();
            if (pathFromUsToEnemy != null && pathFromUsToEnemy.Count > 0)
            {
                switch (pathFromUsToEnemy.Count)
                {
                    case 2:
                        DamageManager.Instance.DealDamage(AbilityDamage);
                        offBalanceREF.CastAbility();
                        break;
                    case 3:
                        FloorGrid.Instance.ProceduralGridManipulationREF.PullEnemy(1);
                        if (ireREF.StatusActive)
                        {
                            DamageManager.Instance.DealDamage(AbilityDamage);
                            offBalanceREF.CastAbility();
                        }
                        break;
                    case 4:
                        FloorGrid.Instance.ProceduralGridManipulationREF.PullEnemy(2);
                        DamageManager.Instance.DealDamage(AbilityDamage);
                        offBalanceREF.CastAbility();
                        break;

                    default:
                        Debug.LogError("Abnormal count of Path in Ground Pound");
                        break;
                }
            }

            //offBalanceREF.CastAbility();
        }

        public void SetAbilityRadius(GameObject radius)
        {
            AbilityRadius = radius;
        }
    }
}
