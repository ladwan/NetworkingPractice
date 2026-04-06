using UnityEngine;

public static class ServerConfig
{
    private const string ServerIpKey = "SERVER_IP";

    public static void SaveServerIP(string ip)
    {
        PlayerPrefs.SetString(ServerIpKey, ip);
        PlayerPrefs.Save();
    }

    public static string GetServerIP()
    {
        return PlayerPrefs.GetString(ServerIpKey, "127.0.0.1");
    }

    public static void ClearServerIP()
    {
        PlayerPrefs.DeleteKey(ServerIpKey);
    }

    public static bool HasServerIP()
    {
        return PlayerPrefs.HasKey(ServerIpKey);
    }
}