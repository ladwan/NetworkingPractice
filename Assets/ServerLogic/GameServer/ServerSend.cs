using System;

namespace GameServer
{
    class ServerSend
    {
        private static void SendTcpData(int _toClient, Packet _packet)
        {
            Server.connectedClients[_toClient].myClientTcp.SendData(_packet);
            Console.WriteLine($"The current index in connected clients is: {_toClient}");
        }

        private static void SendTcpDataToAll(Packet _packet)
        {
            for (int i = 1; i <= Server.maxPlayers; i++)
            {
                Server.connectedClients[i].myClientTcp.SendData(_packet);
            }
        }
        private static void SendTcpDataToOppositePlayer(int _clientToIgnore, Packet _packet)
        {
            if (_clientToIgnore == 1)
            {
                Server.connectedClients[2].myClientTcp.SendData(_packet);
            }
            if (_clientToIgnore == 2)
            {
                Server.connectedClients[1].myClientTcp.SendData(_packet);
            }
        }
        public static void SendTcpDataToAll(int _exeptClient, Packet _packet)
        {
            for (int i = 1; i <= Server.maxPlayers; i++)
            {
                if (i != _exeptClient)
                {
                    Server.connectedClients[i].myClientTcp.SendData(_packet);
                }
            }
        }

        public static void Welcome(int _toClient, string _msg, int _totalPlayers)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);
                _packet.Write(_totalPlayers);

                SendTcpData(_toClient, _packet);
            }
        }

        public static void SendUpdatedPlayerPosition(int _playerThatDoesntNeedMsgId, int x, int y)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendUpdatedPlayerPosition))
            {
                _packet.Write(x);
                _packet.Write(y);

                SendTcpDataToOppositePlayer(_playerThatDoesntNeedMsgId, _packet);
            }
        }

        public static void SendTotalPlayerUpdate(int _playerThatDoesntNeedMsgId, int _playerCount)
        {
            using (Packet _packet = new Packet((int)ServerPackets.totalPlayers))
            {
                _packet.Write(_playerCount);

                if (Server.currentPlayers > 1)
                {
                    SendTcpDataToOppositePlayer(_playerThatDoesntNeedMsgId, _packet);
                }
            }
        }

        public static void ServerSendSelectionPacket(int _playerThatDoesntNeedMsgId, int _panelIndex, int _playerIndex, string _otherPlayersName)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendSelectionPacket))
            {
                _packet.Write(_panelIndex);
                _packet.Write(_playerIndex);
                _packet.Write(_otherPlayersName);

                SendTcpDataToOppositePlayer(_playerThatDoesntNeedMsgId, _packet);
            }
        }

        public static void SendUsernames(int _playerThatDoesntNeedMsgId, string _username)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendUsername))
            {
                _packet.Write(_username);

                SendTcpDataToOppositePlayer(_playerThatDoesntNeedMsgId, _packet);
            }
        }

        public static void StartTurn(int _playerThatDoesntNeedMsgId, int _signalInt)
        {
            using (Packet _packet = new Packet((int)ServerPackets.startTurn))
            {
                _packet.Write(_signalInt);

                SendTcpDataToOppositePlayer(_playerThatDoesntNeedMsgId, _packet);
            }
        }

        public static void RelayReadyUp(int _playerThatDoesntNeedMsgId, int _signalInt)
        {
            using (Packet _packet = new Packet((int)ServerPackets.relayReadyUp))
            {
                _packet.Write(_signalInt);

                SendTcpDataToOppositePlayer(_playerThatDoesntNeedMsgId, _packet);
            }
        }

        public static void SyncTimers(int _playerThatDoesntNeedMsgId, int _currentTime)
        {
            using (Packet _packet = new Packet((int)ServerPackets.syncTimers))
            {
                _packet.Write(_currentTime);

                SendTcpDataToOppositePlayer(1, _packet);
                SendTcpDataToOppositePlayer(2, _packet);
                Console.WriteLine("~ ~ ~ Sync Sent ~ ~ ~");
            }
        }

        public static void ToggleCountdownTimer(int _playerThatDoesntNeedMsgId, int _signalInt)
        {
            using (Packet _packet = new Packet((int)ServerPackets.toggleCountdownTimer))
            {
                _packet.Write(_signalInt);

                SendTcpDataToOppositePlayer(1, _packet);
                SendTcpDataToOppositePlayer(2, _packet);

                Console.WriteLine($"~ ~ ~ Toggle Sent {(int)ServerPackets.toggleCountdownTimer} ~ ~ ~");
            }
        }



        public static void ServerSendingOutTestPacket(int _playerThatDoesntNeedMsgId, int _testInt)
        {
            using (Packet _packet = new Packet((int)ServerPackets.serverSendTestPacket))
            {
                _packet.Write(_testInt);

                SendTcpDataToOppositePlayer(1, _packet);
                SendTcpDataToOppositePlayer(2, _packet);
                Console.WriteLine($"~ ~ ~ Test Packet Sent {(int)ServerPackets.serverSendTestPacket} ~ ~ ~");
            }
        }










        public static void SendDamageToOpponent(int _playerThatDoesntNeedMsgId, int _damageInt)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendDamageToOpponent))
            {
                _packet.Write(_damageInt);

                SendTcpDataToOppositePlayer(_playerThatDoesntNeedMsgId, _packet);
            }
        }

        public static void ServerSendStatusEffectData(int _playerThatDoesntNeedMsgId, int _statusEffectIdentifier, int _duration, int _ownership, bool _endThisStatusEffect)
        {
            using (Packet _packet = new Packet((int)ServerPackets.serverSendStatusEffectData))
            {
                _packet.Write(_statusEffectIdentifier);
                _packet.Write(_duration);
                _packet.Write(_ownership);
                _packet.Write(_endThisStatusEffect);

                SendTcpDataToOppositePlayer(_playerThatDoesntNeedMsgId, _packet);
            }
        }

        public static void ServerSendCurrentStatusEffectDuration(int _playerThatDoesntNeedMsgId, int _currentDuration)
        {
            using (Packet _packet = new Packet((int)ServerPackets.serverSendCurrentStatusEffectDuration))
            {
                _packet.Write(_currentDuration);
                SendTcpDataToOppositePlayer(_playerThatDoesntNeedMsgId, _packet);
            }
        }

        public static void ServerSendStoredMomentumValue(int _playerThatDoesntNeedMsgId, int _storedMomentum)
        {
            using (Packet _packet = new Packet((int)ServerPackets.serverSendStoredMomentumValue))
            {
                _packet.Write(_storedMomentum);
                SendTcpDataToOppositePlayer(_playerThatDoesntNeedMsgId, _packet);
            }
        }

        public static void SendOverrodePosition(int _playerThatDoesntNeedMsgId, int x, int y)
        {
            using (Packet _packet = new Packet((int)ServerPackets.serverSendOverrodePos))
            {
                _packet.Write(x);
                _packet.Write(y);

                SendTcpDataToOppositePlayer(_playerThatDoesntNeedMsgId, _packet);
            }
        }
        public static void SendWinStatus(int _playerThatDoesntNeedMsgId, bool winStatus)
        {
            using (Packet _packet = new Packet((int)ServerPackets.serverSendWinStatus))
            {
                _packet.Write(winStatus);

                SendTcpDataToOppositePlayer(_playerThatDoesntNeedMsgId, _packet);
            }
        }

    }
}
