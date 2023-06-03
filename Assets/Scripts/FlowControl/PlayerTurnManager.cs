using ForverFight.HelperScripts;
using ForverFight.Networking;
using ForverFight.Ui;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace ForverFight.FlowControl
{
    public class PlayerTurnManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject playerUi = null;
        [SerializeField]
        private Animator playerDieAnim = null;
        [SerializeField]
        private Animator playerCameraAnim = null;
        [SerializeField]
        private Countdown playerTimer = null;
        [SerializeField]
        private TMP_Text playerTimerSubtext = null;
        [SerializeField]
        private DragMovement dragMovementREF = null;


        [NonSerialized]
        private static PlayerTurnManager instance = null;
        [NonSerialized]
        private bool isLocalPlayersTurn = true; //0 == unset // 1 == true // 2 == false


        public static PlayerTurnManager Instance { get => instance; set => instance = value; }


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


        public void Start()
        {
            if (ClientInfo.playerNumber == 1)
            {
                isLocalPlayersTurn = true;
            }

            if (ClientInfo.playerNumber == 2)
            {
                EndTurn(false);
            }
        }


        public void StartTurn()
        {
            playerUi.SetActive(true);
            playerTimer.ResetTimer(30);
            playerTimerSubtext.text = "( Your Go ! )";
            isLocalPlayersTurn = true;
            if (playerDieAnim.GetCurrentAnimatorStateInfo(0).IsName("Despawn"))
            {
                playerDieAnim.SetTrigger("ResetDie");
            }

            return;
        }

        public void EndTurn(bool timeRanOut)
        {
            if (isLocalPlayersTurn)
            {
                CleanUpUiAfterTurn.Instance.CleanUpUi();
                playerUi.SetActive(false);
                playerTimer.ResetTimer(30);
                playerTimerSubtext.text = "( Opponents turn... )";
                isLocalPlayersTurn = false;
                if (playerCameraAnim.GetCurrentAnimatorStateInfo(0).IsName("CameraTopView") && !playerCameraAnim.IsInTransition(0))
                {
                    Debug.Log($"camera transition : {playerCameraAnim.IsInTransition(0)}");
                    playerCameraAnim.SetTrigger("Idle");
                }
                if (!playerDieAnim.GetCurrentAnimatorStateInfo(0).IsName("Despawn"))
                {
                    playerDieAnim.Play("Despawn", 0);
                }

                if (timeRanOut)
                {
                    ActionPointsManager.Instance.PlayerTurnHasEnded = true;
                    ActionPointsManager.Instance.UpdateAP(ActionPointsManager.Instance.ApLightsToBeBlinked.Count);
                    ActionPointsManager.Instance.UpdateBlinkingAP();
                    FloorGrid.instance.EmptyGridPointList();
                    dragMovementREF.UpdateDragMoverPosition();
                    dragMovementREF.ResetDragMover();
                }
                else
                {
                    ActionPointsManager.Instance.PlayerTurnHasEnded = true;
                    ActionPointsManager.Instance.UpdateAP(0);
                    FloorGrid.instance.EmptyGridPointList();
                }

                if (LocalStoredNetworkData.localPlayerSelectAbilityToCast)
                {
                    // LocalStoredNetworkData.localPlayerSelectAbilityToCast.ToggleAbilityRadius(false);
                }

                isLocalPlayersTurn = false;
                ClientSend.EndTurn();
                return;
            }
        }
    }
}
