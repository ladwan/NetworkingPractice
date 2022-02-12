using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UiManager : MonoBehaviour
{
    [SerializeField]
    private static UiManager instance;
    [SerializeField]
    private GameObject startMenu = null;
    [SerializeField]
    private InputField usernameInput = null;


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
            Debug.Log("Instance already exsists, destroying object!");
            Destroy(this);
        }
    }

    public void ConnectToSever()
    {
        startMenu.SetActive(false);
        usernameInput.interactable = false;
        ClientInfo.username = usernameInput.text;
        Client.localClientInstance.ConnectToServer();
    }
}
