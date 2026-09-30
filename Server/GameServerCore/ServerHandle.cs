using System;

namespace GameServer
{
    class ServerHandle
    {
        private static int desyncedTimersRecived = 0;


        public static void WelcomeReceived(int _fromClient, int _packetId, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _clientUsername = _packet.ReadString();

            // Older clients don't send a version at all, so a missing one counts as 0 (a mismatch) instead of crashing the read
            int _clientProtocolVersion = _packet.UnreadLength() >= sizeof(int) ? _packet.ReadInt() : 0;

            // Checked before matchmaking so a mismatched client never gets paired into a match
            if (_clientProtocolVersion != Protocol.Version)
            {
                Console.WriteLine($"~~~[SERVER] {_clientUsername} (ID: {_fromClient}) is on protocol version {_clientProtocolVersion} but the server is on {Protocol.Version}, disconnecting them !");
                Server.connectedClients[_fromClient].Disconnect();
                return;
            }

            Console.WriteLine($"{Server.connectedClients[_fromClient].myClientTcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}");
            Console.WriteLine($"Welcome {_clientUsername} ..you have no idea how long i've been waiting for you");
            Server.connectedClients[_fromClient].username = _clientUsername;
            Console.WriteLine($"~~~[USERNAME] {_clientUsername} Has been saved at {_fromClient} index");

            if (Server.currentPlayers % 2 == 0)
            {
                Client p1 = Server.connectedClients[Server.currentPlayers - 1];
                Client p2 = Server.connectedClients[Server.currentPlayers];

                Server.CreateMatch(p1, p2);
            }

            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_clientUsername}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
        }


        public static void ServerReadFromClient(int _fromClient, int _packetId, Packet _packet)
        {
            Console.WriteLine($"~~~[SERVER] Server Recieved Welcome Ack from Client: {_fromClient} !");
            Console.WriteLine($"~~~[SERVER] Packet ID: {_packetId} !");

            Client sender = Server.connectedClients[_fromClient];
            Console.WriteLine($"~~~[SERVER] Client null ?: {sender} !");

            if (sender.MatchId == -1)
            {
                Console.WriteLine($"~~~[SERVER] Invalid Match ID !");
                return;
            }


            if (!Server.matches.TryGetValue(sender.MatchId, out Match match))
            {
                Console.WriteLine($"~~~[SERVER] No match found !");
                return;
            }

            match.HandlePacket(_fromClient, _packetId, _packet);
        }
    }
}
