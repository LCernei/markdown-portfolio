using System;

namespace SocketSubscriber
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("PPP");
            var subscriber = new Subscriber();
            subscriber.ListenToTopic("liviu");
        }
    }
}