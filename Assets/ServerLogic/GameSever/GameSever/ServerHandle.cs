using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            int _clientIdCheck = _packet.ReadInt();
            string _clientUsername = _packet.ReadString();

            Console.WriteLine($"{Server.connectedClients[_fromClient].myClientTcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}");
            Console.WriteLine($"Welcome {_clientUsername} ..you have no idea how long i've been waiting for you");
            Server.usernames.Add(_fromClient, _clientUsername);
            //ServerSend.SendUsernames(_fromClient, _clientUsername);
            Server.trackerInt++;
            Console.WriteLine($" ---- ---- -- tracker value : {Server.trackerInt}");
            if (Server.trackerInt >= 2)
            {
                Console.WriteLine(Server.usernames.Count);
                ServerSend.SendUsernames(_fromClient, _clientUsername);
                ServerSend.SendUsernames(1, Server.usernames[1]);
            }
            if (_fromClient != _clientIdCheck)
            {
                Console.WriteLine($"Player \"{_clientUsername}\" (ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
            }
            //TODO: Send player into game
        }

        public static void TryMove(int _fromClient, Packet _packet)
        {
            int _movementData = _packet.ReadInt();
            Console.WriteLine($"{Server.usernames[_fromClient]} is trying to move {_movementData} spaces");
            ServerSend.ConfirmMove(_fromClient, _movementData);
            //TODO: Send player into game
        }

        public static void ServerReadSelectionPacket(int _fromClient, Packet _packet)
        {
            int _panelIndex = _packet.ReadInt();
            int _playerIndex = _packet.ReadInt();

            ServerSend.ServerSendSelectionPacket(_fromClient, _panelIndex, _playerIndex);

        }
    }
}
