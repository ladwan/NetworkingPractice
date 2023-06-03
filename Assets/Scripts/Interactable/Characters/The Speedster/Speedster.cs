using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForverFight.Interactable.Characters
{
    public class Speedster : Character
    {
        public Identity charIdentity = Identity.Speedster;


        protected void OnEnable()
        {
            CharacterName = "The Speedster";
            Health = 100;
            RollAlotment = 5;
        }


        public void AttemptAbilty()
        {
            CastAbility(Moveset, AbilityNumber, this);
        }

        public void AttemptToTakeDamage(int damage)
        {
            Health -= damage;
        }

        public void AttemptToDealDamage()
        {

        }

        public void SetAbiltyNumber(int value)
        {
            if (value >= 0 && value < 3)
            {
                AbilityNumber = value;
            }
            else
            {
                Debug.Log("value passed for ability number was not valid");
            }
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AttemptAbilty();
            }
        }

        public void QuickPunch()
        {

        }
    }
}
