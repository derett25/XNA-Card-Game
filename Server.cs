using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ShitheadServer
{
    public class Server
    {
        private TcpListener listener;
        private bool running = false;
        private MultiplayerGameState state = new MultiplayerGameState();

        public readonly int BufferSize = 2 * 1024;
        private object semaphore = new object();

        public Server(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void Shutdown()
        {
            running = false;
            state.Shutdown();
            state = null;
        }

        public void Run()
        {
            Console.WriteLine("Started Shithead server...");
            try
            {
                listener.Start();
                new Thread(() =>
                {
                    while (running)
                    {
                        Thread.CurrentThread.IsBackground = true;
                        String command = Console.ReadLine();
                        lock (semaphore)
                        {
                            if (command.Equals("shutdown"))
                            {
                                Console.WriteLine("Shutting down server...");
                                Shutdown();
                            }
                            else if (command.Equals("removegame"))
                            {
                                Console.WriteLine("Removed game...");
                                state.Shutdown();
                                state = null;
                            }
                            else if (command.Equals("initgame"))
                            {
                                Console.WriteLine("Started game...");
                                state = new MultiplayerGameState();
                            }
                            else if (command.Equals("startgame"))
                            {
                                state.StartGame();
                            }
                            else if (command.Equals("restartgame"))
                            {
                                Console.WriteLine("Restarting game...");
                                state.Shutdown();
                                state = new MultiplayerGameState();
                            }
                            else if (command.StartsWith("addbot"))
                            {
                                state.AddBot(command.Split(' ')[1]);
                            }
                            else if (command.Equals("clear"))
                            {
                                Console.Clear();
                            }
                        }
                    }
                }).Start();

                running = true;
                while (running)
                {
                    if (listener.Pending())
                    {
                        HandleNewConnection();
                    }
                    lock (semaphore)
                    {
                        if (state != null)
                        {
                            state.Update();
                        }
                    }
                }
                listener.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not start server - Check if you already have a service listening on this port");
                Thread.Sleep(5000);
            }
        }

        private void HandleNewConnection()
        {
            TcpClient newClient = listener.AcceptTcpClient();
            NetworkStream netStream = newClient.GetStream();

            // Modify the default buffer sizes
            newClient.SendBufferSize = BufferSize;
            newClient.ReceiveBufferSize = BufferSize;

            EndPoint endPoint = newClient.Client.RemoteEndPoint;
            Console.WriteLine("Handling a new client from {0}...", endPoint);

            // Let them identify themselves
            byte[] msgBuffer = new byte[BufferSize];
            int bytesRead = netStream.Read(msgBuffer, 0, msgBuffer.Length);     // Blocks
            if (bytesRead > 0)
            {
                string username = Encoding.UTF8.GetString(msgBuffer, 0, bytesRead);
                if (!state.AddPlayer(username, newClient))
                {
                    Console.WriteLine("{0} was denied access", username);
                    String response = "NOT_OK";
                    byte[] bytesToSend = ASCIIEncoding.ASCII.GetBytes(response);
                    netStream.Write(bytesToSend, 0, bytesToSend.Length);
                }
            }
        }
    }
}
