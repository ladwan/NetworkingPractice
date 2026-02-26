using ForeverFight.Networking;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;


public static class HandlePlayerDisconnection
{
    public static async Task ReturnToLobby(float delay = 0, string sceneToLoad = "Lobby")
    {
        LocalStoredNetworkData.Reset();

        await DelaySceneLoad(delay, sceneToLoad);

        Client.localClientInstance.Disconnect();
    }

    private static async Task DelaySceneLoad(float delay = 0, string sceneToLoad = "Lobby")
    {
        await Task.Delay(TimeSpan.FromSeconds(delay));

        SceneManager.LoadScene(sceneToLoad);
    }
}