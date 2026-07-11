using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.Interactable.Characters
{
    public class Elemental : Character
    {
        protected void OnEnable()
        {
            CharIdentity = Identity.Elemental;
            CharacterName = "The Elemental";
            Health = 80;
            RollAlotment = 3;
            AssignDefaultStance();
        }


        public void AttemptAbilty()
        {
            CastAbility(Moveset, AbilityNumber, this);
        }
    }
}
