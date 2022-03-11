using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Interactable
{
    public class Character : MonoBehaviour
    {
        public enum Identity
        {
            NoIdentity,
            Brawn,
            Speedster,
        }

        public struct Abilty
        {
            public string AbilityName;
            public int AbilityDamage;
            public GameObject AbilityRadius;
        }

        public GameObject CharacterModel;
        public Image CharacterIcon;
        public string CharacterName;
        public int Health;
        public int RollAlotment;
        public List<Abilty> Moveset = new List<Abilty>();

        public GameObject oneSqRadius;
        public GameObject twoSqRadius;
        public GameObject threeSqRadius;
        public GameObject fourSqRadius;
        public GameObject fiveSqRadius;


        public void CastAbility(List<Abilty> moveset, int value, Character currentChar)
        {
            switch (value)
            {
                case 1:
                    ResloveAbilty(moveset[0], currentChar);
                    break;
                case 2:
                    ResloveAbilty(moveset[0], currentChar);
                    break;
                case 3:
                    ResloveAbilty(moveset[0], currentChar);
                    break;
                case 4:
                    ResloveAbilty(moveset[1], currentChar);
                    break;
                case 5:
                    ResloveAbilty(moveset[1], currentChar);
                    break;
                case 6:
                    ResloveAbilty(moveset[2], currentChar);
                    break;
            }
        }

        public void ResloveAbilty(Abilty abilty, Character currentPlayer)
        {
            print("Your Character Used " + abilty.AbilityName + " For " + abilty.AbilityDamage + " Damage !");
            var tempArray = FindObjectsOfType<Character>();
            foreach (Character player in tempArray)
            {
                if (player != currentPlayer)
                {

                    print("I found one");
                }
            }
            abilty.AbilityRadius.SetActive(true);
        }
    }
}

