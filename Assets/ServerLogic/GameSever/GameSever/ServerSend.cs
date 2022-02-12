using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class ServerSend
    {
        private static void SendTcpData(int _toClient, Packet _packet)
        {
            _packet.WriteLength();
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
                _packet.WriteLength();
                Server.connectedClients[2].myClientTcp.SendData(_packet);
            }
            if (_clientToIgnore == 2)
            {
                _packet.WriteLength();
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

        public static void ConfirmMove(int _toClient, int _movementData)
        {
            using (Packet _packet = new Packet((int)ServerPackets.confirmMove))
            {
                _packet.Write(_movementData);
                _packet.Write(_toClient);

                SendTcpData(_toClient, _packet);
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

        public static void ServerSendSelectionPacket(int _playerThatDoesntNeedMsgId, int _panelIndex, int _playerIndex)
        {
            using (Packet _packet = new Packet((int)ServerPackets.sendSelectionPacket))
            {
                _packet.Write(_panelIndex);
                _packet.Write(_playerIndex);

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
    }
}
