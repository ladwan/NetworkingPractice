using System;
using System.Collections;
using UnityEngine;
using ForeverFight.Ui;
using ForeverFight.Networking;
using ForeverFight.HelperScripts;
using ForeverFight.GameMechanics.Timers;
using ForeverFight.GameMechanics.Movement;
using TMPro;
using ForeverFight.Interactable.Characters;

namespace ForeverFight.FlowControl
{
    public class PlayerTurnManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject playerUi = null;
        [SerializeField]
        private Animator playerDieAnim = null;
        [SerializeField]
        private Countdown playerTimer = null;
        [SerializeField]
        private TMP_Text playerTimerSubtext = null;
        [SerializeField]
        private DragMovement dragMovementREF = null;
        [SerializeField]
        private Action onTurnEnd = null;


        [NonSerialized]
        private static PlayerTurnManager instance = null;
        [NonSerialized]
        private bool isLocalPlayersTurn = true; //This will be true for player 2 on the when the game FIRST starts, it should set itself to false using the EndTurn() method
        private Action onTurnStart = null;
        [NonSerialized]
        private Animator localCharacterAnimator = null;


        public static PlayerTurnManager Instance { get => instance; set => instance = value; }

        public Action OnTurnEnd { get => onTurnEnd; set => onTurnEnd = value; }

        public Action OnTurnStart { get => onTurnStart; set => onTurnStart = value; }


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
            playerUi.SetActive(true);
            playerTimer.ResetTimer(playerTimer.MaxTime);
            playerTimerSubtext.text = "( Your Go ! )";
            isLocalPlayersTurn = true;
            if (playerDieAnim.GetCurrentAnimatorStateInfo(0).IsName("Despawn"))
            {
                playerDieAnim.SetTrigger("ResetDie");
            }

            onTurnStart?.Invoke();
            return;
        }

        public void EndTurn(bool timeRanOut)
        {

            if (isLocalPlayersTurn)
            {
                CleanUpUiAfterTurn.Instance.CleanUpUi();
                playerUi.SetActive(false);
                playerTimer.ResetTimer(playerTimer.MaxTime);
                playerTimerSubtext.text = "( Opponents turn... )";
                isLocalPlayersTurn = false;


                if (!localCharacterAnimator.GetCurrentAnimatorStateInfo(1).IsName("Camera - Idle"))
                {
                    localCharacterAnimator.SetTrigger("Camera - Go to Idle");
                }

                if (!playerDieAnim.GetCurrentAnimatorStateInfo(0).IsName("Despawn"))
                {
                    playerDieAnim.Play("Despawn", 0);
                }

                if (timeRanOut)
                {
                    ActionPointsManager.Instance.PlayerTurnHasEnded = true;
                    ActionPointsManager.Instance.MoveWasCanceled();
                    FloorGrid.Instance.EmptyGridPointList();
                    dragMovementREF.UpdateDragMoverPosition();
                    dragMovementREF.ResetDragMover();
                }
                else
                {
                    ActionPointsManager.Instance.PlayerTurnHasEnded = true;
                    ActionPointsManager.Instance.UpdateAP(ActionPointsManager.Instance.MainApLists, 0);
                    FloorGrid.Instance.EmptyGridPointList();
                }

                if (LocalStoredNetworkData.localPlayerSelectAbilityToCast)
                {
                    // LocalStoredNetworkData.localPlayerSelectAbilityToCast.ToggleAbilityRadius(false);
                }

                onTurnEnd?.Invoke();
                isLocalPlayersTurn = false;
                ClientSend.EndTurn();
                return;
            }
        }

        private void SetCharacterAnimatorReferences(CharacterAnimationReferences animationReferences)
        {
            localCharacterAnimator = animationReferences.CharacterAnimator;
            if (ClientInfo.playerNumber == 2)
            {
                EndTurn(false);
            }
        }
    }
}
