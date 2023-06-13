using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForverFight.Interactable.Characters
{
    public class Speedster : Character
    {
        public Identity charIdentity = Identity.Speedster;

        //Passive : Faster  - 3 *one use per turn* movement AP 

        //Quick Punch


        //Momentum 
        //duration 2 turns
        // your next damaging ability will deal 2x the amount of squares youve moved since this was activated
        //momentum is lost if immobilized or if player does not use it before 2 turns
        //display to players how much momentum theyve built up
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
    }
}
