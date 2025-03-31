using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ForeverFight.Networking;
using ForeverFight.FlowControl;
using ForeverFight.HelperScripts;
using ForeverFight.GameMechanics.Movement;
using ForeverFight.Ui;
using System;
using ForeverFight.Interactable.Characters;

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
        [SerializeField] private Speedster speedsterREF = null;


        private List<GridPoint> GPs = new List<GridPoint>();
        private Coroutine sub = null;
        private bool moveWasCompleted = false;
        private bool lookAtWasCompleted = false;
        private Action completedSub = null;
        private Action lookAtActionSub = null;
        private int subCount = 0;

        public void BeginMovement()
        {
            if (LocalStoredNetworkData.squaresMovedThisInstanceOfMovement < 10)
            {
                //LocalStoredNetworkData.GetCountdownTimerScript().TellNetworkToToggleTimer();
                ToggleTimerAndUi.Instance.ToggleInteractivityWhileAnimating();
                FloorGrid.Instance.ConfirmMove();
                LocalStoredNetworkData.squaresMovedThisInstanceOfMovement = 0;
                return;
            }

            //if (sub == null)
            //{
            //    GPs = getRandomGridPointREF.GeneranteListOfRandomGPs(4);
            //    Vector3 finalPos = new Vector3(
            //        FloorGrid.Instance.DragMoverREF.transform.position.x, 0, FloorGrid.Instance.DragMoverREF.transform.position.z);

            //    GPs.Add(FloorGrid.Instance.GridDictionary[Vector3ToVector2.ConvertToVector2(finalPos)]);


            //    DisableGPHighlight(FloorGrid.Instance.HoveredOverGridPoints);

            //    FloorGrid.Instance.HoveredOverGridPoints = GPs;
            //    //FloorGrid.Instance.HoveredOverGridPoints.Add(
            //    //FloorGrid.Instance.GridDictionary[Vector3ToVector2.ConvertToVector2(finalPos)]);

            //    FloorGrid.Instance.ConfirmMove();
            //    //sub = StartCoroutine(Move());
            //    LocalStoredNetworkData.squaresMovedThisInstanceOfMovement = 0;
            //}

            GPs = getRandomGridPointREF.GeneranteListOfRandomGPs(4);

            Vector3 finalPos = new Vector3(
                FloorGrid.Instance.DragMoverREF.transform.position.x, 0, FloorGrid.Instance.DragMoverREF.transform.position.z);

            GPs.Add(FloorGrid.Instance.GridDictionary[Vector3ToVector2.ConvertToVector2(finalPos)]);
            DisableGPHighlight(FloorGrid.Instance.HoveredOverGridPoints);
            FloorGrid.Instance.HoveredOverGridPoints = GPs;
            LocalStoredNetworkData.squaresMovedThisInstanceOfMovement = 0;
            StartCoroutine(TryMove());
            AugmentedMovementManager.Instance.CurrentlyDoingAugmentedMovement = true;
        }

        private void SubscribeToLookAtEvent()
        {
            if (BasePlayerLookAt.Instance != null)
            {
                BasePlayerLookAt.onLookAtCompleted += LookAtCompleted;
                Debug.Log($"Subscribed to onLookAtCompleted event: {BasePlayerLookAt.Instance.gameObject.name}");
            }
            else
            {
                Debug.LogError("BasePlayerLookAt.Instance is null, cannot subscribe");
            }
        }

        private void UnsubscribeFromLookAtEvent()
        {
            if (BasePlayerLookAt.Instance != null)
            {
                BasePlayerLookAt.onLookAtCompleted -= LookAtCompleted;
                Debug.Log("Unsubscribed from onLookAtCompleted event");
            }
        }

        private IEnumerator TryMove()
        {
            if (completedSub != null)
            {
                Debug.LogWarning("~~ This wasn't supposed to run!");
                yield break;
            }



            if (GPs.Count == 1)
            {
                speedsterREF.MoveSpeed = 21;
                speedsterREF.CharacterAnimationReferences.CharacterAnimator.SetFloat("CharSpeed", speedsterREF.MoveSpeed);
            }
            else
            {
                speedsterREF.MoveSpeed = 20;
                speedsterREF.CharacterAnimationReferences.CharacterAnimator.SetFloat("CharSpeed", speedsterREF.MoveSpeed);
            }

            moveWasCompleted = false;
            lookAtWasCompleted = false;

            completedSub = FloorGrid.Instance.OnMoveCompleted += IsMoveComplete;
            SubscribeToLookAtEvent();  // Ensure the event is subscribed

            var gps = new List<GridPoint>();

            gps.Add(
            FloorGrid.Instance.GridDictionary[Vector3ToVector2.ConvertToVector2(
                FloorGrid.Instance.LocalPlayerSpawn.transform.position)]);

            if (GPs.Count >= 1)
            {
                gps.Add(GPs[0]);
                GPs.RemoveAt(0);
            }

            FloorGrid.Instance.HoveredOverGridPoints = gps;
            FloorGrid.Instance.ConfirmMove();
            yield return new WaitUntil(() => moveWasCompleted);


            speedsterREF.MoveSpeed = 0;
            speedsterREF.CharacterAnimationReferences.CharacterAnimator.SetFloat("CharSpeed", speedsterREF.MoveSpeed);


            yield return new WaitForSecondsRealtime(0.25f);


            if (GPs.Count == 0)
            {
                AugmentedMovementManager.Instance.CurrentlyDoingAugmentedMovement = false;
                yield break;
            }

            StartCoroutine(TryMove());

            UnsubscribeFromLookAtEvent();  // Unsubscribe when done
        }






















        private IEnumerator TryMoves()
        {
            if (completedSub != null)
            {
                Debug.LogWarning("~~ This wasnt supposed to run! ");
                yield break;
            }

            moveWasCompleted = false;
            lookAtWasCompleted = false;

            completedSub = FloorGrid.Instance.OnMoveCompleted += IsMoveComplete;
            lookAtActionSub = BasePlayerLookAt.onLookAtCompleted += LookAtCompleted;

            var gps = new List<GridPoint>();

            //Add where the local player currently is located to the list first.
            gps.Add(
            FloorGrid.Instance.GridDictionary[Vector3ToVector2.ConvertToVector2(
                FloorGrid.Instance.LocalPlayerSpawn.transform.position)]);

            if (GPs.Count >= 1)
            {
                gps.Add(GPs[0]);
                GPs.RemoveAt(0);
            }

            FloorGrid.Instance.HoveredOverGridPoints = gps;
            FloorGrid.Instance.ConfirmMove();
            yield return new WaitUntil(() => moveWasCompleted);
            yield return new WaitUntil(() => lookAtWasCompleted);

            if (GPs.Count == 0)
            {
                yield break;
            }

            StartCoroutine(TryMove());
        }

        private void IsMoveComplete()
        {
            FloorGrid.Instance.OnMoveCompleted -= IsMoveComplete;
            completedSub = null;
            moveWasCompleted = true;
        }

        private void LookAtCompleted()
        {
            //BasePlayerLookAt.Instance.onLookAtCompleted -= LookAtCompleted;
            lookAtActionSub = null;
            lookAtWasCompleted = true;
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
            ToggleTimerAndUi.Instance.FireAnimationWithoutToggleOffInteractivity
                (LocalStoredNetworkData.GetLocalCharacter().CharacterAnimationReferences.CharacterAnimator, "Stop To Idle", parameters);


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
