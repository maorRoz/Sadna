using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Internal;

namespace SadnaSrc.MarketFeed
{
    class FeedQueue : IFeedQueue
    {
        private List<Notification> queue;
        private List<IObserver> observers;
        private IFeedDL _feedDL;

        public FeedQueue(IFeedDL feedDL,int userId)
        {
            _feedDL = feedDL;
            observers = new List<IObserver>();
            queue = new List<Notification>(_feedDL.GetUnreadNotifications(userId));
        }


        public void Attach(IObserver observer)
        {
            observers.Add(observer);
            if (observers.Count == 1)
            {
                Notify();
            }
        }

        public void Notify()
        {
            RefreshQueue();
            if (queue.IsNullOrEmpty())
            {
                return;

            }
            foreach (var observer in observers)
            {
                observer.Update();
            }

            MarkQueueAsRead();

        }

        private void MarkQueueAsRead()
        {
            foreach (var notification in queue)
            {
                _feedDL.HasBeenRead(notification.Id);
                notification.Status = "Read";
            }
        }

        public void AddFeed(Notification notification)
        {
            _feedDL.SaveUnreadNotification(notification);
            queue.Add(notification);
            Notify();
        }

        public Notification[] GetPendingFeeds()
        {
            return queue.ToArray();
        }

        private void RefreshQueue()
        {
            var freshFeed = new List<Notification>();
            foreach (var notification in queue)
            {
                if (notification.Status.Equals("Pending"))
                {
                    freshFeed.Add(notification);
                }
            }

            queue = freshFeed;
        }
    }
}
