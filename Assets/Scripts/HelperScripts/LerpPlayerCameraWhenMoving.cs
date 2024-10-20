using System.Collections;
using UnityEngine;
using ForeverFight.GameMechanics.Movement;
using ForeverFight.Networking;
using ForeverFight.Interactable.Characters;

namespace ForeverFight.HelperScripts
{
    public class LerpPlayerCameraWhenMoving : MonoBehaviour
    {
        [SerializeField]
        private Transform dragMoverTransformREF = null;


        private Coroutine sub = null;
        private Transform localCharacterCameraParent = null;


        private void Start()
        {
            StartCoroutine(LocalStoredNetworkData.WaitForCharacterAnimationReferences(SetCharacterAnimatorReferences));
        }

        private void OnEnable()
        {
            if (FloorGrid.Instance)
            {
                FloorGrid.Instance.DragMoverREF.OnDragMoverPosUpdated += LerpObject;
            }
            else
            {
                StartCoroutine(WaitForDragMoverRef());
            }
        }

        private void OnDisable()
        {
            FloorGrid.Instance.DragMoverREF.OnDragMoverPosUpdated -= LerpObject;
        }


        public void LerpObject()
        {
            EndAndCleanUpCouroutine();
            sub = StartCoroutine(LerpObjToMove());
        }

        public void ReturnObjectBackToOriginalPos()
        {
            EndAndCleanUpCouroutine();
            localCharacterCameraParent.localPosition = Vector3.zero;
        }


        private void EndAndCleanUpCouroutine()
        {
            if (sub != null)
            {
                StopCoroutine(sub);
                sub = null;
            }
        }

        private IEnumerator LerpObjToMove()
        {
            var startingPos = localCharacterCameraParent.position;
            var tempPos = new Vector3(dragMoverTransformREF.position.x, startingPos.y, dragMoverTransformREF.position.z); //get where the dragMover is at

            var time = 0.0f;

            while (localCharacterCameraParent.position != dragMoverTransformREF.position)
            {
                time += Time.deltaTime;
                //var percent = time / duration;
                yield return new WaitForEndOfFrame();
                localCharacterCameraParent.position = Vector3.Lerp(startingPos, tempPos, time);
            }

            sub = null;
        }

        private IEnumerator WaitForDragMoverRef()
        {
            yield return new WaitUntil(() => FloorGrid.Instance);
            FloorGrid.Instance.DragMoverREF.OnDragMoverPosUpdated += LerpObject;
        }

        private void SetCharacterAnimatorReferences(CharacterAnimationReferences animationReferences)
        {
            localCharacterCameraParent = animationReferences.CharacterCameraParent.transform;
        }
    }
}
