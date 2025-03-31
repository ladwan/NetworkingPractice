using System;
using UnityEngine;
using ForeverFight.FlowControl;

namespace ForeverFight.HelperScripts
{
    public class BasePlayerLookAt : MonoBehaviour
    {
        private Transform player1Transform = null;
        private Transform player2Transform = null;
        private Transform target = null;
        public static Action onLookAtCompleted = null;
        private bool isPlayer1 = false;
        private static BasePlayerLookAt instance = null;

        public float rotationSpeed = 5f; // The speed at which to rotate


        public Transform Player1Transform { get => player1Transform; set => player1Transform = value; }

        public Transform Player2Transform { get => player2Transform; set => player2Transform = value; }

        public Transform Target { get => target; set => target = value; }

        public bool IsPlayer1 { get => isPlayer1; set => isPlayer1 = value; }

        public static BasePlayerLookAt Instance { get => instance; set => instance = value; }


        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                Debug.Log($"I was made: {gameObject.name}");
            }
            else
            {
                Debug.Log("More Than 1 BaseLookAt detected, Destroying self...");
                Destroy(instance);
                //DestroyThing(instance);
            }
        }

        private void DestroyThing(BasePlayerLookAt toDestroy)
        {
            Destroy(toDestroy);
        }


        private void OnDisable()
        {
            Debug.Log("~~~ ~~~ I was Disabled");
        }

        private void OnDestroy()
        {
            Debug.Log("~~~ ~~~ I was destroyed");
        }

        protected bool IsPlayerTurnAndTargetValid(Transform tempTarget)
        {
            return PlayerTurnManager.Instance.IsLocalPlayersTurn && tempTarget != null;
        }

        protected void RotateTowardsTargetAndDisable(BasePlayerLookAt lookAt, Vector3 targetPos, Transform transformToUpdate)
        {
            var angle = RotateTowardsTarget(lookAt, targetPos, transformToUpdate);

            if (angle < 0.01f)
            {
                onLookAtCompleted?.Invoke();
                lookAt.enabled = false;
            }
        }

        public void InvokeEvent()
        {
            onLookAtCompleted?.Invoke();
        }

        protected float RotateTowardsTarget(BasePlayerLookAt lookAt, Vector3 targetPos, Transform transformToUpdate)
        {
            Vector3 directionToTarget = targetPos - transformToUpdate.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

            // Calculate the angular distance between the current rotation and the target rotation
            float angle = Quaternion.Angle(transformToUpdate.rotation, targetRotation);

            // Calculate step based on the rotation speed and delta time
            float step = rotationSpeed * Time.deltaTime;

            // Use Slerp to smoothly rotate with constant speed
            transformToUpdate.rotation = Quaternion.Slerp(transformToUpdate.rotation, targetRotation, step);

            return angle;
        }
    }
}
