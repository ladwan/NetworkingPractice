using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactable.Characters
{
    public class Speedster : Character
    {
        public Identity charIdentity = Identity.Speedster;

        private Abilty basicAbility = new Abilty();
        private Abilty strongAbility = new Abilty();
        private Abilty ultimateAbility = new Abilty();

        protected void OnEnable()
        {
            CharacterName = "The Speedster";
            Health = 100;
            RollAlotment = 5;

            PopulateMoveset();
        }

        private void PopulateMoveset()
        {
            basicAbility.AbilityName = "Quick Punch";
            basicAbility.AbilityRadius = oneSqRadius;
            basicAbility.AbilityDamage = 5;

            strongAbility.AbilityName = "Faster";
            strongAbility.AbilityRadius = oneSqRadius;
            strongAbility.AbilityDamage = 0;

            ultimateAbility.AbilityName = "Tornado";
            ultimateAbility.AbilityRadius = twoSqRadius;
            ultimateAbility.AbilityDamage = 15;

            Moveset.Add(basicAbility);
            Moveset.Add(strongAbility);
            Moveset.Add(ultimateAbility);
        }

        public void AttemptAbilty()
        {
            CastAbility(Moveset, RollDice.RandomRoll(), this);
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
