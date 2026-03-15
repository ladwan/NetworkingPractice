using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client localClientInstance;
    public static int dataBufferSize = 4096;

    public string serverIp = "127.0.0.1";
    public int port = 32887;
    public int localClientId = 0;
    public TCP tcp;

    private bool isConnected = false;
    private delegate void PacketHandler(Packet _packet);
    private static Dictionary<int, PacketHandler> packetHandlers;

    protected void OnApplicationQuit()
    {
        Disconnect();
    }

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
        isConnected = true;
        tcp.Connect();
    }

    public void HandleConnectionLost()
    {
        if (!isConnected)
            return;

        Debug.LogWarning("Connection to server lost!");
        isConnected = false;

        try { tcp?.socket?.Close(); } catch { }

        ThreadManager.ExecuteOnMainThread(() =>
        {
            HandlePlayerDisconnection.ReturnToLobby();
        });
    }

    public class TCP
    {
        public TcpClient socket;
        private readonly int id;
        private NetworkStream stream;
        private byte[] buffer;
        private int offset, expected;

        public TCP()
        {
        }

        public void Connect()
        {
            socket = new TcpClient
            {
                ReceiveBufferSize = dataBufferSize,
                SendBufferSize = dataBufferSize,
            };

            socket.BeginConnect(localClientInstance.serverIp, localClientInstance.port, ConnectCallback, socket);
        }

        public void ConnectCallback(IAsyncResult result)
        {
            try
            {
                socket.EndConnect(result);

                if (!socket.Connected)
                {
                    localClientInstance.HandleConnectionLost();
                    return;
                }

                stream = socket.GetStream();
                BeginReceiveHeader();
            }
            catch
            {
                localClientInstance.HandleConnectionLost();
            }
        }

        public void SendData(Packet _packet)
        {
            try
            {
                byte[] body = _packet.ToArray();
                byte[] header = BitConverter.GetBytes(body.Length);
                byte[] packet = new byte[header.Length + body.Length];

                Array.Copy(header, 0, packet, 0, header.Length);
                Array.Copy(body, 0, packet, header.Length, body.Length);

                stream.BeginWrite(packet, 0, packet.Length, OnSentData, null);
            }
            catch
            {
                localClientInstance.HandleConnectionLost();
            }
        }

        private void OnSentData(IAsyncResult result)
        {
            try
            {
                stream.EndWrite(result);
            }
            catch
            {
                localClientInstance.HandleConnectionLost();
            }
        }

        private void BeginReceiveHeader()
        {
            buffer = new byte[sizeof(int)];
            offset = 0;
            expected = buffer.Length;

            stream.BeginRead(buffer, offset, expected, OnReceiveHeader, null);
        }

        private void OnReceiveHeader(IAsyncResult result)
        {
            try
            {
                int received = stream.EndRead(result);
                if (received <= 0)
                {
                    localClientInstance.HandleConnectionLost();
                    return;
                }

                offset += received;
                if (offset < expected)
                {
                    stream.BeginRead(buffer, offset, expected - offset, OnReceiveHeader, null);
                    return;
                }

                int length = BitConverter.ToInt32(buffer, 0);
                BeginReceiveBody(length);
            }
            catch
            {
                localClientInstance.HandleConnectionLost();
            }
        }

        private void BeginReceiveBody(int length)
        {
            buffer = new byte[length];
            offset = 0;
            expected = buffer.Length;

            stream.BeginRead(buffer, offset, expected, OnReceiveBody, null);
        }

        private void OnReceiveBody(IAsyncResult result)
        {
            try
            {
                int received = stream.EndRead(result);
                if (received <= 0)
                {
                    localClientInstance.HandleConnectionLost();
                    return;
                }

                offset += received;
                if (offset < expected)
                {
                    stream.BeginRead(buffer, offset, expected - offset, OnReceiveBody, null);
                    return;
                }

                byte[] body = new byte[buffer.Length];
                Array.Copy(buffer, 0, body, 0, body.Length);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(body))
                    {
                        int id = packet.ReadInt();
                        packetHandlers[id](packet);
                    }
                });

                BeginReceiveHeader();
            }
            catch
            {
                localClientInstance.HandleConnectionLost();
            }
        }
    }

    private void InitializeClientData()
    {
        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ServerPackets.welcome, ClientHandle.Welcome },
            { (int)ServerPackets.initalMatchDetails, ClientHandle.ReceiveInitialMatchDetails },
            { (int)ServerPackets.sendSelectionPacket, ClientHandle.ReceiveSelectionPacket},
            { (int)ServerPackets.relayReadyUp, ClientHandle.ReceiveReadyUpSignal},
            { (int)ServerPackets.syncTimers, ClientHandle.ReceiveSyncedTimerTime},
            { (int)ServerPackets.sendSegmentedMovementData, ClientHandle.RecieveSegmentedMovementData },
            { (int)ServerPackets.sendSegmentedRotationData, ClientHandle.RecieveSegmentedRotationData },
            { (int)ServerPackets.toggleCountdownTimer, ClientHandle.ReceiveToggleTimerSignal},
            { (int)ServerPackets.startTurn, ClientHandle.ReceiveStartTurnSignal},
            { (int)ServerPackets.serverSendAnimationTrigger, ClientHandle.ReceiveAnimationTrigger },
            { (int)ServerPackets.sendDamageToOpponent, ClientHandle.ReceiveDamage},
            { (int)ServerPackets.serverSendStatusEffectData, ClientHandle.ClientReceiveStatusEffectData},
            { (int)ServerPackets.sendUpdatedPlayerPosition, ClientHandle.RecieveUpdatedPlayerPosition},
            { (int)ServerPackets.serverSendOverrodePos, ClientHandle.RecieveOverrodePosition},
            { (int)ServerPackets.serverSendWinStatus, ClientHandle.RecieveWinStatus},
            { (int)ServerPackets.sendNetworkedMethodIndex, ClientHandle.RecieveNetworkedMethodIndex },
            { (int)ServerPackets.serverSendStoredMomentumValue, ClientHandle.ClientReceiveStoredMomentumValue},
        };
        Debug.Log("Initialized Packets..");
    }

    public IEnumerator CheckServerConnection()
    {
        yield return new WaitForSeconds(5);
        Debug.Log("Unable to connect, server is most likely not online");
    }

    public void Disconnect()
    {
        HandleConnectionLost();
    }
}