using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using ForeverFight.GameMechanics.Timers;
using ForeverFight.HelperScripts;

public class PlayerCountListener : MonoBehaviour
{
    [SerializeField]
    private Text numOfPlayerText = null;
    [SerializeField]
    private Text lobbyNumOfPlayerText = null;
    [SerializeField]
    private Countdown startMatchTimer;
    [SerializeField]
    private Countdown lobbyStartMatchTimer;
    [SerializeField]
    private Text matchStartCountdown;
    [SerializeField]
    private Text lobbyMatchStartCountdown;

    public Text NumOfPlayerText => numOfPlayerText;
    public Text MatchStartCountdown => matchStartCountdown;
    public Countdown StartMatchTimerREF => startMatchTimer;

    public Text LobbyNumOfPlayerText
    {
        get => lobbyNumOfPlayerText;
        set => lobbyNumOfPlayerText = value;
    }

    public Countdown LobbyStartMatchTimer
    {
        get => lobbyStartMatchTimer;
        set => lobbyStartMatchTimer = value;
    }

    public Text LobbyMatchStartCountdown
    {
        get => lobbyMatchStartCountdown;
        set => lobbyMatchStartCountdown = value;
    }

    private T GetLobbyOrDefault<T>(T lobbyValue, T defaultValue, string fieldName) where T : class
    {
        if (SceneManager.GetActiveScene().name != "Lobby")
            return defaultValue;
        return SafetyNet.IsValid(lobbyValue, fieldName) ? lobbyValue : null;
    }

    private Countdown GetCurrentTimer()       => GetLobbyOrDefault(lobbyStartMatchTimer,    startMatchTimer,    "lobbyStartMatchTimer");
    private Text GetCurrentCountdownText()    => GetLobbyOrDefault(lobbyMatchStartCountdown, matchStartCountdown, "lobbyMatchStartCountdown");
    private Text GetCurrentPlayerCountText()  => GetLobbyOrDefault(lobbyNumOfPlayerText,    numOfPlayerText,    "lobbyNumOfPlayerText");

    private void Start()
    {
        ClientInfo.OnTotalPlayersConnectedChanged += OnPlayerCountChanged;
    }

    private void OnDestroy()
    {
        ClientInfo.OnTotalPlayersConnectedChanged -= OnPlayerCountChanged;
    }

    private void Update()
    {
        Countdown timer = GetCurrentTimer();
        if (timer == null) return;
        Text countdownText = GetCurrentCountdownText();
        if (countdownText == null) return;
        countdownText.text = timer.Time.ToString();
    }

    public void StartMatchTimer()
    {
        var timer = GetCurrentTimer();
        Debug.Log($"[PlayerCountListener] StartMatchTimer — scene={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}, timer={timer?.name ?? "NULL"}, time={timer?.Time.ToString() ?? "N/A"}");
        timer?.StartTimer();
    }

    public void CancelMatchTimer()
    {
        var timer = GetCurrentTimer();
        Debug.Log($"[PlayerCountListener] CancelMatchTimer — scene={UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}, timer={timer?.name ?? "NULL"}, time={timer?.Time.ToString() ?? "N/A"}");
        timer?.ResetToInitialTime();
    }

    public void ResetMatchTimers()
    {
        Debug.Log($"[PlayerCountListener] ResetMatchTimers — startMatchTimer={(startMatchTimer != null ? $"{startMatchTimer.name} time={startMatchTimer.Time}" : "NULL")}, lobbyStartMatchTimer={(lobbyStartMatchTimer != null ? $"{lobbyStartMatchTimer.name} time={lobbyStartMatchTimer.Time}" : "NULL")}");
        // Use Unity's != null (not C# ?.) so destroyed MonoBehaviours are treated as null.
        if (startMatchTimer != null) startMatchTimer.ResetToInitialTime();
        if (lobbyStartMatchTimer != null) lobbyStartMatchTimer.ResetToInitialTime();
    }

    private void OnPlayerCountChanged(int count)
    {
        Text playerCountText = GetCurrentPlayerCountText();
        if (playerCountText != null)
            playerCountText.text = count.ToString();

        if (SceneManager.GetActiveScene().name != "Lobby")
            return;

        if (count == 2)
            StartMatchTimer();
        else
            CancelMatchTimer();
    }
}
