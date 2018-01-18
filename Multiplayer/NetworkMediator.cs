using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace TestProject.Multiplayer
{
    public class NetworkMediator
    {
        private TcpClient client = new TcpClient();

        public NetworkMediator()
        {
        }

        public void Connect(String ip, int port)
        {
            client.Connect(ip, port);
        }

        public void Disconnect()
        {
            client.Close();
        }

        public bool IsConnected()
        {
            try
            {
                if (client != null && client.Client != null && client.Client.Connected)
                {
                    if (client.Client.Poll(0, SelectMode.SelectRead))
                    {
                        byte[] buff = new byte[1];
                        if (client.Client.Receive(buff, SocketFlags.Peek) == 0)
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
            NetworkStream stream = client.GetStream();
            byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(message);
            stream.Write(bytesToSend, 0, bytesToSend.Length);
        }

        public String[] ReceiveMessageBlocking()
        {
            NetworkStream nwStream = client.GetStream();
            byte[] bytesToRead = new byte[client.ReceiveBufferSize];
            int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
            String response = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
            if (!response.Equals("^"))
            {
                return response.Split('^');
            }
            return null;
        }

        public String[] ReceiveMessage()
        {
            NetworkStream nwStream = client.GetStream();
            if (nwStream.DataAvailable)
            {
                byte[] bytesToRead = new byte[client.ReceiveBufferSize];
                int bytesRead = nwStream.Read(bytesToRead, 0, client.ReceiveBufferSize);
                String response = Encoding.ASCII.GetString(bytesToRead, 0, bytesRead);
                if (!response.Equals("^"))
                {
                    return response.Split('^');
                }
            }
            return null;
        }
    }
}
