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

            switch (packetId)
            {
                case (int)ClientPackets.sendSelectionData:
                    HandleSelectionPacket(fromClient, packet);
                    break;
                case (int)ClientPackets.sendReadyUp:
                    HandleReadyUpSignal(fromClient, packet);
                    break;
                case (int)ClientPackets.enterSyncTimerQueue:
                    HandleRecieveUnsyncedTime(fromClient, packet);
                    break;
                case (int)ClientPackets.sendSegmentedMovementData:
                    HandleSegmentedMovementData(fromClient, packet);
                    break;
                case (int)ClientPackets.sendSegmentedRotationData:
                    HandleSegmentedRotationData(fromClient, packet);
                    break;
                case (int)ClientPackets.toggleTimerCountdown:
                    HandleToggleTimerSignal(fromClient, packet);
                    break;
                case (int)ClientPackets.endTurn:
                    HandleStartTurn(fromClient, packet);
                    break;
                case (int)ClientPackets.clientSendAnimationTrigger:
                    HandleRecieveAnimationTrigger(fromClient, packet);
                    break;
                case (int)ClientPackets.requestToDamageOpponentsHealth:
                    HandleRequestToDamageOpponent(fromClient, packet);
                    break;
                case (int)ClientPackets.clientSendStatusEffectData:
                    HandleStatusEffectData(fromClient, packet);
                    break;
                case (int)ClientPackets.updatePlayerCurrentPosition:
                    HandleUpdatedPlayerPosition(fromClient, packet);
                    break;
                case (int)ClientPackets.overrideOppositePlayersPos:
                    HandleOverrodePosition(fromClient, packet);
                    break;
                case (int)ClientPackets.hasWonTheMatch:
                    HandleWinnerStatus(fromClient, packet);
                    break;
                case (int)ClientPackets.sendNetworkedMethodIndex:
                    HandleNetworkedMethodIndex(fromClient, packet);
                    break;
                case (int)ClientPackets.sendStoredMomentumValue:
                    HandleStoredMomentumValue(fromClient, packet);
                    break;
            }
        }


        public void HandleSelectionPacket(int fromClientId, Packet packet)
        {
            Console.WriteLine($"~~[MATCH] Selection Packet 01");

            //int _packetId = packet.ReadInt(); // We dont use this, its just the int to route the packet 

            int panelIndex = packet.ReadInt();
            int playerIndex = packet.ReadInt();
            Console.WriteLine($"~~[MATCH] Selection Packet 02");
            string playerName = packet.ReadString();

            Client opponent = GetOpponent(fromClientId);

            Console.WriteLine($"~~[MATCH] Selection Packet 03");
            if (opponent == null)
                return;

            Console.WriteLine($"~~[MATCH] Selection Packet 04");

            ServerSend.ServerSendSelectionPacket(opponent.id, panelIndex, playerIndex, playerName);
            Console.WriteLine($"~~[MATCH] Selection Packet 06");
        }

        public void HandleReadyUpSignal(int fromClientId, Packet _packet)
        {
            int _signalInt = _packet.ReadInt();

            Client opponent = GetOpponent(fromClientId);

            if (opponent == null)
                return;

            ServerSend.RelayReadyUp(opponent.id, _signalInt);
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

        public void HandleSegmentedMovementData(int fromClientId, Packet _packet)
        {
            int _x = _packet.ReadInt();
            int _y = _packet.ReadInt();
            int _count = _packet.ReadInt();
            bool _hasRotations = _packet.ReadBool();
            bool _completed = _packet.ReadBool();


            Client opponent = GetOpponent(fromClientId);

            if (opponent == null)
                return;

            ServerSend.SendSegmentedMovementData(opponent.id, _x, _y, _count, _hasRotations, _completed);
        }

        public void HandleSegmentedRotationData(int fromClientId, Packet _packet)
        {
            float _x = _packet.ReadFloat();
            float _y = _packet.ReadFloat();
            float _z = _packet.ReadFloat();
            float _w = _packet.ReadFloat();
            int _count = _packet.ReadInt();


            Client opponent = GetOpponent(fromClientId);

            if (opponent == null)
                return;

            ServerSend.SendSegmentedRotationData(opponent.id, _x, _y, _z, _w, _count);
        }

        public void HandleToggleTimerSignal(int fromClientId, Packet _packet)
        {
            int _signalInt = _packet.ReadInt();


            Client opponent = GetOpponent(fromClientId);

            if (opponent == null)
                return;

            ServerSend.ToggleCountdownTimer(MatchId, _signalInt);
        }

        public  void HandleStartTurn(int fromClientId, Packet _packet)
        {
            int _signalInt = _packet.ReadInt();

            Client opponent = GetOpponent(fromClientId);

            if (opponent == null)
                return;

            ServerSend.StartTurn(opponent.id, _signalInt);
        }

        public void HandleRecieveAnimationTrigger(int fromClientId, Packet _packet)
        {
            string trigger = _packet.ReadString();
            float duration = _packet.ReadFloat();
            float magnitude = _packet.ReadFloat();

            Client opponent = GetOpponent(fromClientId);

            if (opponent == null)
                return;

            ServerSend.ServerSendAnimationTrigger(opponent.id, trigger, duration, magnitude);
        }

        public void HandleRequestToDamageOpponent(int fromClientId, Packet _packet)
        {
            int _damageInt = _packet.ReadInt();

            Client opponent = GetOpponent(fromClientId);

            if (opponent == null)
                return;

            ServerSend.SendDamageToOpponent(opponent.id, _damageInt);
        }

        public void HandleStatusEffectData(int fromClientId, Packet _packet)
        {
            int _statusEffectIdentifier = _packet.ReadInt();
            int _duration = _packet.ReadInt();
            int _ownership = _packet.ReadInt();
            bool _endThisStatusEffect = _packet.ReadBool();

            Client opponent = GetOpponent(fromClientId);

            if (opponent == null)
                return;

            ServerSend.ServerSendStatusEffectData(opponent.id, _statusEffectIdentifier, _duration, _ownership, _endThisStatusEffect);
        }

        public void HandleUpdatedPlayerPosition(int fromClientId, Packet _packet)
        {
            int _playerUpdatedX = _packet.ReadInt();
            int _playerUpdatedY = _packet.ReadInt();
            int _hoveredOverGPsCount = _packet.ReadInt();

            Client opponent = GetOpponent(fromClientId);

            if (opponent == null)
                return;

            ServerSend.SendUpdatedPlayerPosition(opponent.id, _playerUpdatedX, _playerUpdatedY, _hoveredOverGPsCount);
        }

        public void HandleOverrodePosition(int fromClientId, Packet _packet)
        {
            int _playerUpdatedX = _packet.ReadInt();
            int _playerUpdatedY = _packet.ReadInt();

            Client opponent = GetOpponent(fromClientId);

            if (opponent == null)
                return;

            ServerSend.SendOverrodePosition(opponent.id, _playerUpdatedX, _playerUpdatedY);
        }

        public void HandleWinnerStatus(int fromClientId, Packet _packet)
        {
            bool _winnerStatus = _packet.ReadBool();

            Client opponent = GetOpponent(fromClientId);

            if (opponent == null)
                return;

            ServerSend.SendWinStatus(opponent.id, _winnerStatus);
        }

        public void HandleNetworkedMethodIndex(int fromClientId, Packet _packet)
        {
            int _abilityIndex = _packet.ReadInt();
            int _methodIndex = _packet.ReadInt();

            Client opponent = GetOpponent(fromClientId);

            if (opponent == null)
                return;

            ServerSend.SendNetworkedMethodIndex(opponent.id, _abilityIndex, _methodIndex);
        }

        public void HandleStoredMomentumValue(int fromClientId, Packet _packet)
        {
            int _storedMomentum = _packet.ReadInt();

            Client opponent = GetOpponent(fromClientId);

            if (opponent == null)
                return;

            ServerSend.ServerSendStoredMomentumValue(opponent.id, _storedMomentum);
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