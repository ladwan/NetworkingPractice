using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForverFight.Interactable.Characters
{
    public class Brawn : Character
    {
        /*
        Passive : Hard Knock Life
        ---------------------------
            Dealing damage will build 'Off-Balanced' stacks on enemy at 5* stacks enemy will be stunned for a turn
            Once per Turn the second time you damage an enemy an additional stack of off balance is applied to them
                Functionally a stun will skip a players turn
                    +50 base life


        ---------------------------
        1st Ability : Haymaker
        ---------------------------
            1sq radius 
            cost 4* AP
            Deals 10* damage


        ---------------------------    
        2nd Ability : Leap
        ---------------------------
            jump in the air to target sq
            can jump 3 sqs away
            cost 5* AP
            Deals 0 damage  


        ---------------------------    
        Ultimate Ability : Ire
        ---------------------------
            Empowers brawn for 3* turns boosting the capibility of his other abilities

                Haymaker :
                    Now deals 20* damage, knocks the target back 3 sqs and applies 2 stacks of Off balance
                        If opponent is knocked agaist a wall while ire is active they are stunned , all stacks of off balance are removed

                Leap : 
                    can leap 5 sq
                        damage radius when landing is a 2 sq radius
                             Now deals 10* damage and pulls enemies in 1 sq towards the brawn, applies two stacks of off balance to the enemy.

        */
        protected void OnEnable()
        {
            CharIdentity = Identity.Brawn;
            CharacterName = "The Brawn";
            Health = 150;
            RollAlotment = 3;
        }


        public void AttemptAbilty()
        {
            CastAbility(Moveset, AbilityNumber, this);
        }
    }
}
