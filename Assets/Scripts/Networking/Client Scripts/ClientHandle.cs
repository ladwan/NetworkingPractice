using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.Movement;
using ForverFight.Ui.CharacterSelection;

public class ClientHandle : MonoBehaviour
{
    public static void Welcome(Packet _packet)
    {
        string _msg = _packet.ReadString();
        int _myId = _packet.ReadInt();
        int _totalPlayers = _packet.ReadInt();
        ClientInfo.totalPlayersConnected = _totalPlayers;
        if (ClientInfo.totalPlayersConnected == 1)
        {
            ClientInfo.playerNumber = 1;
        }
        else if (ClientInfo.totalPlayersConnected == 2)
        {
            ClientInfo.playerNumber = 2;
        }
        Debug.Log($"Message from server: {_msg}.");
        Client.localClientInstance.localClientId = _myId;

        ClientSend.WelcomeReceived();
    }
    public static void ConfirmMove(Packet _packet)
    {
        int _movementData = _packet.ReadInt();
        int _myId = _packet.ReadInt();

        Debug.Log($"Message from server: You have moved {_movementData} spaces!");
        Client.localClientInstance.localClientId = _myId;
        Movement.instance.ServerMovePlayer(_movementData);
    }

    public static void ReceiveTotalPlayerUpdate(Packet _packet)
    {
        int _totalPlayers = _packet.ReadInt();
        ClientInfo.totalPlayersConnected++;
    }

    public static void ReceiveSelectionPacket(Packet _packet)
    {
        int _panelIndex = _packet.ReadInt();
        int _playerIndex = _packet.ReadInt();

        CharacterSelect.Instance.UpdateOtherPlayerSelection(_panelIndex, _playerIndex);
    }

    public static void ReceiveUsername(Packet _packet)
    {
        string _username = _packet.ReadString();


        ClientInfo.otherUsername = _username;
    }
}
