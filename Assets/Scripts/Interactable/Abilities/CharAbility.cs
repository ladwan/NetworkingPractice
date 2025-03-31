using System;
using System.Collections;
using System.Collections.Generic;
using ForeverFight.Interactable.Characters;
using UnityEngine;

namespace ForeverFight.Interactable.Abilities
{
    public abstract class CharAbility : MonoBehaviour
    {
        [SerializeField] private Character owningCharacter = null;
        [SerializeField]
        private string abilityName = "";
        [SerializeField]
        private string abilityDescription = "";
        [SerializeField]
        private int abilityDamage = 0;
        [SerializeField]
        private int abilityCost = 0;
        [SerializeField]
        private GameObject abilityRadius = null;
        [SerializeField]
        private int abilityIndex = -1;


        [SerializeField] private GameObject particleGameObject = null;
        [SerializeField] private MonoBehaviour particleScript = null;


        [Serializable]
        public struct CameraShakeParameters
        {
            public float duration;
            public float magnitude;
        }


        public Character OwningCharacter => owningCharacter;

        public string AbilityName { get => abilityName; set => abilityName = value; }

        public string AbilityDescription { get => abilityDescription; set => abilityDescription = value; }

        public int AbilityDamage { get => abilityDamage; set => abilityDamage = value; }

        public int AbilityCost { get => abilityCost; set => abilityCost = value; }

        public GameObject AbilityRadius { get => abilityRadius; set => abilityRadius = value; }

        public GameObject ParticleGameObject => particleGameObject;

        public MonoBehaviour ParticleScript => particleScript;

        public int AbilityIndex { get => abilityIndex; set => abilityIndex = value; }


        public virtual void CastAbility()
        {
        }

        public void ToggleTargeting(bool value)
        {
        }

        public virtual void ShakeCamera()
        {
        }

        public virtual CameraShakeParameters AssignCameraShakeParameterValues(float duration, float magnitude)
        {
            return new CameraShakeParameters();
        }

        //This will be used to call the toggle particles method from animation event.
        //The animation events dont like multiple params so we will just use this one
        //to call the other ones

        public virtual void ToggleParticles(bool networkThisCall)
        {
        }

        public virtual void NetworkedMethodCall(int methodIndex)
        {
        }
    }
}
