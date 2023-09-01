using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    //This is the client we want to update

    public static Client localClientInstance;
    public static int dataBufferSize = 4096;

    public string severIp = "127.0.0.1"; //local host (study this)
    public int port = 32887;
    public int localClientId = 0;
    public TCP tcp;

    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;
    private void Awake()
    {
        if (localClientInstance == null)
        {
            localClientInstance = this;
        }
        else if (localClientInstance != this)
        {
            Debug.Log("Instance already exsists, destroying object!");
            Destroy(this);
        }
    }

    private void Start()
    {
        tcp = new TCP();
    }

    public void ConnectToServer()
    {
        InitializeClientData();
        tcp.Connect();
    }

    public class TCP
    {
        public TcpClient socket;

        private NetworkStream stream;
        private Packet receiveData;
        private byte[] receiveBuffer;

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize,
            };

            receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(localClientInstance.severIp, localClientInstance.port, ConnectCallback, socket);

        }

        public void ConnectCallback(IAsyncResult result)
        {
            socket.EndConnect(result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();
            receiveData = new Packet();
            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
        }

        public void SendData(Packet _packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Error sending data to server via TCP: {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                int byteLength = stream.EndRead(result);
                if (byteLength <= 0)
                {
                    //TODO: Disconnect
                    return;
                }

                byte[] _data = new byte[byteLength];
                Array.Copy(receiveBuffer, _data, byteLength);

                receiveData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                //TODO: Disconnect
            }
        }

        private bool HandleData(byte[] _data)
        {
            int _packetLength = 0;
            receiveData.SetBytes(_data);
            if (receiveData.UnreadLength() >= 4)
            {
                _packetLength = receiveData.ReadInt();
                if (_packetLength <= 0)
                {
                    return true;
                }
            }
            while (_packetLength > 0 && _packetLength <= receiveData.UnreadLength())
            {
                byte[] _packetBytes = receiveData.ReadBytes(_packetLength);
                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet _packet = new Packet(_packetBytes))
                    {
                        int packetId = _packet.ReadInt();
                        packetHandlers[packetId](_packet);
                    }
                });

                _packetLength = 0;
                if (receiveData.UnreadLength() >= 4)
                {
                    _packetLength = receiveData.ReadInt();
                    if (_packetLength <= 0)
                    {
                        return true;
                    }
                }
            }
            if (_packetLength <= 1)
            {
                return true;
            }
            return false;
        }
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.sendUpdatedPlayerPosition, ClientHandle.RecieveUpdatedPlayerPosition},
            { (int)ServerPackets.totalPlayers, ClientHandle.ReceiveTotalPlayerUpdate},
            { (int)ServerPackets.sendSelectionPacket, ClientHandle.ReceiveSelectionPacket},
            { (int)ServerPackets.sendUsername, ClientHandle.ReceiveUsername},
            { (int)ServerPackets.startTurn, ClientHandle.ReceiveStartTurnSignal},
            { (int)ServerPackets.relayReadyUp, ClientHandle.ReceiveReadyUpSignal},
            { (int)ServerPackets.syncTimers, ClientHandle.ReceiveSyncedTimerTime},
            { (int)ServerPackets.sendDamageToOpponent, ClientHandle.ReceiveDamage},
            { (int)ServerPackets.serverSendStatusEffectData, ClientHandle.ClientReceiveStatusEffectData},
            { (int)ServerPackets.serverSendCurrentStatusEffectDuration, ClientHandle.ClientReceiveCurrentStatusEffectDuration},
            { (int)ServerPackets.serverSendStoredMomentumValue, ClientHandle.ClientReceiveStoredMomentumValue},
        };
        Debug.Log("Initialized Packets..");
    }

    public IEnumerator CheckServerConnection()
    {
        yield return new WaitForSeconds(5);
        Debug.Log("Unable to connect, server is most likely not online");
    }


}
