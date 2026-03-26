using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ForeverFight.Ui.CharacterSelection
{
    public class AbilityDescripton : MonoBehaviour
    {
        [SerializeField]
        private CharacterInfo charInfo;
        [SerializeField] private Color abilityDescriptonTitleColor;

        public void PopulateInfoDisplay()
        {
            switch (charInfo.CharName)
            {
                case "The Speedster":
                    charInfo.AbilityDescriptionText = $"{StyledTitle("Faster - Passive)")} Each turn {charInfo.CharName} gains 3 extra AP that can only be used for movement \n \n" +
                        $"{StyledTitle("Quick Punch - AP Cost: 2)")} {charInfo.CharName} delivers a blow to his opponent. The damage for this ability scales based off of your current momentum \n \n" +
                        $"{StyledTitle("Momentum - AP Cost: 4)")} For the next 3 turns {charInfo.CharName} stores up momentum based on the number of sq's moved. This stored momentum will increase the damage of Quick Punch \n \n" +
                        $"{StyledTitle("Haste - AP Cost: 7)")} Doubles the number of AP gained from {charInfo.CharName}'s passive. Increases the attack radius of Quick Punch";
                    break;

                case "The Brawn":
                    charInfo.AbilityDescriptionText = $"{StyledTitle("Hard Knock Life - Passive)")} {charInfo.CharName} has 150 health points. Dealing damage will build 'Off-Balanced' stacks on enemy at 9 stacks enemy will be stunned for a turn. If you knock a player into a wall, they will be automatically stunned \n \n" +
                        $"{StyledTitle("Haymaker - AP Cost: 3)")} {charInfo.CharName} delivers a powerful blow to his opponent (Ire: Now deals more damage, knocks the target back 3 sqs)\n \n" +
                        $"{StyledTitle("Ground Pound - AP Cost: 2)")} {charInfo.CharName} smashes the ground near him. Enemies in 1 sq radius are dealt damage, enemies in 2 sq radius are pulled in 1 sq (Ire: Increases attack radius. Enemies are damaged if hit. Enemies not in 1 sq radius are pulled to a 1 sq radius)\n \n" +
                        $"{StyledTitle("Ire - AP Cost: 4)")} Empowers {charInfo.CharName} for 3 turns boosting the capability of his other abilities. ";
                    break;
            }
        }

        private string StyledTitle(string text)
        {
            string hex = ColorUtility.ToHtmlStringRGB(abilityDescriptonTitleColor);
            return $"<b><color=#{hex}>{text}</color></b>";
        }
    }

}