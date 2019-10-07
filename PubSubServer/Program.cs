using System;

namespace PubSubServer
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("QQQ");
//            var server = new Server();

            var subscriberService = new SubscriberService();
            subscriberService.StartSubscriberService();

            var publisherService = new PublisherService();
            publisherService.StartPublisherService();
        }
    }
}