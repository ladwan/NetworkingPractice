using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;

namespace GameServer
{
    class Server
    {
        public static int maxPlayers { get; private set; }
        public static int port { get; private set; }


        public static Dictionary<int, Match> matches = new Dictionary<int, Match>();
        private static int nextMatchId = 1;


        public static Dictionary<int, Client> connectedClients = new Dictionary<int, Client>();
        public static Dictionary<int, string> usernames = new Dictionary<int, string>();
        public static int currentPlayers = 0;
        public static int trackerInt = 0;
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener = null; // check this if it doesnt work
        private static bool disconnectLogicIsRunning = false;

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
                Console.WriteLine($"~~ Socket : {connectedClients[i].myClientTcp.socket}  ||  Index : {i}");
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
                Console.WriteLine($"~~~ Hey : {i}");
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                {(int)ClientPackets.sendSelectionData, ServerHandle.ServerReadFromClient},
                {(int)ClientPackets.sendReadyUp, ServerHandle.ServerReadFromClient},
                {(int)ClientPackets.enterSyncTimerQueue, ServerHandle.ServerReadFromClient},
                {(int)(ClientPackets.sendSegmentedMovementData),ServerHandle.ServerReadFromClient},
                {(int)(ClientPackets.sendSegmentedRotationData),ServerHandle.ServerReadFromClient},
                {(int)ClientPackets.toggleTimerCountdown, ServerHandle.ServerReadFromClient},
                {(int)ClientPackets.endTurn, ServerHandle.ServerReadFromClient},
                {(int)(ClientPackets.clientSendAnimationTrigger),ServerHandle.ServerReadFromClient},
                {(int)ClientPackets.requestToDamageOpponentsHealth, ServerHandle.ServerReadFromClient},
                {(int)ClientPackets.clientSendStatusEffectData, ServerHandle.ServerReadFromClient},
                {(int)ClientPackets.updatePlayerCurrentPosition, ServerHandle.ServerReadFromClient },
                {(int)ClientPackets.overrideOppositePlayersPos, ServerHandle.ServerReadFromClient},
                {(int)ClientPackets.hasWonTheMatch, ServerHandle.ServerReadFromClient},
                {(int)(ClientPackets.sendNetworkedMethodIndex),ServerHandle.ServerReadFromClient},
                {(int)ClientPackets.sendStoredMomentumValue, ServerHandle.ServerReadFromClient},

            };
            Console.WriteLine("Initialized Packets..");
        }


        public static void CreateMatch(Client player1, Client player2)
        {
            int matchId = nextMatchId++;

            Match match = new Match(matchId, player1, player2);

            matches.Add(matchId, match);

            player1.MatchId = matchId;
            player1.playerNumber = 1;

            player2.MatchId = matchId;
            player2.playerNumber = 2;

            ServerSend.SendInitialMatchDetails(player1.id, matchId, player1.playerNumber, player2.username);
            ServerSend.SendInitialMatchDetails(player2.id, matchId, player2.playerNumber, player1.username);


            Console.WriteLine($"[MATCH CREATED] Match ID {matchId} | P1: {player1.username} | P2: {player2.username}");
            Console.WriteLine($"[MATCH CREATED] {matchId} | P1: {player1.id} | P2: {player2.id}");
            Console.WriteLine($"Active Matches: {matches.Count}");
        }



        public static void DisconnectAllMatchClients(int matchId, Client _client)
        {
            if (!disconnectLogicIsRunning)
            {
                disconnectLogicIsRunning = true;

                if (!matches.TryGetValue(matchId, out Match match))
                {
                    _client.Disconnect();
                    disconnectLogicIsRunning = false;
                    return;
                }


                matches[matchId].Player1.Disconnect();
                matches[matchId].Player2.Disconnect();

                disconnectLogicIsRunning = false;
            }
        }
    }
}
