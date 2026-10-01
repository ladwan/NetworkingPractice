using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ForeverFight.Networking;
using ForeverFight.Interactable.Abilities;
using System;
using ForeverFight.Interactable.PlayerInputInteractions;
using ForeverFight.GameMechanics.Movement;

namespace ForeverFight.Interactable.Characters
{
    public abstract class Character : MonoBehaviour
    {
        [SerializeField] private Identity charIdentity = Identity.NoIdentity;
        [SerializeField] private GameObject characterModel = null;
        [SerializeField] private Image characterIcon = null;
        [SerializeField] private string characterName = null;
        [SerializeField] private int health = 0;
        [SerializeField] private int rollAlotment = 0;
        [SerializeField] private int abilityNumber = 0;
        [SerializeField] private float baseMoveSpeed = 1.0f;
        [SerializeField] private float moveSpeed = 1.0f;
        [SerializeField] private float moveSpeedHelper = 1.0f;
        [SerializeField] private CharacterAnimationReferences characterAnimationReferences = null;
        [SerializeField] private CharStance currentStance = null;
        [SerializeField] private List<CharStance>  stances = new List<CharStance>();
        [SerializeField] private List<CharAbility> moveset = new List<CharAbility>();
        [SerializeField] private List<GameObject> customUiElements = new List<GameObject>();
        [SerializeField] private GameObject oneSqRadius = null;
        [SerializeField] private GameObject twoSqRadius = null;
        [SerializeField] private GameObject threeSqRadius = null;
        [SerializeField] private GameObject fourSqRadius = null;
        [SerializeField] private GameObject fiveSqRadius = null;
        // One set of curves per movement state (0 = base, 1 = Ire, ...), picked with MovementIndex.
        // Each curve drives both the CharSpeed blend value and how fast the character moves.
        [SerializeField] private List<MovementAnimationCurves> movementAnimCurves = null;
        // Moves that play their own animation (e.g. the Brawn's Ire leap) instead of plain locomotion.
        [SerializeField] private List<SpecialMovement> specialMovements = new List<SpecialMovement>();
        // TODO: dead since MovePlaybackPlan - pacing comes from movementAnimCurves.
        [SerializeField]
        private AnimationCurve runSpeedCurve = new AnimationCurve(
            new Keyframe(0f, 0.5f), new Keyframe(0.2f, 1f), new Keyframe(0.8f, 1f), new Keyframe(1f, 0.5f));
        private int movementIndex = 0;


        [Serializable]
       public class MovementAnimationCurves
        {
            [SerializeField]
            private string name;
            [SerializeField]
            public List<AnimationCurve> movementCurves = null;
        }

        /// <summary>
        /// A move that fires its own animation. Two separate parts, use either or both:
        ///   - Wind up: the character stands still for windUpSeconds before translating.
        ///   - Travel: how the translation plays after the wind up. MovementCurve walks/runs
        ///     with the normal movement curves; FixedDuration covers the whole path in
        ///     travelSeconds (e.g. a leap that lands at the end of the move).
        /// MovementExecutor picks it from the movement state and path length, so both clients
        /// play it the same way from the same move packet.
        /// </summary>
        [Serializable]
        public class SpecialMovement
        {
            public enum TravelType
            {
                MovementCurve,
                FixedDuration,
            }

            [SerializeField]
            private string name;
            [Tooltip("Movement state this applies to (0 = base, 1 = Ire, ...).")]
            public int movementIndex = 0;
            [Tooltip("Moves this many units or shorter use this special movement.")]
            public float maxDistance = 3f;
            [Tooltip("Animator trigger that starts the special animation.")]
            public string animatorTrigger = null;

            [Header("Wind Up")]
            [Tooltip("Seconds the character stands still while the animation winds up, before it starts translating. 0 = no wind up.")]
            public float windUpSeconds = 0.5f;

            [Header("Travel")]
            public TravelType travelType = TravelType.MovementCurve;
            [Tooltip("FixedDuration only: seconds to cover the whole path, e.g. the leap's time in the air.")]
            public float travelSeconds = 0.5f;
            [Tooltip("FixedDuration only: how much of the path is covered (0..1) over the travel time (0..1).")]
            public AnimationCurve travelProgress = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        }




        public Identity CharIdentity { get => charIdentity; set => charIdentity = value; }

        public GameObject CharacterModel { get => characterModel; set => characterModel = value; }

        public Image CharacterIcon { get => characterIcon; set => characterIcon = value; }

        public string CharacterName { get => characterName; set => characterName = value; }

        public int Health { get => health; set => health = value; }

        public int RollAlotment { get => rollAlotment; set => rollAlotment = value; }

        public int AbilityNumber { get => abilityNumber; set => abilityNumber = value; }

        public float BaseMoveSpeed => baseMoveSpeed;

        public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

        public CharacterAnimationReferences CharacterAnimationReferences { get => characterAnimationReferences; set => characterAnimationReferences = value; }

        public List<CharAbility> Moveset => moveset;

        public List<GameObject> CustomUiElements => customUiElements;

        public GameObject OneSqRadius { get => oneSqRadius; set => oneSqRadius = value; }

        public GameObject TwoSqRadius { get => twoSqRadius; set => twoSqRadius = value; }

        public GameObject ThreeSqRadius { get => threeSqRadius; set => threeSqRadius = value; }

        public GameObject FourSqRadius { get => fourSqRadius; set => fourSqRadius = value; }

        public GameObject FiveSqRadius { get => fiveSqRadius; set => fiveSqRadius = value; }

        public int MovementIndex { get => movementIndex; set => movementIndex = value; }

        public AnimationCurve RunSpeedCurve => runSpeedCurve;

        public List<MovementAnimationCurves> MovementAnimCurves  => movementAnimCurves;

        /// <summary>Picks the movement curve for a move of this length, the same way the old grid did.</summary>
        // The old grid used element 0 for a 1 cell move, element 1 for 2 cells, and so on.
        // NavMesh paths aren't whole cells, so the length is rounded (1 world unit ~= 1 old grid cell),
        // and anything longer than the last curve uses the last curve.
        // An unknown movement state falls back to the base curves instead of erroring mid move.
        public AnimationCurve GetMovementCurve(int stateIndex, float pathLength)
        {
            if (movementAnimCurves == null || movementAnimCurves.Count == 0)
            {
                return null;
            }

            if (stateIndex < 0 || stateIndex >= movementAnimCurves.Count)
            {
                Debug.LogWarning($"{CharacterName} has no movement curves for state {stateIndex}, using the base curves");
                stateIndex = 0;
            }

            var curves = movementAnimCurves[stateIndex].movementCurves;
            if (curves == null || curves.Count == 0)
            {
                return null;
            }

            int curveIndex = Mathf.Clamp(Mathf.RoundToInt(pathLength) - 1, 0, curves.Count - 1);
            return curves[curveIndex];
        }

        /// <summary>The special movement for this movement state and move length, or null for a normal move.</summary>
        // The length is rounded the same way GetMovementCurve picks a curve, so a move that plays
        // the "3 cell" curve (2.5 up to 3.5 units) counts as 3 and still gets a max distance 3 move.
        public SpecialMovement GetSpecialMovement(int stateIndex, float pathLength)
        {
            int roundedLength = Mathf.RoundToInt(pathLength);
            for (int i = 0; i < specialMovements.Count; i++)
            {
                var special = specialMovements[i];
                if (special.movementIndex == stateIndex && roundedLength <= special.maxDistance)
                {
                    return special;
                }
            }

            return null;
        }

        public float MoveSpeedHelper { get => moveSpeedHelper; set => moveSpeedHelper = value; }
        
        public CharStance CurrentStance
        {
            get => currentStance;
            set => currentStance = value;
        }
        
        public List<CharStance> Stances => stances;

        protected void AssignDefaultStance()
        {
            currentStance = stances[0];
        }
        
        public enum Identity
        {
            NoIdentity,
            Brawn,
            Speedster,
            Elemental,
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

        public void ParentCustomUi(GameObject parent)
        {
            if (customUiElements.Count == 0) return;

            foreach (var uiElement in customUiElements) 
            {
                uiElement.transform.SetParent(parent.transform, false);
                uiElement.transform.position = Vector3.zero;
                uiElement.transform.rotation = Quaternion.identity;

                if (uiElement.TryGetComponent<RectTransform>(out var t))
                {
                    // Reset stretch offsets (this is the "position" for stretched UI)
                    t.offsetMin = Vector2.zero;
                    t.offsetMax = Vector2.zero;

                    // Reset rotation only
                    t.localRotation = Quaternion.identity;

                    // Optional safety (usually already zero, but ensures no drift)
                    t.anchoredPosition = Vector2.zero;
                }
            }
        }
    }
}

