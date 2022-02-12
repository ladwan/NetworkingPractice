using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForverFight.Ui.CharacterSelection
{
    public class AbilityDescripton : MonoBehaviour
    {
        [SerializeField]
        private CharacterInfo charInfo;

        public void PopulateInfoDisplay()
        {
            switch (charInfo.CharName)
            {
                case "The Speedster":
                    charInfo.AbilityDescriptionText = $"1-3) {charInfo.CharName} dashes toward his opponent in a flash and delivers a stiff jab before returning to his original location \n \n" +
                        "4-5) The Speedster decides to pick up the pace a little bit making him much harder to hit for the next 3 turns \n \n" +
                        "6) The Speedster spins at an incredible pace creating a tornado that sucks his oponent in and does damage";
                    break;

                case "The Brawn":
                    charInfo.AbilityDescriptionText = $"1-3) {charInfo.CharName} delivers a heavy blow to his oponent \n \n" +
                        $"4-5) {charInfo.CharName} steels his nerves making him immune to all status effects for the next turn \n \n" +
                        $"6) {charInfo.CharName} Jumps into the air and comes crashing down with devestating force causing damage within a large radius";
                    break;
            }
        }
    }

}