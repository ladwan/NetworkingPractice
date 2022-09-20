using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using ForverFight.HelperScripts;
using ForverFight.Networking;

namespace ForverFight.FlowControl
{
    public class PlayerTurnManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject player1Ui = null;
        [SerializeField]
        private GameObject player2Ui = null;
        [SerializeField]
        private Animator player1DieAnim = null;
        [SerializeField]
        private Animator player2DieAnim = null;
        [SerializeField]
        private Countdown player1Timer = null;
        [SerializeField]
        private Countdown player2Timer = null;
        [SerializeField]
        private TMP_Text player1TimerSubtext = null;
        [SerializeField]
        private TMP_Text player2TimerSubtext = null;


        [NonSerialized]
        private static PlayerTurnManager instance = null;
        [NonSerialized]
        private bool isLocalPlayersTurn = false; //0 == unset // 1 == true // 2 == false

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
                StartCoroutine(Player2Delay());
            }
        }


        public void StartTurn()
        {
            if (ClientInfo.playerNumber == 1)
            {
                player1Ui.gameObject.SetActive(true);
                player1Timer.ResetTimer(30);
                player1TimerSubtext.text = "( Your Go ! )";
                isLocalPlayersTurn = true;
                if (player1DieAnim.GetCurrentAnimatorStateInfo(0).IsName("Despawn"))
                {
                    player1DieAnim.SetTrigger("ResetDie");
                }

                return;
            }
            if (ClientInfo.playerNumber == 2)
            {
                player2Ui.gameObject.SetActive(true);
                player2Timer.ResetTimer(30);
                player2TimerSubtext.text = "( Your Go ! )";
                isLocalPlayersTurn = true;
                if (player2DieAnim.GetCurrentAnimatorStateInfo(0).IsName("Despawn"))
                {
                    player2DieAnim.SetTrigger("ResetDie");
                }

                return;
            }
        }

        public void EndTurn()
        {
            if (isLocalPlayersTurn)
            {
                if (ClientInfo.playerNumber == 1)
                {
                    CleanUpUiAfterTurn.Instance.CleanUpUi();
                    player1Ui.gameObject.SetActive(false);
                    player1Timer.ResetTimer(30);
                    player1TimerSubtext.text = "( Opponents turn... )";
                    isLocalPlayersTurn = false;
                    if (!player1DieAnim.GetCurrentAnimatorStateInfo(0).IsName("Despawn"))
                    {
                        player1DieAnim.Play("Despawn", 0);
                    }
                    DistributedDieValue.SetUnchangingDieRollValue(0);
                    DistributedDieValue.SetDieRollValue(0);

                    if (LocalStoredNetworkData.localPlayerSelectAbilityToCast)
                    {
                        LocalStoredNetworkData.localPlayerSelectAbilityToCast.ToggleAbilityRadius(false);
                        LocalStoredNetworkData.localPlayerSelectAbilityToCast = null;
                    }

                    ClientSend.EndTurn();
                    return;
                }


                if (ClientInfo.playerNumber == 2)
                {
                    CleanUpUiAfterTurn.Instance.CleanUpUi();
                    player2Ui.gameObject.SetActive(false);
                    player2Timer.ResetTimer(30);
                    player2TimerSubtext.text = "( Opponents turn... )";
                    isLocalPlayersTurn = false;
                    if (!player2DieAnim.GetCurrentAnimatorStateInfo(0).IsName("Despawn"))
                    {
                        player2DieAnim.Play("Despawn", 0);
                    }
                    DistributedDieValue.SetUnchangingDieRollValue(0);
                    DistributedDieValue.SetDieRollValue(0);

                    if (LocalStoredNetworkData.localPlayerSelectAbilityToCast)
                    {
                        LocalStoredNetworkData.localPlayerSelectAbilityToCast.ToggleAbilityRadius(false);
                        LocalStoredNetworkData.localPlayerSelectAbilityToCast = null;
                    }

                    ClientSend.EndTurn();
                    return;
                }
            }

        }

        IEnumerator Player2Delay()
        {
            if (player2DieAnim.isActiveAndEnabled)
            {
                if (ClientInfo.playerNumber == 2)
                {
                    if (!player2DieAnim.GetCurrentAnimatorStateInfo(0).IsName("Despawn"))
                    {
                        player2DieAnim.Play("Despawn", 0);
                    }
                }
                StopCoroutine(Player2Delay());
            }
            else
            {
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(Player2Delay());
            }

        }
    }
}
