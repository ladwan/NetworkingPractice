using TMPro;
using UnityEngine;

public class SaveServerIp : MonoBehaviour
{
    public TMP_InputField ipInputField;

    void Start()
    {
        // Load saved IP into the input field
        if (ServerConfig.HasServerIP())
        {
            ipInputField.text = ServerConfig.GetServerIP();
            Client.localClientInstance.serverIp = ipInputField.text;
        }
        else
        {
            Client.localClientInstance.serverIp = "127.0.0.1";
        }
    }

    public void OnSaveButtonClicked()
    {
        string ip = ipInputField.text.Trim();
        ServerConfig.SaveServerIP(ip);
        Client.localClientInstance.serverIp = ipInputField.text;
        Debug.Log("Saved IP: " + ip);
    }

    public void OnDeleteButtonClicked()
    {
        ServerConfig.ClearServerIP();
        ipInputField.text = "";
        Client.localClientInstance.serverIp = "127.0.0.1";
        Debug.Log("Cleared saved IP");
    }
}