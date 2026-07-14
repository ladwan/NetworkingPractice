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
        private float speed = 1.0f;


        private Coroutine sub = null;
        private Transform localCharacterCameraParent = null;


        private void Start()
        {
            StartCoroutine(LocalStoredNetworkData.WaitForCharacterAnimationReferences(SetCharacterAnimatorReferences));
        }

        private void OnEnable()
        {
            if (MovementPlanner.Instance)
            {
                MovementPlanner.Instance.OnPlanUpdated += LerpObject;
            }
            else
            {
                StartCoroutine(WaitForPlannerRef());
            }
        }

        private void OnDisable()
        {
            if (MovementPlanner.Instance)
            {
                MovementPlanner.Instance.OnPlanUpdated -= LerpObject;
            }
        }


        public void LerpObject(float pathLength, int apCost)
        {
            EndAndCleanUpCouroutine();
            sub = StartCoroutine(LerpObjToMove());
        }

        public void ReturnObjectBackToOriginalPos()
        {
            EndAndCleanUpCouroutine();
            if (localCharacterCameraParent != null)
            {
                localCharacterCameraParent.localPosition = Vector3.zero;
            }
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
            var endpoint = MovementGuideLine.Instance != null ? MovementGuideLine.Instance.EndpointTransform : null;
            if (endpoint == null || localCharacterCameraParent == null)
            {
                sub = null;
                yield break;
            }

            var startingPos = localCharacterCameraParent.position;
            var tempPos = new Vector3(endpoint.position.x, startingPos.y, endpoint.position.z);

            var time = 0.0f;

            while (localCharacterCameraParent.position != tempPos)
            {
                time += Time.deltaTime * speed;
                yield return new WaitForEndOfFrame();
                localCharacterCameraParent.position = Vector3.Lerp(startingPos, tempPos, time);
            }

            sub = null;
        }

        private IEnumerator WaitForPlannerRef()
        {
            yield return new WaitUntil(() => MovementPlanner.Instance);
            MovementPlanner.Instance.OnPlanUpdated += LerpObject;
        }

        private void SetCharacterAnimatorReferences(CharacterAnimationReferences animationReferences)
        {
            localCharacterCameraParent = animationReferences.CharacterCameraParent.transform;
        }
    }
}
