using Microsoft.Xna.Framework;
using System;
using System.Net.Sockets;
using System.Text;

namespace ShitheadServer
{
    public class NetworkPlayer : Player
    {
        public TcpClient Client { get; private set; }
        private bool isConnected = true;
        private object semaphore = new object();

        public readonly int BufferSize = 2 * 1024;

        public NetworkPlayer(string name, TcpClient client) : base(name)
        {
            Client = client;
            Client.SendTimeout = 10000;
            Client.ReceiveTimeout = 10000;
        }

        public void Shutdown()
        {
            Client.Close();
            isConnected = false;
        }

        public override PlayerAction HandleInput(GameState state)
        {
            lock (semaphore)
            {
                try
                {
                    byte[] msgBuffer = new byte[BufferSize];
                    NetworkStream netStream = Client.GetStream();
                    if (netStream.DataAvailable)
                    {
                        int bytesRead = netStream.Read(msgBuffer, 0, msgBuffer.Length);     // Blocks
                        if (bytesRead > 0)
                        {
                            string msg = Encoding.UTF8.GetString(msgBuffer, 0, bytesRead);
                            if (msg.StartsWith("PLAYER_ACTION"))
                            {
                                PlayerAction action = MessageParser.ToPlayerAction(msg);
                                return action;
                            }
                            return null;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
            return null;
        }

        public bool IsConnected()
        {
            try
            {
                if (Client != null && Client.Client != null && Client.Client.Connected)
                {
                    if (Client.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (Client.Client.Receive(buff, SocketFlags.Peek) == 0)
                        {
                            // Client disconnected
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public void SendMessage(String message)
        {
            lock (semaphore)
            {
                try
                {
                    NetworkStream netStream = Client.GetStream();
                    byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(message);
                    netStream.Write(bytesToSend, 0, bytesToSend.Length);
                }
                catch (Exception ex)
                {
                    isConnected = false;
                }
            }
        }
    }
}
