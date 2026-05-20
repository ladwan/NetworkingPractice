using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ForeverFight.HelperScripts
{
    public class HandlePlayerDisconnectionHelper : MonoBehaviour
    {
        public void Disconnect()
        {
            //Debug.Log("[Disconnect 1/6] HandlePlayerDisconnectionHelper.Disconnect");

            // ReturnToLobby is async — exceptions after the first await are stored in the Task,
            // not thrown synchronously, so a try-catch here would never catch them.
            var task = HandlePlayerDisconnection.ReturnToLobby();
            //task.ContinueWith(t =>
            //    Debug.LogError("[Disconnect 1/6 FAULT] ReturnToLobby Task faulted: " +
            //        t.Exception?.Flatten().InnerException),
            //    System.Threading.CancellationToken.None,
            //    System.Threading.Tasks.TaskContinuationOptions.OnlyOnFaulted,
            //    System.Threading.Tasks.TaskScheduler.Default);
        }
    }
}
