using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketFeed
{
    public class FeedObserver : IObserver
    {
        private int _userId;
        public string SocketId { get; }
        private IListener _server;

        private readonly Publisher publisher;
        public FeedObserver(IListener server,int userId,string socketId)
        {
            _server = server;
            _userId = userId;
            SocketId = socketId;
            publisher = Publisher.Instance;
        }
        public void Update()
        {
            var feedQueue = publisher.GetFeedQueue(_userId);
            var newNotifications = feedQueue.GetPendingFeeds();
            foreach (var notification in newNotifications)
            {
                _server.GetMessage(SocketId, notification.Message);
            }
        }

        public void AttachToQueue()
        {
            var queue = publisher.GetFeedQueue(_userId);
            queue.Attach(this);
        }

        public void DetachFromQueue()
        {
            var feedQueue = publisher.GetFeedQueue(_userId);
            feedQueue.Detach(this);
        }

    }
}
