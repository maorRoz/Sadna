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

        private readonly IPublisher _publisher;
        public FeedObserver(IPublisher publisher,IListener server,int userId,string socketId)
        {
            _server = server;
            _userId = userId;
            SocketId = socketId;
            _publisher = publisher;
            var feedQueue = _publisher.GetFeedQueue(_userId);
            feedQueue.Attach(this);
        }
        public void Update()
        {
            var feedQueue = _publisher.GetFeedQueue(_userId);
            var newNotifications = feedQueue.GetPendingFeeds();
            foreach (var notification in newNotifications)
            {
                _server.GetMessage(SocketId, notification.Message);
            }
        }

        public void DetachFromQueue()
        {
            var feedQueue = _publisher.GetFeedQueue(_userId);
            feedQueue.Detach(this);
        }

    }
}
