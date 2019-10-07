using System;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace SocketSubscriber
{
    public partial class Subscriber
    {
        Socket _client;
        EndPoint _remoteEndPoint;
        byte[] _data;
        int _recv;
        bool _isReceivingStarted = false;

        public Subscriber()
        {
            var serverIPAddress = IPAddress.Parse("127.0.0.1");
            var serverPort = 10001;

            _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _remoteEndPoint = new IPEndPoint(serverIPAddress, serverPort);
        }


        private void ReceiveDataFromServer()
        {
            EndPoint publisherEndPoint = _client.LocalEndPoint;
            while (true)
            {
                _recv = _client.ReceiveFrom(_data, ref publisherEndPoint);
                string msg = Encoding.ASCII.GetString(_data, 0, _recv) + "," + publisherEndPoint.ToString();
                Console.WriteLine(msg);
            }
        }

        public void DoNotListeningToTopic(string topicName)
        {
            if (string.IsNullOrEmpty(topicName))
            {
                return;
            }
            string command = "UnSubscribe";

            string message = command + "," + topicName;
            _client.SendTo(Encoding.ASCII.GetBytes(message), _remoteEndPoint);

        }

        public void ListenToTopic(string topicName)
        {
            string Command = "Subscribe";

            string message = Command + "," + topicName;
            _client.SendTo(Encoding.ASCII.GetBytes(message), _remoteEndPoint);

            if (_isReceivingStarted == false)
            {
                _isReceivingStarted = true;
                _data = new byte[1024];
                Thread th = new Thread(new ThreadStart(ReceiveDataFromServer));
                th.IsBackground = false;
                th.Start();
            }
        }
    }
}
