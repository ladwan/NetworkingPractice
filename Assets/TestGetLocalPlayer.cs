using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Networking;

public class TestGetLocalPlayer : MonoBehaviour
{
    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PrintLocalPlayerChar();
        }
    }

    public void PrintLocalPlayerChar()
    {
        Debug.Log($"Local Player Char: {LocalStoredNetworkData.GetLocalCharacter().CharacterName}");
    }
}
