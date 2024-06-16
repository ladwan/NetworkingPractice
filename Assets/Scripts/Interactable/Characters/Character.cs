using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ForeverFight.Networking;
using ForeverFight.Interactable.Abilities;


namespace ForeverFight.Interactable
{
    public abstract class Character : MonoBehaviour
    {
        [SerializeField]
        private Identity charIdentity = Identity.NoIdentity;
        [SerializeField]
        private GameObject characterModel = null;
        [SerializeField]
        private Image characterIcon = null;
        [SerializeField]
        private string characterName = null;
        [SerializeField]
        private int health = 0;
        [SerializeField]
        private int rollAlotment = 0;
        [SerializeField]
        private int abilityNumber = 0;
        [SerializeField]
        private Animator characterAnimator = null;
        [SerializeField]
        private List<CharAbility> moveset = new List<CharAbility>();
        [SerializeField]
        private List<GameObject> customUiElements = new List<GameObject>();
        [SerializeField]
        private GameObject oneSqRadius = null;
        [SerializeField]
        private GameObject twoSqRadius = null;
        [SerializeField]
        private GameObject threeSqRadius = null;
        [SerializeField]
        private GameObject fourSqRadius = null;
        [SerializeField]
        private GameObject fiveSqRadius = null;


        public Identity CharIdentity { get => charIdentity; set => charIdentity = value; }

        public GameObject CharacterModel { get => characterModel; set => characterModel = value; }

        public Image CharacterIcon { get => characterIcon; set => characterIcon = value; }

        public string CharacterName { get => characterName; set => characterName = value; }

        public int Health { get => health; set => health = value; }

        public int RollAlotment { get => rollAlotment; set => rollAlotment = value; }

        public int AbilityNumber { get => abilityNumber; set => abilityNumber = value; }

        public Animator CharacterAnimator { get => characterAnimator; set => characterAnimator = value; }

        public List<CharAbility> Moveset => moveset;

        public List<GameObject> CustomUiElements => customUiElements;

        public GameObject OneSqRadius { get => oneSqRadius; set => oneSqRadius = value; }

        public GameObject TwoSqRadius { get => twoSqRadius; set => twoSqRadius = value; }

        public GameObject ThreeSqRadius { get => threeSqRadius; set => threeSqRadius = value; }

        public GameObject FourSqRadius { get => fourSqRadius; set => fourSqRadius = value; }

        public GameObject FiveSqRadius { get => fiveSqRadius; set => fiveSqRadius = value; }


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


        public void CastAbility(List<CharAbility> moveset, int intToDetermineAbility, Character currentChar)
        {
            switch (intToDetermineAbility)
            {
                case 0:
                    ResolveAbilty(moveset[intToDetermineAbility], currentChar);
                    break;
                case 1:
                    ResolveAbilty(moveset[intToDetermineAbility], currentChar);
                    break;
                case 2:
                    ResolveAbilty(moveset[intToDetermineAbility], currentChar);
                    break;
                default:
                    Debug.Log("Error in casting ability");
                    break;
            }
        }

        public void ResolveAbilty(CharAbility abilty, Character currentPlayer)
        {
            print("Your Character Used " + abilty.AbilityName + " For " + abilty.AbilityDamage + " Damage !");
            LocalStoredNetworkData.opponentHealthSlider.value -= abilty.AbilityDamage;

            //find reference to enemies health 
            //damage enemy based off ability damage
            //network updated enemy health
            //clean ui any targeting highlights or combat ui left over
            // reset to a clean state so this behavior can be looped


            //abilty.AbilityRadius.SetActive(true);
        }
    }
}

