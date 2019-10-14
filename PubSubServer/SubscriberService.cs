﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PubSubServer
{
    // receive subscribe and unsubscribe commands
    public class SubscriberService
    {
        public void StartSubscriberService()
        {
            var th = new Thread(HostSubscriberService);
            th.IsBackground = false;
            th.Start();
        }

        private void HostSubscriberService()
        {
            var ipV4 = IPAddress.Parse("127.0.0.1");
            var localEP = new IPEndPoint(ipV4, 10001);
            var server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.Bind(localEP);
            StartListening(server);
        }

        private static void StartListening(Socket server)
        {
            while (true)
            {
                var data = new byte[1024];
                EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
                var recv = server.ReceiveFrom(data, ref remoteEP);
                var receivedMessage = Encoding.ASCII.GetString(data, 0, recv);
                var messageParts = receivedMessage.Trim().Split(",");
                Console.WriteLine(receivedMessage + "|" + remoteEP);
                if (messageParts.Length < 2)
                    return;
                var command = messageParts[0];
                var topic = messageParts[1];
                if (!string.IsNullOrEmpty(command) && !string.IsNullOrEmpty(topic))
                {
                    if (command == "Subscribe")
                        Filter.AddSubscriber(topic, remoteEP);
                    if (command == "UnSubscribe")
                        Filter.RemoveSubscriber(topic, remoteEP);
                }
            }
        }
    }
}
