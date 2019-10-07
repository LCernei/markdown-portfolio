using System.Text;
using System.Net.Sockets;
using System.Net;

namespace SocketPublisher
{
    public class Publisher
    {
        private Socket _client;
        private EndPoint _remoteEndPoint;
        private int _noOfEventsFired = 0;

        private string _command = "Publish";
        
        public Publisher()
        {
            var serverIPAddress = IPAddress.Parse("127.0.0.1");
            var serverPort = 10002;

            _client = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _remoteEndPoint = new IPEndPoint(serverIPAddress, serverPort);
            
        }
        
        public void SendASingleEvent(string topicName, string eventData)
        {
            string message = _command + "," + topicName + "," + eventData;
            _client.SendTo(Encoding.ASCII.GetBytes(message), _remoteEndPoint);
            _noOfEventsFired++;
        }
    }
}
