using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Interactable.Abilities;
using ForeverFight.Interactable.Characters;

namespace ForeverFight.HelperScripts.Animation
{
    public class ShakeShakeOnAnimationEvent : MonoBehaviour
    {
        [SerializeField] private Character characterREF = null;


        private List<CharAbility> moveSet = new List<CharAbility>();


        protected void Start()
        {
            moveSet = characterREF.Moveset;
        }

        public void Shake(int abilityIndex)
        {
            moveSet[abilityIndex].ShakeCamera();
        }
    }
}
