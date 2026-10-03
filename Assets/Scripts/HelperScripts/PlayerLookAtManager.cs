using System.Collections;
using System.Collections.Generic;
using ForeverFight.FlowControl;
using ForeverFight.GameMechanics.Movement;
using UnityEngine;

namespace ForeverFight.HelperScripts
{
    public class PlayerLookAtManager : MonoBehaviour
    {
        [SerializeField]
        private Transform player1Transform = null;
        [SerializeField]
        private Transform player2Transform = null;

        private BasePlayerLookAt localPlayerLookAt = null;
        private BasePlayerLookAt remotePlayerLookAt = null;

        private void Awake()
        {
            if (ClientInfo.playerNumber == 1)
            {
                InstantiateLookAt(player1Transform.gameObject, true);
                InstantiateLookAt(player2Transform.gameObject, false);
            }
            else
            {
                InstantiateLookAt(player2Transform.gameObject, true);
                InstantiateLookAt(player1Transform.gameObject, false);
            }

            PlayerTurnManager.Instance.IsLocalPlayersTurnAction += EnableLookAtBasedOnLocalPlayerTurn;
        }

        //Subscribed in Start, not Awake: MovementPlanner sets its Instance in its own Awake,
        //which can run after ours, so subscribing in Awake silently missed the move-completed event
        private void Start()
        {
            MovementPlanner.Instance.OnMoveCompleted += TurnOnLookAt;
        }

        private void OnDestroy()
        {
            if (MovementPlanner.Instance != null)
            {
                MovementPlanner.Instance.OnMoveCompleted -= TurnOnLookAt;
            }
        }

        private void TurnOnLookAt()
        {
            //if (AugmentedMovementManager.Instance.CurrentlyDoingAugmentedMovement)
            //{
            //    return;
            //}

            if (PlayerTurnManager.Instance.IsLocalPlayersTurn)
            {
                localPlayerLookAt.enabled = true;
            }
            else
            {
                remotePlayerLookAt.enabled = true;
            }
        }

        //We are turning off the look at scripts so they are not just ticking in the background when they are not needed
        //When the localPlayerTurn var changes. Ensure the lookAt attached to the person whos turn it ISNT, is turned on
        //When its not your turn the look at just constantly ticks to ensure you keep up with the player who is moving
        //So if its not your turn, we need to make sure the lookAt stays on
        private void EnableLookAtBasedOnLocalPlayerTurn(bool value)
        {
            var lookAtToEnable = value ? remotePlayerLookAt : localPlayerLookAt;
            lookAtToEnable.enabled = true;
        }

        private void InstantiateLookAt(GameObject parent, bool isLocalPlayer)
        {
            var lookAt = parent.AddComponent(isLocalPlayer ? typeof(LocalPlayerLookAt) : typeof(RemotePlayerLookAt)) as BasePlayerLookAt;
            lookAt.Player1Transform = player1Transform;
            lookAt.Player2Transform = player2Transform;
            if (isLocalPlayer)
            {
                localPlayerLookAt = lookAt;
            }
            else
            {
                remotePlayerLookAt = lookAt;
            }
        }
    }
}
