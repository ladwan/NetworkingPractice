using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClientInfo
{
    public static int playerNumber = 0;

    private static int _totalPlayersConnected = 0;
    public static event Action<int> OnTotalPlayersConnectedChanged;
    public static int totalPlayersConnected
    {
        get => _totalPlayersConnected;
        set
        {
            _totalPlayersConnected = value;
            OnTotalPlayersConnectedChanged?.Invoke(_totalPlayersConnected);
        }
    }

    public static int matchIndex = -1;

    public static string username = "";

    public static string otherUsername = "";

    public static void Reset()
    {
        playerNumber = 0;

        totalPlayersConnected = 0;

        matchIndex = -1;

        username = "";

        otherUsername = "";
    }
}
