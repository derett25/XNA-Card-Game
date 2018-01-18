using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestProject.Multiplayer
{
    public class NetworkThread
    {
        public Queue<String> ReceivedMessageQueue { get; private set; }
        public Queue<String> MessageQueue { get; private set; }
        private NetworkMediator mediator;
        private bool running = true;
        private object receivedSemaphore = new object();
        private object sendSemaphore = new object();

        public NetworkThread(NetworkMediator nm)
        {
            mediator = nm;
            ReceivedMessageQueue = new Queue<String>();
            MessageQueue = new Queue<String>();
        }

        public void Run()
        {
            while (running)
            {
                try
                {
                    if (mediator.IsConnected())
                    {
                        String[] messages = mediator.ReceiveMessage();
                        if (messages != null)
                        {
                            lock (receivedSemaphore)
                            {
                                foreach (String msg in messages)
                                {
                                    ReceivedMessageQueue.Enqueue(msg);
                                }
                            }
                        }
                        if (MessageQueue.Count > 0)
                        {
                            String message;
                            lock (receivedSemaphore)
                            {
                                message = MessageQueue.Dequeue();
                            }
                            mediator.SendMessage(message);
                        }
                    }
                    else
                    {
                        Screens.ScreenManager.GotoScreen("menu", "Lost connection to server");
                        running = false;
                    }
                }
                catch (Exception ex)
                {
                    Screens.ScreenManager.GotoScreen("menu", ex.Message);
                    running = false;
                }
            }
        }

        public void Stop()
        {
            running = false;
        }

        public String GetLatestMessage()
        {
            lock (receivedSemaphore)
            {
                if (ReceivedMessageQueue.Count > 0)
                {
                    return ReceivedMessageQueue.Dequeue();
                }
                else
                {
                    return null;
                }
            }
        }

        public void SendMessageSameThread(String message)
        {
            mediator.SendMessage(message);
        }

        public void SendMessage(String message)
        {
            if (!MessageQueue.Contains(message))
            {
                MessageQueue.Enqueue(message);
            }
        }
    }
}
