using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.Interactable.Abilities;

namespace ForverFight.Networking
{
    public static class LocalStoredNetworkData
    {
        public static string locallyStoredOpponentsName = "";

        public static bool isPlayer1Turn = true;

        public static SelectAbilityToCast localPlayerSelectAbilityToCast = null;
    }
}

