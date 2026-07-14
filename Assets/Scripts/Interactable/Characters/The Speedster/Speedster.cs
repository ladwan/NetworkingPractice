using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Networking;
using ForeverFight.Interactable.Abilities;
using ForeverFight.GameMechanics.Movement;

namespace ForeverFight.Interactable.Characters
{
    public class Speedster : Character
    {
        [SerializeField]
        private FasterPassive fasterPassive = null;
        [SerializeField]
        private Haste hasteREF = null;
        // Speedster's speed AND gait scale hard with move distance (walk -> sprint).
        [SerializeField]
        private LocomotionProfile speedsterLocomotionProfile = LocomotionProfile.CreateSpeedster();
        // Haste changes his idle (Hasted blend tree) but not his run - a separate
        // profile so the hasted feel is tunable independently. Applied by Haste.cs.
        [SerializeField]
        private LocomotionProfile hastedLocomotionProfile = LocomotionProfile.CreateSpeedsterHasted();


        public FasterPassive FasterPassive { get => fasterPassive; set => fasterPassive = value; }

        public LocomotionProfile HastedLocomotionProfile => hastedLocomotionProfile;


        //momentum is lost if immobilized or if player does not use it before 2 turns
        protected void OnEnable()
        {
            CharIdentity = Identity.Speedster;
            CharacterName = "The Speedster";
            Health = 100;
            RollAlotment = 5;
            AssignDefaultStance();
            SetBaseLocomotionProfile(speedsterLocomotionProfile);
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
