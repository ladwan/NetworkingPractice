using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.GameMechanics;
using ForverFight.Movement;
using ForverFight.Ui.CharacterSelection;
using ForverFight.Networking;
using ForverFight.FlowControl;

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
    public static void RecieveUpdatedPlayerPosition(Packet _packet)
    {

        int x = _packet.ReadInt();
        int y = _packet.ReadInt();
        FloorGrid.instance.UpdateOpponentPosition(new Vector2(x, y));
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
        string _otherPlayersCharName = _packet.ReadString();
        CharacterSelect.Instance.UpdateOtherPlayerSelection(_panelIndex, _playerIndex);
        LocalStoredNetworkData.locallyStoredOpponentsName = _otherPlayersCharName;
    }

    public static void ReceiveUsername(Packet _packet)
    {
        string _username = _packet.ReadString();


        ClientInfo.otherUsername = _username;
    }

    public static void ReceiveStartTurnSignal(Packet _packet)
    {
        int signalInt = _packet.ReadInt();
        PlayerTurnManager.Instance.StartTurn();
    }
}
