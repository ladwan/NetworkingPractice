using System;
using System.Collections;
using System.Collections.Generic;
using ForeverFight.GameMechanics;
using ForeverFight.HelperScripts;
using ForeverFight.Interactable.Characters;
using ForeverFight.Networking;
using UnityEngine;

namespace ForeverFight.Interactable.Abilities
{
    public abstract class CharAbility : MonoBehaviour
    {
        [SerializeField] private Character owningCharacter = null;
        [SerializeField] private string abilityName = "";
        [SerializeField] private string abilityDescription = "";
        [SerializeField] private int abilityDamage = 0;
        [SerializeField] private int abilityCost = 0;
        [SerializeField] private GameObject abilityRadius = null;
        [SerializeField] private int abilityIndex = -1;
        [SerializeField] protected Animator animREF;


        protected CameraShakeParameters currentCameraShakeParameters;
        protected List<CameraShakeParameters> shakeParameters = new();
        protected List<Vector2> shakeParametersSettings;


        [SerializeField] private GameObject particleGameObject = null;
        [SerializeField] private MonoBehaviour particleScript = null;
        [SerializeField] protected ShaderManager vfx = null;


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

        public virtual void HandleAbilityResponses(Character characterREF)
        {
        }


        private CameraShakeParameters AssignCameraShakeParameterValues(float duration, float magnitude)
        {
            CameraShakeParameters parameters = new CameraShakeParameters();

            parameters.duration = duration;
            parameters.magnitude = magnitude;

            return parameters;
        }

        protected List<CameraShakeParameters> ReturnParamsBasedOnSettings(List<Vector2> settings)
        {
            var tempList = new List<CameraShakeParameters>();
            for (int i = 0; i < settings.Count; i++)
            {
                var param = AssignCameraShakeParameterValues(settings[i].x, settings[i].y);
                tempList.Add(param);
            }

            return tempList;
        }

        protected bool LocalCharacterIsCallingAbilityResponse(CameraShakeParameters currentCameraShakeParameters, Character characterREF)
        {
            CameraScreenShakeManager.Instance.StartShake(currentCameraShakeParameters);
            ExecuteMethodAfterDelay.Instance.WaitUntilTrue = true;

            if (characterREF != LocalStoredNetworkData.GetLocalCharacter())
            {
                return false;
            }

            return true;
        }

        public Animator AttemptAbility(Animator anim)
        {
            if (anim == null)
            {
                anim = LocalStoredNetworkData.GetLocalCharacter().CharacterAnimationReferences.CharacterAnimator;
                if (anim == null)
                {
                    Debug.LogError("Anim REF was NULL !");
                    return null;
                }

                return anim;
            }

            return anim;
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
