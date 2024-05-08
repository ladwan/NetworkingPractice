using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client : MonoBehaviour
{
    //This is the client we want to update

    public static Client localClientInstance;
    public static int dataBufferSize = 4096;

    public string serverIp = "127.0.0.1"; //local host (study this)
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

    public class TCP
    {
        public TcpClient socket;
        private readonly int id;
        private NetworkStream stream;

        // our network buffer into which we receive raw packets
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

            //receiveBuffer = new byte[dataBufferSize];
            socket.BeginConnect(localClientInstance.serverIp, localClientInstance.port, ConnectCallback, socket);
        }

        public void ConnectCallback(IAsyncResult result)
        {
            socket.EndConnect(result);

            if (!socket.Connected)
            {
                return;
            }

            stream = socket.GetStream();
            BeginReceiveHeader();
        }

        public void SendData(Packet _packet)
        {
            byte[] body = _packet.ToArray();
            byte[] header = BitConverter.GetBytes(body.Length);
            byte[] packet = new byte[header.Length + body.Length];

            Array.Copy(header, 0, packet, 0, header.Length);
            Array.Copy(body, 0, packet, header.Length, body.Length);

            stream.BeginWrite(packet, 0, packet.Length, OnSentData, null);

            byte[] testBytes = _packet.ToArray();
            for (int i = 0; i < testBytes.Length; i++)
            {
                Debug.Log($"Network Stream data : {testBytes[i]}");
            }
        }

        private void OnSentData(IAsyncResult result)
        {
            stream.EndWrite(result);
            // NetworkStream.BeginWrite() always writes all data before returning,
            // so no need to check whether there is more data to send (there wont be)
        }

        // our packet header is a single int: |len|
        private void BeginReceiveHeader()
        {
            buffer = new byte[sizeof(int)];
            offset = 0;
            expected = buffer.Length;

            stream.BeginRead(buffer, offset, expected, OnReceiveHeader, null);
        }

        private void OnReceiveHeader(IAsyncResult result)
        {
            int received = stream.EndRead(result);
            if (received <= 0)
            {
                Debug.Log("Received was less than 0  : 0");
                //localClientInstance.Disconnect();
                // disconnect or error
                return;
            }

            offset += received;
            if (offset < expected)
            {
                // there is more data in the header to be read
                stream.BeginRead(buffer, offset, expected - offset, OnReceiveHeader, null);
                return;
            }

            // fully received header, parse it and start receiving body

            int length = BitConverter.ToInt32(buffer, 0); // we only have to read the single length field
            Debug.Log($"Length : {length} ");
            BeginReceiveBody(length);
        }

        // our packet body is the packet id (an int), and some bytes: |id|data...|
        private void BeginReceiveBody(int length)
        {
            buffer = new byte[length];
            offset = 0;
            expected = buffer.Length;

            stream.BeginRead(buffer, offset, expected, OnReceiveBody, null);
        }

        private void OnReceiveBody(IAsyncResult result)
        {
            int received = stream.EndRead(result);
            if (received <= 0)
            {
                Debug.Log("Received was less than 0  :  1");
                //localClientInstance.Disconnect();
                // disconnect or error
                return;
            }

            offset += received;
            if (offset < expected)
            {
                // there is more data in the body to read
                stream.BeginRead(buffer, offset, expected - offset, OnReceiveBody, null);
                return;
            }

            // fully received body, handle the packet and start reading next packet

            for (int i = 0; i < 3; i++)
            {
                Debug.Log($"Buffer data : {buffer[i]}");
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
                //Server.packetHandlers[id](id, packet);
            });


            BeginReceiveHeader();
        }
    }


    /*
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
                    localClientInstance.Disconnect();
                    return;
                }

                byte[] _data = new byte[byteLength];
                Array.Copy(receiveBuffer, _data, byteLength);

                receiveData.Reset(HandleData(_data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch
            {
                Disconnect();
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
                        Debug.Log($"packet id : {packetId}");
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

        private void Disconnect()
        {
            localClientInstance.Disconnect();

            stream = null;
            receiveBuffer = null;
            receiveData = null;
            socket = null;
        }
    }
*/
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
            { (int)ServerPackets.serverSendOverrodePos, ClientHandle.RecieveOverrodePosition},
            { (int)ServerPackets.serverSendWinStatus, ClientHandle.RecieveWinStatus},
            { (int)ServerPackets.toggleCountdownTimer, ClientHandle.ReceiveToggleTimerSignal},
            { (int)ServerPackets.serverSendTestPacket, ClientHandle.ReceiveTestPacket },
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
        if (isConnected)
        {
            isConnected = false;
            tcp.socket.Close();
            ClientInfo.totalPlayersConnected--;

            Debug.Log("Disconnected from Server");
        }
    }
}
