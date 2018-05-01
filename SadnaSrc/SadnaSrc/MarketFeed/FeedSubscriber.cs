using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketFeed
{
    public static class FeedSubscriber
    {
        private static List<FeedObserver> observers;

        public static void SubscribeSocket(IListener webSocketServer, int userId,string socket)
        {
            var observer = new FeedObserver(webSocketServer, userId, socket);
            observer.AttachToQueue();
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
