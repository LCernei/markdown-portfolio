using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace PubSubServer
{
    class PublisherService
    {

        public void StartPublisherService()
        {
            var th = new Thread(new ThreadStart(HostPublisherService));
            th.IsBackground = false;
            th.Start();
        }

        private void HostPublisherService()
        {
            Console.WriteLine("Host Publisher");
            var ipV4 = IPAddress.Parse("127.0.0.1");// ReturnMachineIP(); if you need machine ip then use this method.            
            var localEP = new IPEndPoint(ipV4, 10002);
            var server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.Bind(localEP);

            StartListening(server);
        }

        private static IPAddress ReturnMachineIP()
        {
            var hostName = Dns.GetHostName();
            var ipEntry = Dns.GetHostEntry(hostName);
            var addr = ipEntry.AddressList;
            IPAddress ipV4 = null;
            foreach (var item in addr)
            {
                if (item.AddressFamily == AddressFamily.InterNetwork)
                {
                    ipV4 = item;
                    break;
                }

            }
            if (ipV4 == null)
            {
                Console.WriteLine("You have no IP of Version 4.Server can not run without it");
                Environment.Exit(-1);
            }
            return ipV4;
        }

        private static void StartListening(Socket server)
        {
            EndPoint remoteEP = new IPEndPoint(IPAddress.Any, 0);
            var recv = 0;
            var data = new byte[1024];
            while (true)
            {
                try
                {
                    recv = 0;
                    data = new byte[1024];
                    recv = server.ReceiveFrom(data, ref remoteEP);
                    var messageSendFromClient = Encoding.ASCII.GetString(data, 0, recv);
                    var messageParts = messageSendFromClient.Split(",".ToCharArray());
                    var command = messageParts[0];
                    var topicName = messageParts[1];
                    if (!string.IsNullOrEmpty(command) && messageParts[0] == "Publish" &&
                        !string.IsNullOrEmpty(topicName))
                    {
                        var eventParts = new List<string>(messageParts);
                        eventParts.RemoveRange(0, 1);
                        var message = MakeCommaSeparatedString(eventParts);
                        var subscriberListForThisTopic = Filter.GetSubscribers(topicName);
                        var workerThreadParameters = new WorkerThreadParameters();
                        workerThreadParameters.Server = server;
                        workerThreadParameters.Message = message;
                        workerThreadParameters.SubscriberListForThisTopic = subscriberListForThisTopic;

                        ThreadPool.QueueUserWorkItem(new WaitCallback(Publish), workerThreadParameters);
                    }
                }
                catch(Exception exception)
                {
                    Console.WriteLine(exception.ToString());
                }
            }
        }

        public static void Publish(object stateInfo)
        {
            var workerThreadParameters = (WorkerThreadParameters)stateInfo;
            var server = workerThreadParameters.Server;
            var message = workerThreadParameters.Message;
            var subscriberListForThisTopic = workerThreadParameters.SubscriberListForThisTopic;
            var messageLength = message.Length;

            if (subscriberListForThisTopic != null)
            {
                foreach (var endPoint in subscriberListForThisTopic)
                {
                    server.SendTo(Encoding.ASCII.GetBytes(message), messageLength, SocketFlags.None, endPoint);

                }
            }
        }

        private static string MakeCommaSeparatedString(IEnumerable<string> eventParts)
        {
            var message = string.Empty;
            foreach (var item in eventParts)
            {
                message = message + item + ",";

            }
            
            if (message.Length != 0)
            {
                message = message.Remove(message.Length - 1, 1);
            }
            return message;
        }
    }
    class WorkerThreadParameters
    {
        public Socket Server { get; set; }

        public string Message { get; set; }

        public List<EndPoint> SubscriberListForThisTopic { get; set; }
    }
}
