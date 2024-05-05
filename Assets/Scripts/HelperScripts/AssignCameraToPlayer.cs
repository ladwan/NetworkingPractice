using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.HelperScripts
{
    public class AssignCameraToPlayer : MonoBehaviour
    {
        [SerializeField]
        private GameObject playerHolderREF = null;
        [SerializeField]
        private Transform player1Spawn = null;
        [SerializeField]
        private Transform player2Spawn = null;

        protected void Awake()
        {
            var transformToParentPlayerHolderUnder = ClientInfo.playerNumber == 1 ? player1Spawn : player2Spawn;
            playerHolderREF.transform.parent = transformToParentPlayerHolderUnder;
            playerHolderREF.transform.position = Vector3.zero;
        }
    }
}
