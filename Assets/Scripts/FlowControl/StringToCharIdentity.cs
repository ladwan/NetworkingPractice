using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Interactable.Characters;

namespace FlowControl
{
    public static class StringToCharIdentity
    {
        public static Character.Identity IdentifyOponent(string charName)
        {
            switch (charName)
            {
                case "No Identity":
                    return Character.Identity.NoIdentity;

                case "The Speedster":
                    return Character.Identity.Speedster;

                case "The Brawn":
                    return Character.Identity.Brawn;

                default:
                    return Character.Identity.NoIdentity;
            }
        }
    }
}
