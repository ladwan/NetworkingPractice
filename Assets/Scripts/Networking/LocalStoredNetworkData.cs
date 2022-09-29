using ForverFight.Interactable.Abilities;
using UnityEngine.UI;

namespace ForverFight.Networking
{
    public static class LocalStoredNetworkData
    {
        public static string locallyStoredOpponentsName = "";

        public static bool isPlayer1Turn = true;

        public static SelectAbilityToCast localPlayerSelectAbilityToCast = null;

        public static Button localPlayerAttackConfirmButton = null;
    }
}

