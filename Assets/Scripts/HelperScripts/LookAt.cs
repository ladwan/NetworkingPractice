using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForverFight.HelperScripts
{
    public class LookAt : MonoBehaviour
    {
        [SerializeField]
        private Animator cameraAnimatorREF = null;
        [SerializeField]
        private Transform player1Transform = null;
        [SerializeField]
        private Transform player2Transform = null;


        private Transform target = null;


        protected void Awake()
        {
            target = ClientInfo.playerNumber == 1 ? target = player2Transform : target = player1Transform;
        }


        void LateUpdate()
        {
            if (cameraAnimatorREF.GetCurrentAnimatorStateInfo(0).IsName("CameraIdle") && !cameraAnimatorREF.IsInTransition(0))
            {
                // Rotate the camera every frame so it keeps looking at the target
                transform.LookAt(target);

                // Same as above, but setting the worldUp parameter to Vector3.left in this example turns the camera on its side
                transform.LookAt(target, Vector3.up);
            }
        }
    }
}
