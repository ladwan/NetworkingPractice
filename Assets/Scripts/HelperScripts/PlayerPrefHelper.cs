using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefHelper
{
    public static void IncrementPlayerPrefValue(string key)
    {
        int val = PlayerPrefs.GetInt(key, 0) + 1;
        PlayerPrefs.SetInt(key, val);
        PlayerPrefs.Save();
    }
}
