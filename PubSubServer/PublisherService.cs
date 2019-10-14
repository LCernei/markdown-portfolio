using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PubSubServer
{
    // receive publish commands
    public class PublisherService
    {
        public void StartPublisherService()
        {
            var th = new Thread(HostPublisherService);
            th.IsBackground = false;
            th.Start();
        }

        private void HostPublisherService()
        {
            var ipV4 = IPAddress.Parse("127.0.0.1");
            var localEP = new IPEndPoint(ipV4, 10002);
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
                var messageParts = receivedMessage.Trim().Split(",", 3);
                Console.WriteLine(receivedMessage + "|" + remoteEP);
                if (messageParts.Length < 3)
                    return;
                var command = messageParts[0];
                var topic = messageParts[1];
                var message = topic + "," + messageParts[2];
                if (!string.IsNullOrEmpty(command) && command == "Publish" &&
                    !string.IsNullOrEmpty(topic))
                {
                    var subscribersList = Filter.GetSubscribers(topic);
                    if (subscribersList != null)
                    {
                        foreach (var endPoint in subscribersList)
                        {
                            server.SendTo(Encoding.ASCII.GetBytes(message), message.Length, SocketFlags.None, endPoint);
                        }
                    }
                }
            }
        }
    }
}
