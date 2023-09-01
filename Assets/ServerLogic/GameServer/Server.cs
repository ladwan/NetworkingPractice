using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    class Server
    {
        public static int maxPlayers { get; private set; }
        public static int port { get; private set; }
        public static Dictionary<int, Client> connectedClients = new Dictionary<int, Client>();
        public static Dictionary<int, string> usernames = new Dictionary<int, string>();
        public static int currentPlayers = 0;
        public static int trackerInt = 0;
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener = null; // check this if it doesnt work

        public static void Start(int _maxPlayers, int _port)
        {
            maxPlayers = _maxPlayers;
            port = _port;

            InitializeSeverData();
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);
            Console.WriteLine("Welcome.. your server has finally started ");
            Console.WriteLine($"We're live on Port {port}. ");
        }

        private static void TcpConnectCallback(IAsyncResult result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(result);
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TcpConnectCallback), null);

            Console.WriteLine($"Incoming connection from... {_client.Client.RemoteEndPoint}");
            for (int i = 1; i <= maxPlayers; i++)
            {
                if (connectedClients[i].myClientTcp.socket == null)
                {
                    connectedClients[i].myClientTcp.Connect(_client);
                    Console.WriteLine($"Successfully connected... {_client.Client.RemoteEndPoint}");
                    return;
                }
            }
            Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect, server is full !");
        }

        private static void InitializeSeverData()
        {
            for (int i = 1; i <= maxPlayers; i++)
            {
                connectedClients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                {(int)ClientPackets.updatePlayerCurrentPosition, ServerHandle.ReadUpdatedPlayerPosition },
                {(int)ClientPackets.sendSelectionData, ServerHandle.ServerReadSelectionPacket},
                {(int)ClientPackets.endTurn, ServerHandle.ServerRecieveEndTurnSignal},
                {(int)ClientPackets.sendReadyUp, ServerHandle.ServerRecieveReadyUpSignal},
                {(int)ClientPackets.enterSyncTimerQueue, ServerHandle.ServerRecieveCurrentTime},
                {(int)ClientPackets.requestToDamageOpponentsHealth, ServerHandle.ServerRecieveRequestToDamageOpponent},
                {(int)ClientPackets.clientSendStatusEffectData, ServerHandle.ServerRecieveStatusEffectData},
                {(int)ClientPackets.sendCurrentStatusEffectDuration, ServerHandle.ServerRecieveStatusEffectCurrentDuration},
                {(int)ClientPackets.sendStoredMomentumValue, ServerHandle.ServerRecieveStoredMomentumValue},
            };
            Console.WriteLine("Initialized Packets..");
        }
    }
}
