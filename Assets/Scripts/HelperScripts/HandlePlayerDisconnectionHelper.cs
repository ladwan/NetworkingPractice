using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.HelperScripts
{
    public class HandlePlayerDisconnectionHelper : MonoBehaviour
    {
        public void Disconnect()
        {
            HandlePlayerDisconnection.ReturnToLobby();
        }
    }
}
