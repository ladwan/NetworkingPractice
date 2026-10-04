using System;

namespace GameServer
{
    public class Match
    {
        public int MatchId { get; private set; }
        public Client Player1 { get; private set; }
        public Client Player2 { get; private set; }
        public bool IsActive { get; private set; }

        public Match(int matchId, Client p1, Client p2)
        {
            MatchId = matchId;

            Player1 = p1;
            Player2 = p2;

            Player1.MatchId = matchId;
            Player2.MatchId = matchId;

            IsActive = true;

            Console.WriteLine($"Match {MatchId} created. Players: {Player1.id} vs {Player2.id}");
        }

        private int desyncedTimersRecived = 0;
        private bool player1ReadyToStart = false;
        private bool player2ReadyToStart = false;
        private bool hasMatchStarted = false;


        // =============================
        // Core Lookup
        // =============================

        public Client GetOpponent(int clientId)
        {
            if (Player1.id == clientId)
                return Player2;

            if (Player2.id == clientId)
                return Player1;

            return null;
        }

        public bool ContainsPlayer(int clientId)
        {
            return Player1.id == clientId || Player2.id == clientId;
        }

        // =============================
        // Sending Utilities
        // =============================

        public void SendToPlayer(int clientId, Packet packet)
        {
            Client target = null;

            if (Player1.id == clientId)
                target = Player1;
            else if (Player2.id == clientId)
                target = Player2;

            if (target == null)
            {
                Console.WriteLine($"Match {MatchId}: Tried sending to invalid client {clientId}");
                return;
            }

            target.myClientTcp.SendData(packet);
        }

        public void SendToOpponent(int senderId, Packet packet)
        {
            Client opponent = GetOpponent(senderId);

            if (opponent == null)
            {
                Console.WriteLine($"Match {MatchId}: Opponent not found for {senderId}");
                return;
            }

            opponent.myClientTcp.SendData(packet);
        }

        public void SendToBoth(Packet packet)
        {
            Player1.myClientTcp.SendData(packet);
            Player2.myClientTcp.SendData(packet);
        }

        // =============================
        // Packet Routing Entry Point
        // =============================

        public void HandlePacket(int fromClient, int packetId, Packet packet)
        {
            Console.WriteLine($"~~[MATCH] Match {MatchId}: Received packet {packetId} from {fromClient}");

            // Most packets are gameplay the server doesn't need to understand, so they go straight to the opponent untouched
            if (Protocol.RelayedPackets.TryGetValue((ClientPackets)packetId, out ServerPackets relayedPacketId))
            {
                RelayToOpponent(fromClient, relayedPacketId, packet);
                return;
            }

            // Only packets the server actually has to make a decision about are handled here
            switch (packetId)
            {
                case (int)ClientPackets.enterSyncTimerQueue:
                    HandleRecieveUnsyncedTime(fromClient, packet);
                    break;
                case (int)ClientPackets.toggleTimerCountdown:
                    HandleToggleTimerSignal(fromClient, packet);
                    break;
                case (int)ClientPackets.readyToStartMatch:
                    HandleReadyToStartMatch(fromClient);
                    break;
            }
        }


        // Stands in for the old per-packet handlers (HandleReadyUpSignal, HandleStartTurn, etc).
        // Those read each field out just to write the same fields back, which meant the server had to
        // change every time a packet's contents did. Forwarding the leftover bytes skips that entirely.
        public void RelayToOpponent(int fromClientId, ServerPackets relayedPacketId, Packet _packet)
        {
            Client opponent = GetOpponent(fromClientId);

            if (opponent == null)
                return;

            ServerSend.RelayRaw(opponent.id, relayedPacketId, _packet.ReadRemainingBytes());
        }

        public void HandleRecieveUnsyncedTime(int fromClientId, Packet _packet)
        {
            desyncedTimersRecived++;

            if (desyncedTimersRecived >= 2)
            {
                int _currentTime = _packet.ReadInt();

                ServerSend.SyncTimers(MatchId, _currentTime);
                Console.WriteLine("~ ~ ~ Sync ~ ~ ~");
                desyncedTimersRecived = 0;
            }
        }

        // Players finish loading the combat scene at different times, so neither one starts until both have checked in.
        // Tracked per player (not with a counter) so the same player sending it twice can't start the match alone.
        public void HandleReadyToStartMatch(int fromClientId)
        {
            if (Player1.id == fromClientId)
                player1ReadyToStart = true;
            else if (Player2.id == fromClientId)
                player2ReadyToStart = true;

            if (!player1ReadyToStart || !player2ReadyToStart || hasMatchStarted)
                return;

            hasMatchStarted = true;
            ServerSend.StartMatch(MatchId);
            Console.WriteLine($"Match {MatchId}: Both players ready, starting match");
        }

        public void HandleToggleTimerSignal(int fromClientId, Packet _packet)
        {
            int _signalInt = _packet.ReadInt();


            Client opponent = GetOpponent(fromClientId);

            if (opponent == null)
                return;

            ServerSend.ToggleCountdownTimer(MatchId, _signalInt);
        }


        // =============================
        // Disconnect Handling
        // =============================

        public void HandleDisconnect(int disconnectedClientId)
        {
            if (!IsActive)
                return;

            Client opponent = GetOpponent(disconnectedClientId);

            if (opponent != null)
            {
                try
                {
/*                    using (Packet packet = new Packet((int)ServerPackets.opponentDisconnected))
                    {
                        packet.Write("Opponent disconnected.");
                        opponent.myClientTcp.SendData(packet);
                    }*/
                }
                catch
                {
                    // Ignore send failure
                }

                opponent.MatchId = -1;
            }

            Player1.MatchId = -1;
            Player2.MatchId = -1;

            IsActive = false;

            Console.WriteLine($"Match {MatchId} cleaned up.");
        }

        // =============================
        // Cleanup
        // =============================

        public void Cleanup()
        {
            if (!IsActive)
                return;

            Player1.MatchId = -1;
            Player2.MatchId = -1;

            IsActive = false;

            Console.WriteLine($"Match {MatchId} manually cleaned up.");
        }
    }
}