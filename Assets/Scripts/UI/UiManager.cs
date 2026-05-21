using System.Collections;
using System.Collections.Generic;
using ForeverFight.GameMechanics.Timers;
using ForeverFight.HelperScripts;
using Networking;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UiManager : MonoBehaviour
{
    [SerializeField]
    private static UiManager instance;
    [SerializeField]
    private GameObject startMenu = null;
    [SerializeField]
    private GameObject lobbyPanel = null;
    [SerializeField]
    private InputField usernameInput = null;
    [SerializeField]
    private Countdown lobbyCountdown = null;
    [SerializeField]
    private Text lobbyMatchStartCountdownText = null;
    [SerializeField]
    private Text lobbyNumOfPlayerText = null;


    public static UiManager Instance { get => instance; set => instance = value; }

    public GameObject StartMenu { get => startMenu; set => startMenu = value; }

    public InputField UsernameInput { get => usernameInput; set => usernameInput = value; }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        UpdateUiIfAlreadyConnected();
        InitLobbyCountdownIfNeeded();
    }

    private void InitLobbyCountdownIfNeeded()
    {
        ScenePersistentNetworkUiConnectionManager networkUi = ScenePersistentNetworkUiConnectionManager.Instance;
        if (!SafetyNet.IsValid(networkUi, "ScenePersistentNetworkUiConnectionManager in UiManager.cs")) return;

        PlayerCountListener playerCountListener = networkUi.PlayerCountListener;
        if (!SafetyNet.IsValid(playerCountListener, "PlayerCountListener in UiManager.cs")) return;

        if (playerCountListener.LobbyStartMatchTimer == null)
        {
            if (!SafetyNet.IsValid(lobbyCountdown, "lobbyCountdown in UiManager.cs")) return;
            playerCountListener.LobbyStartMatchTimer = lobbyCountdown;
        }

        if (playerCountListener.LobbyMatchStartCountdown == null)
        {
            if (!SafetyNet.IsValid(lobbyMatchStartCountdownText, "lobbyMatchStartCountdownText in UiManager.cs")) return;
            playerCountListener.LobbyMatchStartCountdown = lobbyMatchStartCountdownText;
        }

        if (playerCountListener.LobbyNumOfPlayerText == null)
        {
            if (!SafetyNet.IsValid(lobbyNumOfPlayerText, "lobbyNumOfPlayerText in UiManager.cs")) return;
            playerCountListener.LobbyNumOfPlayerText = lobbyNumOfPlayerText;
        }
    }


    public void ConnectToSever()
    {
        startMenu.SetActive(false);
        usernameInput.interactable = false;
        ClientInfo.username = usernameInput.text;
        Client.localClientInstance.ConnectToServer();
    }

    public void UpdateUiIfAlreadyConnected()
    {
        if (!Client.localClientInstance.IsConnected)
        {
            return;
        }

        startMenu.SetActive(false);
        lobbyPanel.SetActive(true);
    }
}
