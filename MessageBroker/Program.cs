using System;

namespace PubSubServer
{
    public class Program
    {
        public static void Main()
        {
            var subscriberService = new SubscriberService();
            subscriberService.StartSubscriberService();

            var publisherService = new PublisherService();
            publisherService.StartPublisherService();
        }
    }
}