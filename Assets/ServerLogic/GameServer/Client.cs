﻿using System;
using System.Net;
using System.Net.Sockets;

namespace GameServer
{
    class Client
    {
        public int id;
        public TCP myClientTcp;
        public static int dataBufferSize = 4096;

        public Client(int _clientId)
        {
            id = _clientId;
            myClientTcp = new TCP(id);
        }



        public class TCP
        {
            public TcpClient socket;
            private readonly int id;
            private NetworkStream stream;

            // our network buffer into which we receive raw packets
            private byte[] buffer;
            private int offset, expected;

            public TCP(int _id)
            {
                id = _id;
            }

            public void Connect(TcpClient _socket)
            {

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                BeginReceiveHeader();

                Server.currentPlayers++;
                ServerSend.Welcome(id, "Hey man, belive it or not.. you're now connected!", Server.currentPlayers);
                ServerSend.SendTotalPlayerUpdate(id, Server.currentPlayers);
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

                Console.WriteLine($" ~ ~ ~ ");

                for (int i = 0; i < testBytes.Length; i++)
                {
                    Console.WriteLine($"Network Stream data : {testBytes[i]}");
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
                    Server.connectedClients[id].Disconnect();
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
                    Server.connectedClients[id].Disconnect();
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
                byte[] body = new byte[buffer.Length];
                Array.Copy(buffer, 0, body, 0, body.Length);


                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(body))
                    {
                        int _packetId = packet.ReadInt();
                        Server.packetHandlers[_packetId](id, packet);
                    }
                });


                BeginReceiveHeader();
            }

            public void Disconnect()
            {
                socket.Close();
                stream = null;
                socket = null;
                buffer = null;
                Server.trackerInt--;
                Server.usernames.Remove(id);
                Server.currentPlayers--;
            }
        }

        public void Disconnect()
        {
            Console.WriteLine($"{myClientTcp.socket.Client.RemoteEndPoint} has disconnected ");

            myClientTcp.Disconnect();
        }
    }
}

