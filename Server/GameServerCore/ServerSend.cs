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

        private static void SendTcpDataToAllMatchPlayers(int matchIndex, Packet _packet)
        {
            var match = Server.matches[matchIndex];

            match.Player1.myClientTcp.SendData(_packet);
            match.Player2.myClientTcp.SendData(_packet);
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

        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTcpData(_toClient, _packet);
            }
        }

        public static void SendInitialMatchDetails(int _toClient, int _matchId, int _playerNumber, string _otherPlayerUsername)
        {
            using (Packet _packet = new Packet((int)ServerPackets.initalMatchDetails))
            {
                _packet.Write(_matchId);
                _packet.Write(_playerNumber);
                _packet.Write(_otherPlayerUsername);
                SendTcpData(_toClient, _packet);
            }
        }

        public static void ServerSendSelectionPacket(int _toClient, int _panelIndex, int _playerIndex, string _otherPlayersName)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendSelectionPacket))
            {
                _packet.Write(_panelIndex);
                _packet.Write(_playerIndex);
                _packet.Write(_otherPlayersName);

                SendTcpData(_toClient, _packet);
            }
        }

        public static void RelayReadyUp(int _toClient, int _signalInt)
        {
            using (Packet _packet = new Packet((int)ServerPackets.relayReadyUp))
            {
                _packet.Write(_signalInt);
                SendTcpData(_toClient, _packet);
            }
        }

        public static void SyncTimers(int matchId, int _currentTime)
        {
            using (Packet _packet = new Packet((int)ServerPackets.syncTimers))
            {
                _packet.Write(_currentTime);

                SendTcpDataToAllMatchPlayers(matchId, _packet);
            }
        }

        public static void SendSegmentedMovementData(int _toClient, int x, int y, int count, bool hasRotations, bool completed)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendSegmentedMovementData))
            {
                Console.WriteLine($"~~[MATCH] Made it to Send Movement");
                _packet.Write(x);
                _packet.Write(y);
                _packet.Write(count);
                _packet.Write(hasRotations);
                _packet.Write(completed);

                SendTcpData(_toClient, _packet);
                Console.WriteLine($"~~[MATCH] Finished Sending Movement");
            }
        }

        public static void SendSegmentedRotationData(int _toClient, float x, float y, float z, float w, int count)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendSegmentedRotationData))
            {
                _packet.Write(x);
                _packet.Write(y);
                _packet.Write(z);
                _packet.Write(w);
                _packet.Write(count);

                SendTcpData(_toClient, _packet);
            }
        }

        public static void ToggleCountdownTimer(int matchId, int _signalInt)
        {

            using (Packet _packet = new Packet((int)ServerPackets.toggleCountdownTimer))
            {
                _packet.Write(_signalInt);

                SendTcpDataToAllMatchPlayers(matchId, _packet);
            }
        }

        public static void StartTurn(int _toClient, int _signalInt)
        {
            using (Packet _packet = new Packet((int)ServerPackets.startTurn))
            {
                _packet.Write(_signalInt);

                SendTcpData(_toClient, _packet);
            }
        }

        public static void ServerSendAnimationTrigger(int _toClient, string trigger, float duration, float magnitude)
        {
            using (Packet _packet = new Packet((int)ServerPackets.serverSendAnimationTrigger))
            {
                _packet.Write(trigger);
                _packet.Write(duration);
                _packet.Write(magnitude);

                SendTcpData(_toClient, _packet);
            }
        }

        public static void SendDamageToOpponent(int _toClient, int _damageInt)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendDamageToOpponent))
            {
                _packet.Write(_damageInt);

                SendTcpData(_toClient, _packet);
            }
        }

        public static void ServerSendStatusEffectData(int _toClient, int _statusEffectIdentifier, int _duration, int _ownership, bool _endThisStatusEffect)
        {
            using (Packet _packet = new Packet((int)ServerPackets.serverSendStatusEffectData))
            {
                _packet.Write(_statusEffectIdentifier);
                _packet.Write(_duration);
                _packet.Write(_ownership);
                _packet.Write(_endThisStatusEffect);

                SendTcpData(_toClient, _packet);
            }
        }

        public static void SendUpdatedPlayerPosition(int _toClient, int x, int y, int hoveredOverGPsCount)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendUpdatedPlayerPosition))
            {
                _packet.Write(x);
                _packet.Write(y);
                _packet.Write(hoveredOverGPsCount);

                SendTcpData(_toClient, _packet);
            }
        }

        public static void SendOverrodePosition(int _toClient, int x, int y)
        {
            using (Packet _packet = new Packet((int)ServerPackets.serverSendOverrodePos))
            {
                _packet.Write(x);
                _packet.Write(y);

                SendTcpData(_toClient, _packet);
            }
        }

        public static void SendWinStatus(int _toClient, bool winStatus)
        {
            using (Packet _packet = new Packet((int)ServerPackets.serverSendWinStatus))
            {
                _packet.Write(winStatus);

                SendTcpData(_toClient, _packet);
            }
        }

        public static void SendNetworkedMethodIndex(int _toClient, int abilityIndex, int methodIndex)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendNetworkedMethodIndex))
            {
                _packet.Write(abilityIndex);
                _packet.Write(methodIndex);

                SendTcpData(_toClient, _packet);
            }
        }

        public static void ServerSendStoredMomentumValue(int _toClient, int _storedMomentum)
        {
            using (Packet _packet = new Packet((int)ServerPackets.serverSendStoredMomentumValue))
            {
                _packet.Write(_storedMomentum);

                SendTcpData(_toClient, _packet);
            }
        }

    }
}
