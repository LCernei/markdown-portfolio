using System;

namespace SocketPublisher
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("PPP");
            var publisher = new Publisher();
            publisher.SendASingleEvent("liviu", "message");
        }
    }
}