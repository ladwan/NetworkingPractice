using System;
using System.Collections;
using UnityEngine;
using ForeverFight.Ui;
using ForeverFight.Networking;
using ForeverFight.HelperScripts;
using ForeverFight.GameMechanics.Timers;
using ForeverFight.GameMechanics.Movement;
using ForeverFight.GameMechanics.DiceRoll;
using ForeverFight.Interactable.Characters;
using ForeverFight.Interactable.PlayerInputInteractions;
using TMPro;

namespace ForeverFight.FlowControl
{
    public class PlayerTurnManager : MonoBehaviour
    {
        //[SerializeField]
        //private GameObject playerUi = null;
        [SerializeField]
        private RollDice rollDiceREF = null;
        [SerializeField]
        private Countdown playerTimer = null;
        [SerializeField]
        private TMP_Text playerTimerSubtext = null;
        [SerializeField]
        private Action onTurnEnd = null;
        private Action<bool> isLocalPlayersTurnAction = null;


        [NonSerialized]
        private static PlayerTurnManager instance = null;
        [NonSerialized]
        private bool isLocalPlayersTurn = true; //This will be true for player 2 on the when the game FIRST starts, it should set itself to false in BeginMatch()
        private Action onTurnStart = null;
        [NonSerialized]
        private Animator localCharacterAnimator = null;
        [NonSerialized] private int turnsUntilOverdrive = 5;
        [NonSerialized] private int overdriveAp = -1;
        [NonSerialized] private bool doOnce = false;


        public static PlayerTurnManager Instance { get => instance; set => instance = value; }

        public Action OnTurnEnd { get => onTurnEnd; set => onTurnEnd = value; }

        public Action OnTurnStart { get => onTurnStart; set => onTurnStart = value; }

        public bool IsLocalPlayersTurn
        {
            get { return isLocalPlayersTurn; }
            set
            {
                isLocalPlayersTurn = value;
                isLocalPlayersTurnAction?.Invoke(value);
            }
        }

        public Action<bool> IsLocalPlayersTurnAction { get => isLocalPlayersTurnAction; set => isLocalPlayersTurnAction = value; }
        public int TurnsUntilOverdrive => turnsUntilOverdrive;
        public int OverdriveAp  => overdriveAp;


        protected void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance);
                Debug.Log("Instance of Player Turn Manager already exsists! Destroyed new instance");
            }
        }

        protected void Start()
        {
            StartCoroutine(LocalStoredNetworkData.WaitForCharacterAnimationReferences(SetCharacterAnimatorReferences));
        }


        public void StartTurn()
        {
            //playerUi.SetActive(true);
            playerTimer.ResetTimer(playerTimer.MaxTime);
            playerTimerSubtext.text = "( Your Go ! )";
            IsLocalPlayersTurn = true;

            if (!SafetyNet.IsValid(rollDiceREF, "Dice Reference on turn START"))
            {
                return;
            }

            if (rollDiceREF.SixSidedDieAnimator.GetCurrentAnimatorStateInfo(0).IsName("Despawn"))
            {
                rollDiceREF.SixSidedDieAnimator.SetTrigger("ResetDie");
            }

            if (turnsUntilOverdrive < 0)
            {
                overdriveAp = Mathf.Abs(turnsUntilOverdrive);

                ActionPointsManager.Instance.UpdateAP(ActionPointsManager.Instance.MainApLists, overdriveAp);
            }

            onTurnStart?.Invoke();
            return;
        }

        //Called on both players at the same time by the server, once both have loaded in. Nothing (timer, die, turns) starts before this
        public void BeginMatch()
        {
            if (ClientInfo.playerNumber == 1)
            {
                StartTurn();
                return;
            }

            //Player 2 starts on the opponent's turn. Player 1 already knows it's their go, so nothing is sent
            EndTurnLocally(false);
            transform.gameObject.AddComponent<BasePlayerLookAt>();
        }

        public void EndTurn(bool timeRanOut)
        {
            if (isLocalPlayersTurn)
            {
                EndTurnLocally(timeRanOut);
                ClientSend.EndTurn();
            }
        }

        //Everything ending a turn does on this client, without telling the opponent it's their go
        private void EndTurnLocally(bool timeRanOut)
        {
            if (isLocalPlayersTurn)
            {
                CleanUpUiAfterTurn.Instance.CleanUpUi();
                //playerUi.SetActive(false);
                playerTimer.ResetTimer(playerTimer.MaxTime);
                playerTimerSubtext.text = "( Opponents turn... )";
                IsLocalPlayersTurn = false;

                if (!localCharacterAnimator.GetCurrentAnimatorStateInfo(1).IsName("Camera - Idle"))
                {
                    localCharacterAnimator.SetTrigger("Camera - Go to Idle");
                }

                if (!rollDiceREF.SixSidedDieAnimator.GetCurrentAnimatorStateInfo(0).IsName("Despawn"))
                {
                    rollDiceREF.SixSidedDieAnimator.Play("Despawn", 0);
                }


                if (timeRanOut)
                {
                    ActionPointsManager.Instance.PlayerTurnHasEnded = true;
                    ActionPointsManager.Instance.MoveWasCanceled(); // Also refunds + clears the movement plan
                    BasePlayerInputInteraction.Instance.ForceEndDrag();
                }
                else
                {
                    ActionPointsManager.Instance.PlayerTurnHasEnded = true;
                    ActionPointsManager.Instance.UpdateAP(ActionPointsManager.Instance.MainApLists, 0);
                    MovementPlanner.Instance.CancelPlan();
                }

                if (LocalStoredNetworkData.localPlayerSelectAbilityToCast)
                {
                    // LocalStoredNetworkData.localPlayerSelectAbilityToCast.ToggleAbilityRadius(false);
                }

                UpdateTurnsUntilOverdrive();

                onTurnEnd?.Invoke();
                IsLocalPlayersTurn = false;
                return;
            }
        }

        private void SetCharacterAnimatorReferences(CharacterAnimationReferences animationReferences)
        {
            localCharacterAnimator = animationReferences.CharacterAnimator;

            //Players load in at different speeds, so the server waits for both of these before sending BeginMatch to both
            playerTimerSubtext.text = "( Waiting for opponent... )";
            ClientSend.ReadyToStartMatch();
        }

        private void UpdateTurnsUntilOverdrive()
        {
            if (ClientInfo.playerNumber == 2 && doOnce == false)
            {
                doOnce = true;
                return;
            }

            turnsUntilOverdrive--;

            if (turnsUntilOverdrive < -9)
            {
                turnsUntilOverdrive = -9;
            }

            if (turnsUntilOverdrive == 0)
            {
                turnsUntilOverdrive = -1;
            }


            if (turnsUntilOverdrive < 0)
            {
                overdriveAp = Mathf.Abs(turnsUntilOverdrive);
            }
        }
    }
}
