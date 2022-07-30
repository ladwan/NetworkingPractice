﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.Movement;
public class ClientSend : MonoBehaviour
{
    private static void SendTcpData(Packet _packet)
    {
        _packet.WriteLength();
        Client.localClientInstance.tcp.SendData(_packet);
    }

    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.localClientInstance.localClientId);
            _packet.Write(UiManager.Instance.UsernameInput.text);

            SendTcpData(_packet);
        }
    }

    public static void TryMove()
    {
        using (Packet _packet = new Packet((int)ClientPackets.tryMove))
        {
            _packet.Write(Movement.instance.movementIndex);

            SendTcpData(_packet);
        }
    }

    public static void SendSelectionData(int _panelIndex, int _playerIndex, string _selectedCharName)
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendSelectionData))
        {
            _packet.Write(_panelIndex);
            _packet.Write(_playerIndex);
            _packet.Write(_selectedCharName);

            SendTcpData(_packet);
        }
    }
    #endregion
}
