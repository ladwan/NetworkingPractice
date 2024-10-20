using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ForeverFight.Networking;
using ForeverFight.FlowControl;
using ForeverFight.HelperScripts;
using ForeverFight.GameMechanics.Movement;
using ForeverFight.Ui;

namespace ForeverFight.Interactable.Abilities
{
    public class MoveToRandomGPs : MonoBehaviour, IAugmentedMovement
    {
        [SerializeField]
        private GetRandomGridPoint getRandomGridPointREF = null;
        [SerializeField]
        private GenericLerp genericLerpREF = null;
        [SerializeField]
        private UnityEvent movementBeganEvent = new UnityEvent();
        [SerializeField]
        private UnityEvent movementCompletedEvent = new UnityEvent();


        private List<GridPoint> GPs = new List<GridPoint>();
        private Coroutine sub = null;


        public void BeginMovement()
        {
            if (LocalStoredNetworkData.squaresMovedThisInstanceOfMovement < 4)
            {
                //LocalStoredNetworkData.GetCountdownTimerScript().TellNetworkToToggleTimer();
                ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating();
                FloorGrid.Instance.ConfirmMove();
                LocalStoredNetworkData.squaresMovedThisInstanceOfMovement = 0;
                return;
            }

            if (sub == null)
            {
                GPs = getRandomGridPointREF.GeneranteListOfRandomGPs(4);
                sub = StartCoroutine(Move());
                LocalStoredNetworkData.squaresMovedThisInstanceOfMovement = 0;
            }
        }

        private IEnumerator Move()
        {
            movementBeganEvent?.Invoke();
            var playerSpawn = ClientInfo.playerNumber == 1 ? FloorGrid.Instance.Player1Spawn : FloorGrid.Instance.Player2Spawn;
            Vector3 finalPos = playerSpawn.transform.position;

            for (int i = 0; i < GPs.Count - 1; i++)
            {
                FloorGrid.Instance.TryHighlighting(GPs[i], true);
                FloorGrid.Instance.ConfirmMove();
                yield return new WaitForSecondsRealtime(0.5f);
            }

            Vector3 beforeLastGPposVector3 = new Vector3(finalPos.x, 0.0f, finalPos.z - 1);
            CharAbility.CameraShakeParameters parameters = new CharAbility.CameraShakeParameters();
            ToggleTimerAndUi.Instance.FireAnimationWithoutToggleOffInteractivity(LocalStoredNetworkData.GetLocalCharacter().CharacterAnimationReferences.CharacterAnimator, "Stop To Idle", parameters);
            FloorGrid.Instance.TryHighlighting(FloorGrid.Instance.GridDictionary[Vector3ToVector2.ConvertToVector2(finalPos)], true);
            FloorGrid.Instance.ConfirmMove();

            getRandomGridPointREF.ClearList(GPs);
            sub = null;

            yield return new WaitForSecondsRealtime(1.5f);
            movementCompletedEvent?.Invoke();
        }
    }
}
