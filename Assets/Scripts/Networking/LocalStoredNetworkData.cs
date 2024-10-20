using UnityEngine;
using UnityEngine.UI;
using ForeverFight.GameMechanics.Timers;
using ForeverFight.Interactable.Abilities;
using ForeverFight.Interactable.Characters;
using System.Threading.Tasks;
using System;
using System.Collections;

namespace ForeverFight.Networking
{
    public static class LocalStoredNetworkData
    {
        public static string locallyStoredOpponentsName = "";

        public static bool isPlayer1Turn = true;

        public static bool damageableObjectDetected = false;

        public static SelectAbilityToCast localPlayerSelectAbilityToCast = null;

        public static Slider localPlayerHealthSlider = null;

        public static Slider opponentHealthSlider = null;

        public static Character localPlayerCharacter = null;

        public static Character opponentCharacter = null;

        public static CharacterAnimationReferences localPlayerCharacterAnimationReferences = null;

        public static Button localPlayerAttackConfirmButton = null;

        public static int localPlayerCurrentAP = 3;

        public static int opponentsCurrentAP = 3;

        public static Countdown countdownTimerScript = null;

        public static int squaresMovedThisInstanceOfMovement = 0; //This is to keep track of how many sq's a player has just moved, in one instance of movement. It should be reset with every new instance of movement and is NOT cumulative for the turn.


        public static IEnumerator WaitForCharacterAnimationReferences(Action<CharacterAnimationReferences> callback)
        {
            yield return new WaitUntil(() => localPlayerCharacterAnimationReferences is not null);

            if (callback is not null)
            {
                callback?.Invoke(localPlayerCharacterAnimationReferences);
            }
            else
            {
                Debug.LogWarning("Callback was null!");
            }
        }

        public static Character GetLocalCharacter()
        {
            if (localPlayerCharacter)
            {
                if (localPlayerCharacterAnimationReferences is null)
                {
                    localPlayerCharacterAnimationReferences = localPlayerCharacter.CharacterAnimationReferences;
                }

                return localPlayerCharacter;
            }
            else
            {
                Debug.LogWarning("No Local player could be found");
                return null;
            }
        }

        public static CharacterAnimationReferences GetLocalCharacterAnimationReferences()
        {
            if (localPlayerCharacterAnimationReferences)
            {
                return localPlayerCharacterAnimationReferences;
            }
            else
            {
                Debug.LogWarning("No localPlayerCharacterAnimationReferences could be found, searching again in a momement..");
                return null;
            }
        }

        public static Character GetOpponentCharacter()
        {
            if (opponentCharacter)
            {
                return opponentCharacter;
            }
            else
            {
                Debug.LogWarning("No Opponent player could be found");
                return null;
            }
        }

        public static Slider GetLocalHealthSlider()
        {
            if (localPlayerHealthSlider)
            {
                return localPlayerHealthSlider;
            }
            else
            {
                Debug.LogWarning("No local player health slider could be found");
                return null;
            }
        }

        public static Slider GetOpponentHealthSlider()
        {
            if (opponentHealthSlider)
            {
                return opponentHealthSlider;
            }
            else
            {
                Debug.LogWarning("No opponent health slider could be found");
                return null;
            }
        }

        public static Countdown GetCountdownTimerScript()
        {
            if (countdownTimerScript)
            {
                return countdownTimerScript;
            }
            else
            {
                Debug.LogWarning("No Count Down Script could be found");
                return null;
            }
        }
    }
}

