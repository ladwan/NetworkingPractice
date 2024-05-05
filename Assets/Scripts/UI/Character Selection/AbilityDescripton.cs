using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.Ui.CharacterSelection
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
                    charInfo.AbilityDescriptionText = $"Faster - Passive) Each turn {charInfo.CharName} gains 3 extra AP that can only be used for movement \n \n" +
                        $"Quick Punch - AP Cost: 2) {charInfo.CharName} dashes toward his opponent in a flash and delivers a stiff jab before returning to his original location \n \n" +
                        $"Momentum - AP Cost: 4) For the next 3 turns {charInfo.CharName} stores up momentum based on the number of sq's moved. This stored momentum will increase the damage of Quick Punch \n \n" +
                        $"Haste - AP Cost: 7) Doubles the number of AP gained from {charInfo.CharName}'s passive. Increases the attack radius of Quick Punch";
                    break;

                case "The Brawn":
                    charInfo.AbilityDescriptionText = $"Hard Knock Life - Passive) {charInfo.CharName} has 150 health points. Dealing damage will build 'Off-Balanced' stacks on enemy at 9 stacks enemy will be stunned for a turn \n \n" +
                        $"Haymaker - AP Cost: 3) {charInfo.CharName} delivers a powerful blow to his opponent (Ire: Now deals more damage, knocks the target back 3 sqs)\n \n" +
                        $"Ground Pound - AP Cost: 2) {charInfo.CharName} smashes the ground near him. Enemies in 1 sq radius are dealt damage, enemies in 2 sq radius are pulled in 1 sq (Ire: Increases attack radius. Enemies are damaged if hit. Enemies not in 1 sq radius are pulled to a 1 sq radius)\n \n" +
                        $"Ire - AP Cost: 4) Empowers {charInfo.CharName} for 3 turns boosting the capibility of his other abilities. ";
                    break;
            }
        }
    }

}