using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketFeed
{
    class FeedObserver : IObserver
    {
        private int _userId;
        private string _socketId;
        private IListener _server;
        public FeedObserver(IListener server,int userId,string socketId)
        {
            _server = server;
            _userId = userId;
            _socketId = socketId;
            var queue = Publisher.Instance.GetFeedQueue(_userId);
            queue.Attach(this);
        }
        public void Update()
        {
            var feedQueue = Publisher.Instance.GetFeedQueue(_userId);
            var newNotifications = feedQueue.GetPendingFeeds();
            foreach (var notification in newNotifications)
            {
                _server.GetMessage(_socketId,notification.Message);
            }
        }
    }
}
