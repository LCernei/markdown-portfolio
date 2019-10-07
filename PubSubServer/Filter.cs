using System;
using System.Collections.Generic;
using System.Net;

namespace PubSubServer
{
    public class Filter  
    {
        private static readonly Dictionary<string, List<EndPoint>> 
            _subscribersList = new Dictionary<string, List<EndPoint>>();

        static public Dictionary<string, List<EndPoint>> SubscribersList
        {
            get { return _subscribersList; }
        }

        public static List<EndPoint> GetSubscribers(string topicName)
        {
            if (SubscribersList.ContainsKey(topicName))
            {
                return SubscribersList[topicName];
            }
            else
                return null;
        }

        public static void AddSubscriber(string topicName, EndPoint subscriberEndPoint)
        {
            if (SubscribersList.ContainsKey(topicName))
            {
                if (!SubscribersList[topicName].Contains(subscriberEndPoint))
                {
                    SubscribersList[topicName].Add(subscriberEndPoint);
                }
            }
            else
            {
                var newSubscribersList = new List<EndPoint> {subscriberEndPoint};
                SubscribersList.Add(topicName, newSubscribersList);
            }
        }

        public static void RemoveSubscriber(string topicName, EndPoint subscriberEndPoint)
        {
            if (SubscribersList.ContainsKey(topicName))
            {
                if (SubscribersList[topicName].Contains(subscriberEndPoint))
                {
                    SubscribersList[topicName].Remove(subscriberEndPoint);
                }
            }
        }
    }
}