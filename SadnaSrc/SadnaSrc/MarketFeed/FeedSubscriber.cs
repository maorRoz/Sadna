using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.MarketFeed
{
    public static class FeedSubscriber
    {
        private static List<FeedObserver> observers = new List<FeedObserver>();

        public static void SubscribeSocket(IListener webSocketServer, int userId,string socket)
        {
            var observer = new FeedObserver(MarketYard.Instance.GetPublisher(), webSocketServer, userId, socket);
            observers.Add(observer);
        }

        public static void UnSubscribeSocket(string socket)
        {
            foreach (var observer in observers)
            {
                if (observer.SocketId == socket)
                {
                    observer.DetachFromQueue();
                }
            }
        }

    }
}
