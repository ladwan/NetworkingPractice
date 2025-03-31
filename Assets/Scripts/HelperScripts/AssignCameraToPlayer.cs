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


        private Transform currentPlayerSpawn = null;

        protected void Awake()
        {
            var transformToParentPlayerHolderUnder = ClientInfo.playerNumber == 1 ? player1Spawn : player2Spawn;
            playerHolderREF.transform.parent = transformToParentPlayerHolderUnder;
            playerHolderREF.transform.localPosition = Vector3.zero;

            currentPlayerSpawn = transformToParentPlayerHolderUnder;
        }


        private void Start()
        {
            playerHolderREF.transform.localRotation = currentPlayerSpawn == player1Spawn ?
                                playerHolderREF.transform.localRotation = Quaternion.Euler(new Vector3(0, -90, 0)) :
                                                playerHolderREF.transform.localRotation = Quaternion.Euler(new Vector3(0, -90, 0));
        }
    }
}
