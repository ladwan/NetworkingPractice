using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ForeverFight.Ui;
using ForeverFight.Networking;
using ForeverFight.FlowControl;
using ForeverFight.Interactable.Abilities;
using ForeverFight.Interactable.Characters;

namespace ForeverFight.GameMechanics.Movement
{
    /// <summary>
    /// Base for any passive that gives a character extra movement-only AP each turn.
    /// Holds the pool, the per-turn reset, the passive light bar backgrounds and the
    /// registration with ActionPointsManager - everything that used to live in the
    /// Speedster's FasterPassive. A new character only needs a subclass that says who
    /// owns it and what the passive is called, plus an ApReferenceLists (display type
    /// movementPassive) for its light bar. ApDistanceBank and the guide line already
    /// work off IMovementPassiveAp, so they pick it up with no changes.
    /// </summary>
    public abstract class MovementPassiveAp : MonoBehaviour, IPassiveAbility, IMovementPassiveAp
    {
        [SerializeField]
        private ApReferenceLists referenceLists = null;
        [SerializeField]
        private string passiveAbilityName = null;
        [SerializeField]
        private string passiveAbilityDescription = null;
        [SerializeField]
        private List<Image> passiveApBackgrounds = new List<Image>();
        [SerializeField]
        private Color passiveHightlightColor = Color.black;
        [SerializeField]
        private int startingPassiveAp = 3;


        // Each passive AP is one movement bucket (ApDistanceBank.UnitsPerAp world units),
        // consumed before the main pool.
        private int passiveAp = 3;
        private int maxPassiveAp = 3;
        private Coroutine coroutineREF = null;


        #region Public Properties
        public int PassiveAp
        {
            get => passiveAp;
            set
            {
                if (value >= 0)
                {
                    passiveAp = value;
                }
                else
                {
                    passiveAp = 0;
                    Debug.Log("Invalid passive AP amount");
                }
            }
        }

        public string PassiveAbilityName
        {
            get => passiveAbilityName;
            set
            {
                if (value != "" && value != null)
                {
                    passiveAbilityName = value;
                }
                else
                {
                    Debug.Log("Invalid passive ability name");
                }
            }
        }
        public string PassiveAbilityDescription
        {
            get => passiveAbilityDescription;
            set
            {
                if (value != "" && value != null)
                {
                    passiveAbilityDescription = value;
                }
                else
                {
                    Debug.Log("Invalid passive ability description");
                }
            }
        }

        public List<Image> PassiveApBackgrounds => passiveApBackgrounds;

        public Color PassiveHightlightColor { get => passiveHightlightColor; set => passiveHightlightColor = value; }

        public int MaxPassiveAp { get => maxPassiveAp; set => maxPassiveAp = value; }

        /// <summary>The distinct light bar this pool renders on (IMovementPassiveAp).</summary>
        public ApReferenceLists PassiveApLists => referenceLists;

        #endregion


        #region Subclass Settings
        /// <summary>The character this passive belongs to. Used when it can't find its owning Character in its parents.</summary>
        protected abstract Character.Identity OwnerIdentity { get; }

        protected abstract string DefaultPassiveName { get; }

        protected abstract string DefaultPassiveDescription { get; }

        #endregion


        protected virtual void Awake()
        {
            passiveAp = startingPassiveAp;
            maxPassiveAp = startingPassiveAp;
        }

        protected virtual void OnEnable()
        {
            BeginCoroutine();
            PassiveAbilityName = DefaultPassiveName;
            PassiveAbilityDescription = DefaultPassiveDescription;
            CombatUiStatesManager.Instance.OnCombatUiStateChange += ApplyPassive;
            PlayerTurnManager.Instance.OnTurnEnd += ResetPassiveAp;
        }

        protected virtual void OnDisable()
        {
            CombatUiStatesManager.Instance.OnCombatUiStateChange -= ApplyPassive;
            PlayerTurnManager.Instance.OnTurnEnd -= ResetPassiveAp;

            if (ActionPointsManager.Instance != null
                && ReferenceEquals(ActionPointsManager.Instance.MovementPassiveApProvider, this))
            {
                ActionPointsManager.Instance.MovementPassiveApProvider = null;
            }
        }


        public void ApplyPassive()
        {
            if (CombatUiStatesManager.Instance.CurrentCombatUiState == CombatUiStatesManager.CombatUiState.movement)
            {
                if (passiveAp > 0)
                {
                    ActionPointsManager.Instance.UpdateAP(referenceLists, 0);
                    UpdateApBackgrounds();
                }
                else
                {
                    ActionPointsManager.Instance.UpdateAP(ActionPointsManager.Instance.MainApLists, 0);
                }
            }
            else
            {
                ToggleApBackgrounds(false);
            }
        }

        public void ToggleApBackgrounds(bool toggle)
        {
            for (int i = 0; i < PassiveApBackgrounds.Count; i++)
            {
                PassiveApBackgrounds[i].enabled = toggle;
            }
        }

        public void UpdateApBackgrounds()
        {
            ToggleApBackgrounds(false);
            for (int i = 0; i < passiveAp && i < PassiveApBackgrounds.Count; i++)
            {
                PassiveApBackgrounds[i].enabled = true;
            }
        }

        public void UpdateMaxApValue(int value)
        {
            maxPassiveAp = value;
        }

        public void SetMaxPassiveApPool(int value)
        {
            var spentAp = maxPassiveAp - passiveAp;
            maxPassiveAp = value;
            var difference = (maxPassiveAp - passiveAp) - spentAp;
            ActionPointsManager.Instance.UpdateAP(referenceLists, difference);
        }


        private void ResetPassiveAp()
        {
            passiveAp = maxPassiveAp;
        }

        private void BeginCoroutine()
        {
            EndCoroutine();
            coroutineREF = StartCoroutine(OnEnableEnum());
        }

        private void EndCoroutine()
        {
            if (coroutineREF != null)
            {
                StopCoroutine(coroutineREF);
                coroutineREF = null;
            }
        }

        private IEnumerator OnEnableEnum()
        {
            yield return new WaitUntil(() => LocalStoredNetworkData.GetLocalCharacter());

            if (!BelongsToLocalCharacter(LocalStoredNetworkData.GetLocalCharacter()))
            {
                this.enabled = false;
                yield break;
            }

            // Only the confirmed owner registers as the movement passive AP provider.
            ActionPointsManager.Instance.MovementPassiveApProvider = this;
        }

        // Checking the owning Character instance (not just its identity) keeps the opponent's
        // copy from registering when both players pick the same character.
        private bool BelongsToLocalCharacter(Character localCharacter)
        {
            var owner = GetComponentInParent<Character>();
            if (owner != null)
            {
                return owner == localCharacter;
            }

            return localCharacter.CharIdentity == OwnerIdentity;
        }
    }
}
