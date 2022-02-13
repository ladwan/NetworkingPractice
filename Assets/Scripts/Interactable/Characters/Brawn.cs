using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactable.Characters
{
    public class Brawn : Character
    {
        public Identity charIdentity = Identity.Brawn;

        private Abilty basicAbility = new Abilty();
        private Abilty strongAbility = new Abilty();
        private Abilty ultimateAbility = new Abilty();

        protected void OnEnable()
        {
            CharacterName = "The Brawn";
            Health = 150;
            RollAlotment = 3;

            PopulateMoveset();
        }

        private void PopulateMoveset()
        {
            basicAbility.AbilityName = "Haymaker";
            basicAbility.AbilityRadius = oneSqRadius;
            basicAbility.AbilityDamage = 5;

            strongAbility.AbilityName = "Ire";
            strongAbility.AbilityRadius = oneSqRadius;
            strongAbility.AbilityDamage = 0;

            ultimateAbility.AbilityName = "Seismic Smash";
            ultimateAbility.AbilityRadius = threeSqRadius;
            ultimateAbility.AbilityDamage = 15;

            Moveset.Add(basicAbility);
            Moveset.Add(strongAbility);
            Moveset.Add(ultimateAbility);
        }

        public void AttemptAbilty()
        {
            CastAbility(Moveset, StaticDieValue.staticDieRollValue, this);
        }

        public void AttemptToTakeDamage(int damage)
        {
            Health -= damage;
        }

        public void AttemptToDealDamage()
        {

        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AttemptAbilty();
            }
        }
    }
}
