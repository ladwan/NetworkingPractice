using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Ui.CharacterSelection;
using ForeverFight.Interactable.Abilities;

public class ClientSend : MonoBehaviour
{
    private static void SendTcpData(Packet _packet)
    {
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

    public static void ToggleCountdownTimer()
    {
        using (Packet _packet = new Packet((int)ClientPackets.toggleTimerCountdown))
        {
            int signalInt = 1;
            _packet.Write(signalInt);
            SendTcpData(_packet);
        }
    }

    public static void ClientSendAnimationTrigger(string trigger)
    {
        using (Packet _packet = new Packet((int)ClientPackets.clientSendAnimationTrigger))
        {
            _packet.Write(trigger);
            SendTcpData(_packet);
        }
    }

    public static void RequestToDamageOpponentsHealth(int damage)
    {
        using (Packet _packet = new Packet((int)ClientPackets.requestToDamageOpponentsHealth))
        {
            _packet.Write(damage);

            SendTcpData(_packet);
        }
    }

    public static void SendStatusEffectData(StatusEffect.StatusEffectType statusEffectIdentifier, int duration, int ownership, bool endThisStatusEffect)
    {
        using (Packet _packet = new Packet((int)ClientPackets.clientSendStatusEffectData))
        {
            _packet.Write(((int)statusEffectIdentifier));
            _packet.Write(duration);
            _packet.Write(ownership);
            _packet.Write(endThisStatusEffect);

            SendTcpData(_packet);
        }
    }

    public static void SendStatusEffectCurrentDuration(int currentDuration)
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendCurrentStatusEffectDuration))
        {
            _packet.Write(currentDuration);
            SendTcpData(_packet);
        }
    }

    public static void SendStoredMomentumValue(int storedMomentum)
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendStoredMomentumValue))
        {
            _packet.Write(storedMomentum);
            SendTcpData(_packet);
        }
    }

    public static void OverrideOppositePlayersPostition(int x, int y) // Pass this a vector 2's x and y
    {
        using (Packet _packet = new Packet((int)ClientPackets.overrideOppositePlayersPos))
        {
            _packet.Write(x);
            _packet.Write(y);

            SendTcpData(_packet);
        }
    }

    public static void SendWinnerStatus(bool hasWonTheMatch)
    {
        using (Packet _packet = new Packet((int)ClientPackets.hasWonTheMatch))
        {
            _packet.Write(hasWonTheMatch);

            SendTcpData(_packet);
        }
    }
    #endregion
}
