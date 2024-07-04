using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ForeverFight.Movement;
using ForeverFight.Networking;
using ForeverFight.FlowControl;
using ForeverFight.HelperScripts;

namespace ForeverFight.Interactable.Abilities
{
    public class MoveToRandomGPs : MonoBehaviour, IAugmentedMovement
    {
        [SerializeField]
        private GetRandomGridPoint getRandomGridPointREF = null;
        [SerializeField]
        private GenericLerp genericLerpREF = null;
        [SerializeField]
        private UnityEvent movementCompletedEvent = new UnityEvent();


        private List<GridPoint> GPs = new List<GridPoint>();
        private Coroutine sub = null;


        public void BeginMovement()
        {
            if (sub == null)
            {
                GPs = getRandomGridPointREF.GeneranteListOfRandomGPs(4);
                sub = StartCoroutine(Move());
            }
        }

        private IEnumerator Move()
        {
            var playerSpawn = ClientInfo.playerNumber == 1 ? FloorGrid.Instance.Player1Spawn : FloorGrid.Instance.Player2Spawn;
            Vector3 finalPos = playerSpawn.transform.position;

            ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating();

            for (int i = 0; i < GPs.Count - 1; i++)
            {
                FloorGrid.Instance.TryHighlighting(GPs[i], true);
                FloorGrid.Instance.ConfirmMove();
                yield return new WaitForSecondsRealtime(0.5f);
            }

            Vector3 beforeLastGPposVector3 = new Vector3(finalPos.x, 0.0f, finalPos.z - 1);
            //genericLerpREF.BeginLerpCoroutine(playerSpawn, beforeLastGPposVector3, finalPos, 1);
            ToggleTimerAndUi.Instance.FireAnimationWithoutToggleOfInteractivity(LocalStoredNetworkData.GetLocalCharacter().CharacterAnimator, "Stop To Idle");
            ClientSend.ClientSendAnimationTrigger("Stop To Idle");
            //LocalStoredNetworkData.GetLocalCharacter().CharacterAnimator.SetTrigger("Stop To Idle");
            FloorGrid.Instance.TryHighlighting(FloorGrid.Instance.GridDictionary[Vector3ToVector2.ConvertToVector2(finalPos)], true);
            FloorGrid.Instance.ConfirmMove();

            getRandomGridPointREF.ClearList(GPs);
            sub = null;
            movementCompletedEvent?.Invoke();
        }
    }
}
