using ForeverFight.Interactable;
using ForeverFight.Interactable.Abilities;
using UnityEngine;
using UnityEngine.UI;

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

        public static Button localPlayerAttackConfirmButton = null;

        public static int localPlayerCurrentAP = 3;

        public static int opponentsCurrentAP = 3;



        public static Character GetLocalCharacter()
        {
            if (localPlayerCharacter)
            {
                return localPlayerCharacter;
            }
            else
            {
                Debug.Log("No Local player could be found");
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
                Debug.Log("No Opponent player could be found");
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
                Debug.Log("No local player health slider could be found");
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
                Debug.Log("No opponent health slider could be found");
                return null;
            }
        }
    }
}

