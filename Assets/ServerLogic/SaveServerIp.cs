using TMPro;
using UnityEngine;

public class SaveServerIp : MonoBehaviour
{
    [SerializeField] private Client client;
    public TMP_InputField ipInputField;

    void Start()
    {
        // Load saved IP into the input field
        if (ServerConfig.HasServerIP())
        {
            ipInputField.text = ServerConfig.GetServerIP();
            client.serverIp = ipInputField.text;
        }
        else
        {
            client.serverIp = "127.0.0.1";
        }
    }

    public void OnSaveButtonClicked()
    {
        string ip = ipInputField.text.Trim();
        ServerConfig.SaveServerIP(ip);
        client.serverIp = ipInputField.text;
        Debug.Log("Saved IP: " + ip);
    }

    public void OnDeleteButtonClicked()
    {
        ServerConfig.ClearServerIP();
        ipInputField.text = "";
        client.serverIp = "127.0.0.1";
        Debug.Log("Cleared saved IP");
    }
}