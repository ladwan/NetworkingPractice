using System;

namespace GameServer
{
    class ServerHandle
    {
        private static int desyncedTimersRecived = 0;

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

        public static void ReadUpdatedPlayerPosition(int _fromClient, Packet _packet)
        {
            int _playerUpdatedX = _packet.ReadInt();
            int _playerUpdatedY = _packet.ReadInt();

            ServerSend.SendUpdatedPlayerPosition(_fromClient, _playerUpdatedX, _playerUpdatedY);
        }

        public static void ServerReadSelectionPacket(int _fromClient, Packet _packet)
        {
            int _panelIndex = _packet.ReadInt();
            int _playerIndex = _packet.ReadInt();
            string _playerName = _packet.ReadString();

            ServerSend.ServerSendSelectionPacket(_fromClient, _panelIndex, _playerIndex, _playerName);
        }

        public static void ServerRecieveEndTurnSignal(int _fromClient, Packet _packet)
        {
            int _signalInt = _packet.ReadInt();
            ServerSend.StartTurn(_fromClient, _signalInt);
        }

        public static void ServerRecieveReadyUpSignal(int _fromClient, Packet _packet)
        {
            int _signalInt = _packet.ReadInt();
            ServerSend.RelayReadyUp(_fromClient, _signalInt);
        }
        public static void ServerRecieveCurrentTime(int _fromClient, Packet _packet)
        {
            desyncedTimersRecived++;
            if (desyncedTimersRecived >= 2)
            {
                int _currentTime = _packet.ReadInt();
                ServerSend.SyncTimers(_fromClient, _currentTime);
                Console.WriteLine("~ ~ ~ Sync ~ ~ ~");
                desyncedTimersRecived = 0;
            }
        }
        public static void ServerRecieveToggleTimerSignal(int _fromClient, Packet _packet)
        {
            int _signalInt = _packet.ReadInt();
            Console.WriteLine("~ ~ ~ Toggle ~ ~ ~");
            ServerSend.ToggleCountdownTimer(_fromClient, _signalInt);
        }

        public static void ServerRecieveAnimationTrigger(int _fromClient, Packet _packet)
        {
            string trigger = _packet.ReadString();
            ServerSend.ServerSendAnimationTrigger(_fromClient, trigger);
        }

        public static void ServerRecieveRequestToDamageOpponent(int _fromClient, Packet _packet)
        {
            int _damageInt = _packet.ReadInt();
            ServerSend.SendDamageToOpponent(_fromClient, _damageInt);
        }

        public static void ServerRecieveStatusEffectData(int _fromClient, Packet _packet)
        {
            int _statusEffectIdentifier = _packet.ReadInt();
            int _duration = _packet.ReadInt();
            int _ownership = _packet.ReadInt();
            bool _endThisStatusEffect = _packet.ReadBool();
            ServerSend.ServerSendStatusEffectData(_fromClient, _statusEffectIdentifier, _duration, _ownership, _endThisStatusEffect);
        }

        public static void ServerRecieveStatusEffectCurrentDuration(int _fromClient, Packet _packet)
        {
            int _currentDuration = _packet.ReadInt();
            ServerSend.ServerSendCurrentStatusEffectDuration(_fromClient, _currentDuration);
        }

        public static void ServerRecieveStoredMomentumValue(int _fromClient, Packet _packet)
        {
            int _storedMomentum = _packet.ReadInt();
            ServerSend.ServerSendStoredMomentumValue(_fromClient, _storedMomentum);
        }

        public static void ServerRecieveOverrodePosition(int _fromClient, Packet _packet)
        {
            int _playerUpdatedX = _packet.ReadInt();
            int _playerUpdatedY = _packet.ReadInt();

            ServerSend.SendOverrodePosition(_fromClient, _playerUpdatedX, _playerUpdatedY);
        }

        public static void ServerRecieveWinnerStatus(int _fromClient, Packet _packet)
        {
            bool _winnerStatus = _packet.ReadBool();

            ServerSend.SendWinStatus(_fromClient, _winnerStatus);
        }
    }
}
