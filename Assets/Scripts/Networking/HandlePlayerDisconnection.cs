using ForeverFight.Networking;
using System;
using System.Threading.Tasks;
using ForeverFight.FlowControl;
using ForeverFight.HelperScripts;
using Networking;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class HandlePlayerDisconnection
{
    public static async Task ReturnToLobby(float delay = 0, string sceneToLoad = "Main Menu")
    {
        //Debug.Log("[Disconnect 2/6] HandlePlayerDisconnection.ReturnToLobby entry");

        // ?.  does not catch Unity fake-null (destroyed objects); use explicit Unity-safe checks instead.
        var networkUiManager = ScenePersistentNetworkUiConnectionManager.Instance;
        if (networkUiManager != null)
        {
            var listener = networkUiManager.PlayerCountListener;
            if (listener != null)
                listener.ResetMatchTimers();
        }

        //Debug.Log($"[Disconnect 2a/6] After ResetMatchTimers. matchIndex={ClientInfo.matchIndex}, totalPlayersConnected={ClientInfo.totalPlayersConnected}, localClientId={Client.localClientInstance?.localClientId}");

        LocalStoredNetworkData.Reset(); // NOTE: this calls ClientInfo.Reset(), zeroing matchIndex and totalPlayersConnected before Disconnect() is called.

        //Debug.Log($"[Disconnect 2b/6] After Reset, before scene load. localClientInstance is {(Client.localClientInstance == null ? "NULL" : "valid")}");

        await DelaySceneLoad(delay, sceneToLoad);

        //Debug.Log($"[Disconnect 2c/6] After scene load, about to Disconnect. localClientInstance is {(Client.localClientInstance == null ? "NULL" : "valid")}");

        Client.localClientInstance.Disconnect();
    }

    private static async Task DelaySceneLoad(float delay = 0, string sceneToLoad = "Main Menu")
    {
        //Debug.Log($"[Disconnect 3/6] DelaySceneLoad entry. delay={delay}, scene={sceneToLoad}");

        await Task.Delay(TimeSpan.FromSeconds(delay));

        //Debug.Log("[Disconnect 3a/6] After Task.Delay, about to start scene load");

        if (SafetyNet.IsValid(SceneLoader.Instance, "SceneLoader Instance"))
        {
            //Debug.Log("[Disconnect 3b/6] Using SceneLoader.Instance.LoadScene");
            SceneLoader.Instance.LoadScene(sceneToLoad);
        }
        else
        {
            //Debug.Log("[Disconnect 3b/6] SceneLoader.Instance null — falling back to SceneManager.LoadSceneAsync");
            SceneManager.LoadSceneAsync(sceneToLoad);
        }

        //Debug.Log("[Disconnect 3c/6] DelaySceneLoad returning (scene load started, not yet complete)");
    }
}