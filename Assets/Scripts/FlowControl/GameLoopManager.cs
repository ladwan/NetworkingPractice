using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ForeverFight.FlowControl
{
    public class GameLoopManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject winnerScreen = null;
        [SerializeField]
        private GameObject loserScreen = null;
        [SerializeField]
        private float loadSceneDelay = 0.0f;
        [SerializeField]
        private string sceneToLoad = "";

        protected void OnEnable()
        {
            ClientHandle.winnerStatusReceived += ShowWinnerScreen;
        }

        protected void OnDisable()
        {
            ClientHandle.winnerStatusReceived -= ShowWinnerScreen;
        }


        public void ShowWinnerScreen()
        {
            winnerScreen.SetActive(true);
            ReturnToLobby();
        }

        public void ShowLoserScreen()
        {
            loserScreen.SetActive(true);
            ReturnToLobby();
        }

        public void ReturnToLobby()
        {
            StartCoroutine(LoadScene());
            Client.localClientInstance.Disconnect();
        }


        private IEnumerator LoadScene()
        {
            yield return new WaitForSecondsRealtime(loadSceneDelay);
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}

