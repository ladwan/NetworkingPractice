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
            movementBeganEvent?.Invoke(); // Used to update camera angle

            Vector3 finalPos = new Vector3(FloorGrid.Instance.DragMoverREF.transform.position.x, 0, FloorGrid.Instance.DragMoverREF.transform.position.z);
            FloorGrid.Instance.GridDictionary[Vector3ToVector2.ConvertToVector2(finalPos)].DisplayConnections(false);

            for (int i = 0; i < GPs.Count - 1; i++)
            {
                FloorGrid.Instance.RemoveAllButFirstIndexOfHoveredOverGPs();
                if (FloorGrid.Instance.HoveredOverGridPoints.Count == 0)
                {
                    FloorGrid.Instance.TryHighlighting(GPs[i - 1], true);
                }
                FloorGrid.Instance.TryHighlighting(GPs[i], true);
                DisableGPHighlight(GPs);
                FloorGrid.Instance.ConfirmMove();

                yield return new WaitForSecondsRealtime(0.5f);
            }

            Vector3 beforeLastGPposVector3 = new Vector3(finalPos.x, 0.0f, finalPos.z - 1);
            CharAbility.CameraShakeParameters parameters = new CharAbility.CameraShakeParameters();
            ToggleTimerAndUi.Instance.FireAnimationWithoutToggleOffInteractivity(LocalStoredNetworkData.GetLocalCharacter().CharacterAnimationReferences.CharacterAnimator, "Stop To Idle", parameters);


            //We need to keep track of these GPs so we can shut off their highlights while were zipping around
            var tempGPList = new List<GridPoint>();
            tempGPList.Add(FloorGrid.Instance.GridDictionary[Vector3ToVector2.ConvertToVector2(FloorGrid.Instance.LocalPlayerSpawn.transform.position)]);
            tempGPList.Add(FloorGrid.Instance.GridDictionary[Vector3ToVector2.ConvertToVector2(finalPos)]);

            //Here we highlight where you currently are and where you want to go. You need at least 2 GPs in the HoveredOverGP list to make movement work
            FloorGrid.Instance.TryHighlighting(FloorGrid.Instance.GridDictionary
                [Vector3ToVector2.ConvertToVector2(FloorGrid.Instance.LocalPlayerSpawn.transform.position)], true);
            FloorGrid.Instance.TryHighlighting(FloorGrid.Instance.GridDictionary[Vector3ToVector2.ConvertToVector2(finalPos)], true);

            DisableGPHighlight(tempGPList);
            FloorGrid.Instance.ConfirmMove();

            getRandomGridPointREF.ClearList(GPs);
            sub = null;

            yield return new WaitForSecondsRealtime(1.5f);
            movementCompletedEvent?.Invoke();
        }

        private void DisableGPHighlight(List<GridPoint> gridPoints)
        {
            foreach (var gp in gridPoints)
            {
                gp.ShowHighlight(false);
            }
        }

    }
}
