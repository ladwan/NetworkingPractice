using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForverFight.Interactable.Characters
{
    public class Speedster : Character
    {
        [SerializeField]
        private FasterPassive fasterPassive = null;


        public FasterPassive FasterPassive { get => fasterPassive; set => fasterPassive = value; }


        //public Identity charIdentity = Identity.Speedster;


        //Quick Punch


        //Momentum 
        //duration 2 turns
        // your next damaging ability will deal 2x the amount of squares youve moved since this was activated
        //momentum is lost if immobilized or if player does not use it before 2 turns
        //display to players how much momentum theyve built up
        protected void OnEnable()
        {
            CharIdentity = Identity.Speedster;
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
