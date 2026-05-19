using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using ForeverFight.FlowControl;

namespace Networking
{
    public class ScenePersistentNetworkUiConnectionManager : MonoBehaviour
    {
        public static ScenePersistentNetworkUiConnectionManager Instance { get; private set; }

        [SerializeField] private GameObject connectionButtonVisualAndInteractivity;
        [SerializeField] private GameObject backButtonVisualAndInteractivity;
        [SerializeField] private GameObject matchBeginTextGameObject;
        [SerializeField] private GameObject waitingTextGameObject;
        [SerializeField] private Button backgroundButton;
        [SerializeField] private Button xButton;
        [SerializeField] private GameObject connectionPopup;
        [SerializeField] private PlayerCountListener playerCountListener;

        public PlayerCountListener PlayerCountListener => playerCountListener;

        private bool isConnected = false;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            string currentScene = SceneManager.GetActiveScene().name;
            backButtonVisualAndInteractivity.SetActive(currentScene != "Main Menu" && currentScene != "CombatScene");

            if (Client.localClientInstance != null)
                Client.localClientInstance.OnConnectionStatusChanged += OnConnectionStatusChanged;

            if (SceneLoader.Instance != null)
                SceneLoader.Instance.OnSceneLoaded += OnSceneLoaded;

            ClientInfo.OnTotalPlayersConnectedChanged += OnPlayerCountChanged;
        }

        private void OnDestroy()
        {
            if (Client.localClientInstance != null)
                
                Client.localClientInstance.OnConnectionStatusChanged -= OnConnectionStatusChanged;

            if (SceneLoader.Instance != null)
                SceneLoader.Instance.OnSceneLoaded -= OnSceneLoaded;

            ClientInfo.OnTotalPlayersConnectedChanged -= OnPlayerCountChanged;
        }

        private void OnConnectionStatusChanged(bool connected)
        {
            isConnected = connected;
            
            if (SceneManager.GetActiveScene().name == "Lobby")
            {
                return;
            }
            
            if (!connected)
            {
                connectionButtonVisualAndInteractivity.SetActive(false);
                waitingTextGameObject.SetActive(true);
                matchBeginTextGameObject.SetActive(false);
                connectionPopup.SetActive(false);
            }
            else
                AttemptToShowConnectionButton();
        }

        private void OnSceneLoaded(string sceneName)
        {
            if (sceneName == "Character Select")
            {
                gameObject.SetActive(false);
                return;
            }

            bool showBack = sceneName != "Main Menu";
            backButtonVisualAndInteractivity.SetActive(showBack);
            AttemptToShowConnectionButton();
        }

        private void OnPlayerCountChanged(int count)
        {
            if (count == 2)
            {
                backButtonVisualAndInteractivity.SetActive(false);

                if (SceneManager.GetActiveScene().name == "Lobby")
                    return;

                connectionPopup.SetActive(true);
                backgroundButton.interactable = false;
                xButton.interactable = false;
                playerCountListener.StartMatchTimer();
                return;
            }

            string currentScene = SceneManager.GetActiveScene().name;
            backButtonVisualAndInteractivity.SetActive(currentScene != "Main Menu" && currentScene != "CombatScene");

            if (currentScene == "Lobby")
                return;

            backgroundButton.interactable = true;
            xButton.interactable = true;
            connectionPopup.SetActive(false);
            playerCountListener.CancelMatchTimer();
        }

        private void AttemptToShowConnectionButton()
        {
            if (!isConnected)
                return;
            
            if (SceneManager.GetActiveScene().name == "Lobby")
            {
                connectionButtonVisualAndInteractivity.SetActive(false);
                return;
            }

            connectionButtonVisualAndInteractivity.SetActive(true);
        }
    }
}
