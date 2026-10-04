using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ForeverFight.Ui.CharacterSelection;
using ForeverFight.Interactable.Abilities;
using ForeverFight.Networking;
using ForeverFight.GameMechanics.Movement;

public class ClientSend : MonoBehaviour
{
    private static void SendTcpData(Packet _packet)
    {
        Client.localClientInstance.tcp.SendData(_packet);
    }

    // Every packet writes its ID exactly once, through the Packet constructor. Don't write it again inside the packet,
    // the server hands the ID to its handlers and forwards everything after it to the opponent untouched.
    #region Packets
    public static void WelcomeReceived()
    {
        using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            _packet.Write(Client.localClientInstance.localClientId);
            _packet.Write(UiManager.Instance.UsernameInput.text);
            _packet.Write(Protocol.Version);

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

    // Sends a full confirmed move as one packet: waypoint count, total path distance,
    // AP spent, the mover's movement state index, then each waypoint as three
    // floats. waypoints[0] is the mover's start position so the receiver can
    // snap-and-replay drift-free. The movement state (0 = base, 1 = Ire, ...) lets the
    // receiver pick the same movement curve without knowing the mover's buff state.
    // No rotations are sent - facing is derived from waypoint directions identically
    // on both clients.
    public static void SendMovementPath(List<Vector3> waypoints, float totalPathDistance, int apSpent, int movementIndex)
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendMovementPath))
        {
            _packet.Write(waypoints.Count);
            _packet.Write(totalPathDistance);
            _packet.Write(apSpent);
            _packet.Write(movementIndex);

            for (int i = 0; i < waypoints.Count; i++)
            {
                _packet.Write(waypoints[i]);
            }

            SendTcpData(_packet);
        }
    }

    // This is going to be sent to BOTH players, meaning you will send this to yourself!
    public static void ToggleCountdownTimer()
    {
        using (Packet _packet = new Packet((int)ClientPackets.toggleTimerCountdown))
        {
            int signalInt = 1;
            _packet.Write(signalInt);
            SendTcpData(_packet);
        }
    }

    public static void ReadyToStartMatch()
    {
        using (Packet _packet = new Packet((int)ClientPackets.readyToStartMatch))
        {
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

    public static void ClientSendAnimationTrigger(string trigger, float duration, float magnitude)
    {
        using (Packet _packet = new Packet((int)ClientPackets.clientSendAnimationTrigger))
        {
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

    // World-space position snap (was integer grid coords + a dead third field).
    public static void UpdatePlayerPosition(Vector3 position)
    {
        using (Packet _packet = new Packet((int)ClientPackets.updatePlayerCurrentPosition))
        {
            _packet.Write(position);

            SendTcpData(_packet);
        }
    }

    // Forced move (pull / knockback) landing position for the opponent, world-space.
    public static void OverrideOpponentPosition(Vector3 position)
    {
        using (Packet _packet = new Packet((int)ClientPackets.overrideOppositePlayersPos))
        {
            _packet.Write(position);

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

    public static void SendNetworkedMethodIndex(int abilityIndex, int methodIndex)
    {
        using (Packet _packet = new Packet((int)ClientPackets.sendNetworkedMethodIndex))
        {
            _packet.Write(abilityIndex);
            _packet.Write(methodIndex);

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
    #endregion
}
