namespace PubSubServer
{
    public class Server
    {
        public Server()
        {
            HostPublishSubscribeServices();
        }

        private static void HostPublishSubscribeServices()
        {
            SubscriberService subscriberService = new SubscriberService();
            subscriberService.StartSubscriberService();

            PublisherService publisherService = new PublisherService();
            publisherService.StartPublisherService();
        }
    }
}