using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Ui.CharacterSelection;
using ForeverFight.Interactable.Abilities;
using ForeverFight.Networking;

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

    public static void SendSelectionData(int _panelIndex, int _playerIndex, string _selectedCharName)
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendSelectionData))
        {
            _packet.Write((int)ClientPackets.sendSelectionData);

            _packet.Write(_panelIndex);
            _packet.Write(_playerIndex);
            _packet.Write(_selectedCharName);

            SendTcpData(_packet);
        }
    }

    public static void SendReadyUp()
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendReadyUp))
        {
            _packet.Write((int)ClientPackets.sendReadyUp);

            int signalInt = 1;
            _packet.Write(signalInt);
            SendTcpData(_packet);
        }
    }

    public static void EnterSyncTimerQueue()
    {
        using (Packet _packet = new Packet((int)ClientPackets.enterSyncTimerQueue))
        {
            _packet.Write((int)ClientPackets.enterSyncTimerQueue);

            _packet.Write(CharacterSelect.Instance.CountdownTimer.Time);
            SendTcpData(_packet);
        }
    }

    public static void SendSegmentedMovementData(int x, int y, int count, bool hasRotations, bool completed) // Pass this a vector 3's x and z
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendSegmentedMovementData))
        {
            _packet.Write((int)ClientPackets.sendSegmentedMovementData);

            _packet.Write(x);
            _packet.Write(y);
            _packet.Write(count);
            _packet.Write(hasRotations);
            _packet.Write(completed);

            SendTcpData(_packet);
        }
    }

    public static void SendSegmentedRotationData(float x, float y, float z, float w, int count)
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendSegmentedRotationData))
        {
            _packet.Write((int)ClientPackets.sendSegmentedRotationData);

            _packet.Write(x);
            _packet.Write(y);
            _packet.Write(z);
            _packet.Write(w);
            _packet.Write(count);

            SendTcpData(_packet);
        }
    }

    // This is going to be sent to BOTH players, meaning you will send this to yourself!
    public static void ToggleCountdownTimer()
    {
        using (Packet _packet = new Packet((int)ClientPackets.toggleTimerCountdown))
        {
            _packet.Write((int)ClientPackets.toggleTimerCountdown);

            int signalInt = 1;
            _packet.Write(signalInt);
            SendTcpData(_packet);
        }
    }

    public static void EndTurn()
    {
        using (Packet _packet = new Packet((int)ClientPackets.endTurn))
        {
            _packet.Write((int)ClientPackets.endTurn);

            int signalInt = 1;
            _packet.Write(signalInt);
            SendTcpData(_packet);
        }
    }

    public static void ClientSendAnimationTrigger(string trigger, float duration, float magnitude)
    {
        using (Packet _packet = new Packet((int)ClientPackets.clientSendAnimationTrigger))
        {
            _packet.Write((int)ClientPackets.clientSendAnimationTrigger);

            _packet.Write(trigger);
            _packet.Write(duration);
            _packet.Write(magnitude);
            SendTcpData(_packet);
        }
    }

    public static void RequestToDamageOpponentsHealth(int damage)
    {
        using (Packet _packet = new Packet((int)ClientPackets.requestToDamageOpponentsHealth))
        {
            _packet.Write((int)ClientPackets.requestToDamageOpponentsHealth);

            _packet.Write(damage);
            SendTcpData(_packet);
        }
    }

    public static void SendStatusEffectData(StatusEffect.StatusEffectType statusEffectIdentifier, int duration, int ownership, bool endThisStatusEffect)
    {
        using (Packet _packet = new Packet((int)ClientPackets.clientSendStatusEffectData))
        {
            _packet.Write((int)ClientPackets.clientSendStatusEffectData);

            _packet.Write(((int)statusEffectIdentifier));
            _packet.Write(duration);
            _packet.Write(ownership);
            _packet.Write(endThisStatusEffect);

            SendTcpData(_packet);
        }
    }

    public static void UpdatePlayerCurrentPostition(int x, int y, int hoveredOverGPsCount) // Pass this a vector 2's x and y
    {
        using (Packet _packet = new Packet((int)ClientPackets.updatePlayerCurrentPosition))
        {
            _packet.Write((int)ClientPackets.updatePlayerCurrentPosition);

            _packet.Write(x);
            _packet.Write(y);
            _packet.Write(hoveredOverGPsCount);

            SendTcpData(_packet);
        }
    }

    public static void OverrideOppositePlayersPostition(int x, int y) // Pass this a vector 2's x and y
    {
        using (Packet _packet = new Packet((int)ClientPackets.overrideOppositePlayersPos))
        {
            _packet.Write((int)ClientPackets.overrideOppositePlayersPos);

            _packet.Write(x);
            _packet.Write(y);

            SendTcpData(_packet);
        }
    }

    public static void SendWinnerStatus(bool hasWonTheMatch)
    {
        using (Packet _packet = new Packet((int)ClientPackets.hasWonTheMatch))
        {
            _packet.Write((int)ClientPackets.hasWonTheMatch);

            _packet.Write(hasWonTheMatch);
            SendTcpData(_packet);
        }
    }

    public static void SendNetworkedMethodIndex(int abilityIndex, int methodIndex)
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendNetworkedMethodIndex))
        {
            _packet.Write((int)ClientPackets.sendNetworkedMethodIndex);

            _packet.Write(abilityIndex);
            _packet.Write(methodIndex);

            SendTcpData(_packet);
        }
    }

    public static void SendStoredMomentumValue(int storedMomentum)
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendStoredMomentumValue))
        {
            _packet.Write((int)ClientPackets.sendStoredMomentumValue);

            _packet.Write(storedMomentum);
            SendTcpData(_packet);
        }
    }
    #endregion
}
