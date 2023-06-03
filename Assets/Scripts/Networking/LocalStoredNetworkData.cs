using ForverFight.Interactable;
using ForverFight.Interactable.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace ForverFight.Networking
{
    public static class LocalStoredNetworkData
    {
        public static string locallyStoredOpponentsName = "";

        public static bool isPlayer1Turn = true;

        public static SelectAbilityToCast localPlayerSelectAbilityToCast = null;

        public static Slider opponentHealthSlider = null;

        public static Character localPlayerCharacter = null;

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
                Debug.Log("No player could be found");
                return null;
            }
        }
    }
}

