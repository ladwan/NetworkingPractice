using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.Interactable.Characters
{
    public class Brawn : Character
    {
        /*
        Passive : Hard Knock Life
        ---------------------------
            Dealing damage will build 'Off-Balanced' stacks on enemy at 9* stacks enemy will be stunned for a turn
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
        2nd Ability : Ground Pound
        ---------------------------
            smash the ground near you 
            enemies in 1 sq radius take 10 damage
                     enemies in 2 sq radius are pulled in 1 sq

            cost 4* AP
            Deals 0 damage  
            Pulls enemies in 1 sq towards the brawn
            dash and grab


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
                             Now deals 10* damage , applies two stacks of off balance to the enemy.

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
