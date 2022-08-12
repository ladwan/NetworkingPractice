using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForverFight.Movement;
using ForverFight.Ui.CharacterSelection;
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

    public static void UpdatePlayerCurrentPostition(int x, int y) // Pass this a vector 2's x and y
    {
        using (Packet _packet = new Packet((int)ClientPackets.updatePlayerCurrentPosition))
        {
            _packet.Write(x);
            _packet.Write(y);

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

    public static void EndTurn()
    {
        using (Packet _packet = new Packet((int)ClientPackets.endTurn))
        {
            int signalInt = 1;
            _packet.Write(signalInt);
            SendTcpData(_packet);
        }
    }

    public static void SendReadyUp()
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendReadyUp))
        {
            int signalInt = 1;
            _packet.Write(signalInt);
            SendTcpData(_packet);
        }
    }

    public static void EnterSyncTimerQueue()
    {
        using (Packet _packet = new Packet((int)ClientPackets.enterSyncTimerQueue))
        {
            _packet.Write(CharacterSelect.Instance.CountdownTimer.Time);
            SendTcpData(_packet);
        }
    }
    #endregion
}
