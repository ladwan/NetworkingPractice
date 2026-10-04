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
                _packet.Write(Protocol.Version);

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

        public static void SyncTimers(int matchId, int _currentTime)
        {
            using (Packet _packet = new Packet((int)ServerPackets.syncTimers))
            {
                _packet.Write(_currentTime);

                SendTcpDataToAllMatchPlayers(matchId, _packet);
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

        public static void StartMatch(int matchId)
        {
            using (Packet _packet = new Packet((int)ServerPackets.startMatch))
            {
                SendTcpDataToAllMatchPlayers(matchId, _packet);
            }
        }

        // Replaces the old one-sender-per-packet methods (RelayReadyUp, StartTurn, SendDamageToOpponent, etc).
        // Those each re-read and re-wrote the packet's contents, so any change on the client had to be copied here too.
        // This sends the bytes exactly as the client wrote them, only swapping in the ID the opponent listens for.
        public static void RelayRaw(int _toClient, ServerPackets _packetId, byte[] _payload)
        {
            using (Packet _packet = new Packet((int)_packetId))
            {
                _packet.Write(_payload);

                SendTcpData(_toClient, _packet);
            }
        }

    }
}
