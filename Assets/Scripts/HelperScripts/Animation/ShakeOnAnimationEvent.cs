using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Interactable.Abilities;
using ForeverFight.Interactable.Characters;
using ForeverFight.Networking;

namespace ForeverFight.HelperScripts.Animation
{
    public class ShakeOnAnimationEvent : MonoBehaviour
    {
        [SerializeField] private Character characterREF = null;


        private List<CharAbility> moveSet = new List<CharAbility>();


        protected void Start()
        {
            moveSet = characterREF.Moveset;
        }

        public void Shake(int abilityIndex)
        {
            moveSet[abilityIndex].HandleAbilityResponses(characterREF);
        }

        public void SetMoveSpeed(float speed)
        {
            characterREF.MoveSpeedHelper = speed;
        }

        public void SetMovementState(int index)
        {
            characterREF.MovementIndex = index;
        }
    }
}
