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


        public FasterPassive FasterPassive { get => fasterPassive; set => fasterPassive = value; }


        //momentum is lost if immobilized or if player does not use it before 2 turns
        protected void OnEnable()
        {
            CharIdentity = Identity.Speedster;
            CharacterName = "The Speedster";
            Health = 100;
            RollAlotment = 5;

            if (FloorGrid.Instance)
            {
                FloorGrid.Instance.OnMoveConfirmed += MoveWasConfirmed;
                return;
            }
        }

        protected void OnDisable()
        {
            if (FloorGrid.Instance)
            {
                FloorGrid.Instance.OnMoveConfirmed -= MoveWasConfirmed;
            }
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


        private void MoveWasConfirmed(int value)
        {
            if (!hasteREF.StatusActive)
            {
                LocalStoredNetworkData.GetCountdownTimerScript().TellNetworkToToggleTimer(); //This should be called to turn the timer back on, timer should be shut off by Augemented Movement Manager
            }
        }
    }
}
