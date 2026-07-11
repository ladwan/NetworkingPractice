using UnityEngine;
using UnityEngine.UI;
using ForeverFight.GameMechanics.Timers;
using ForeverFight.Interactable.Abilities;
using ForeverFight.Interactable.Characters;
using System.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;

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
                Debug.LogError("Callback was null!");
            }
        }

        private static readonly Dictionary<string, Task> NullCheckTimeoutTasks = new Dictionary<string, Task>();

        private static int nullCheckResetGeneration;

        public static Character GetLocalCharacter()
        {
            if (localPlayerCharacter is not null)
            {
                if (localPlayerCharacterAnimationReferences is null)
                {
                    localPlayerCharacterAnimationReferences = localPlayerCharacter.CharacterAnimationReferences;
                }

                return localPlayerCharacter;
            }

            StartNullCheckTimeout(() => localPlayerCharacter is null, "No Local player could be found");
            return null;
        }

        private static void StartNullCheckTimeout(Func<bool> isStillNull, string errorMessage)
        {
            if (NullCheckTimeoutTasks.TryGetValue(errorMessage, out var runningTask) && !runningTask.IsCompleted)
            {
                return;
            }

            NullCheckTimeoutTasks[errorMessage] = LogErrorIfStillNull(isStillNull, errorMessage);
        }

        private static async Task LogErrorIfStillNull(Func<bool> isStillNull, string errorMessage)
        {
            var generationWhenStarted = nullCheckResetGeneration;

            await Task.Delay(2000);

            if (generationWhenStarted == nullCheckResetGeneration && isStillNull())
            {
                Debug.LogError(errorMessage);
            }
        }

        public static CharacterAnimationReferences GetLocalCharacterAnimationReferences()
        {
            if (localPlayerCharacterAnimationReferences)
            {
                return localPlayerCharacterAnimationReferences;
            }

            StartNullCheckTimeout(() => !localPlayerCharacterAnimationReferences, "No localPlayerCharacterAnimationReferences could be found");
            return null;
        }

        public static Character GetOpponentCharacter()
        {
            if (opponentCharacter)
            {
                return opponentCharacter;
            }

            StartNullCheckTimeout(() => !opponentCharacter, "No Opponent player could be found");
            return null;
        }

        public static Slider GetLocalHealthSlider()
        {
            if (localPlayerHealthSlider)
            {
                return localPlayerHealthSlider;
            }

            StartNullCheckTimeout(() => !localPlayerHealthSlider, "No local player health slider could be found");
            return null;
        }

        public static Slider GetOpponentHealthSlider()
        {
            if (opponentHealthSlider)
            {
                return opponentHealthSlider;
            }

            StartNullCheckTimeout(() => !opponentHealthSlider, "No opponent health slider could be found");
            return null;
        }

        public static Countdown GetCountdownTimerScript()
        {
            if (countdownTimerScript)
            {
                return countdownTimerScript;
            }

            StartNullCheckTimeout(() => !countdownTimerScript, "No Count Down Script could be found");
            return null;
        }

        public static void Reset()
        {
            locallyStoredOpponentsName = "";

            isPlayer1Turn = true;
            damageableObjectDetected = false;

            localPlayerSelectAbilityToCast = null;

            localPlayerHealthSlider = null;
            opponentHealthSlider = null;

            localPlayerCharacter = null;
            opponentCharacter = null;

            localPlayerCharacterAnimationReferences = null;

            localPlayerAttackConfirmButton = null;

            localPlayerCurrentAP = 3;
            opponentsCurrentAP = 3;

            countdownTimerScript = null;

            squaresMovedThisInstanceOfMovement = 0;

            nullCheckResetGeneration++;
            NullCheckTimeoutTasks.Clear();

            ClientInfo.Reset();
        }
    }
}

