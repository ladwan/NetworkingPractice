using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCountListener : MonoBehaviour
{
    [SerializeField]
    private Text numOfPlayerText = null;
    [SerializeField]
    private Countdown startMatchTimer;
    [SerializeField]
    private Text matchStartCountdown;

    private bool doOnce = false;

    private void Update()
    {
        numOfPlayerText.text = ClientInfo.totalPlayersConnected.ToString();
        matchStartCountdown.text = startMatchTimer.Time.ToString();
        if (ClientInfo.totalPlayersConnected == 2)
        {
            if (doOnce == false)
            {
                startMatchTimer.StartTimer();
                doOnce = true;
            }
        }
    }
}
