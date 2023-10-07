using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForverFight.HelperScripts
{
    public class LerpPlayerCameraWhenMoving : MonoBehaviour
    {
        [SerializeField]
        private Transform objToLerp = null;
        [SerializeField]
        private Transform pos1REF = null;
        [SerializeField]
        private Transform pos2REF = null;
        [SerializeField]
        private float duration = 1.0f;


        private Coroutine sub = null;
        private float time = 0f;


        protected void OnEnable()
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

        protected void OnDisable()
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
            objToLerp.localPosition = Vector3.zero;
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
            var startingPos = Vector3.zero;
            var endingPos = Vector3.zero;
            var tempPos = Vector3.zero;

            startingPos = pos1REF.position;
            endingPos = pos2REF.position;
            tempPos = new Vector3(endingPos.x, startingPos.y, endingPos.z);

            time = 0;
            while (objToLerp.position != pos2REF.position)
            {
                time += Time.deltaTime;
                var percent = time / duration;
                yield return new WaitForEndOfFrame();
                objToLerp.position = Vector3.Lerp(startingPos, tempPos, percent);
            }

            sub = null;
        }

        private IEnumerator WaitForDragMoverRef()
        {
            yield return new WaitUntil(() => FloorGrid.Instance);
            FloorGrid.Instance.DragMoverREF.OnDragMoverPosUpdated += LerpObject;
        }
    }
}
