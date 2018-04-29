using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketFeed
{
    class FeedQueue : IFeedQueue
    {
        private List<Notification> queue;
        private List<IObserver> observers;
        private IFeedDL _feedDL;

        public FeedQueue(IFeedDL feedDL)
        {
            _feedDL = feedDL;
            queue = new List<Notification>();
            observers = new List<IObserver>();
        }

        public FeedQueue(IFeedDL feedDL, Notification[] notifications)
        {
            _feedDL = feedDL;
            queue = new List<Notification>(notifications);
            observers = new List<IObserver>();
        }


        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Notify()
        {
            foreach (var observer in observers)
            {
                observer.Update();
            }
        }

        public void AddFeed(Notification notification)
        {
            queue.Add(notification);
            _feedDL.SaveNotification(notification);
            Notify();
        }

        public Notification[] GetFeed()
        {
            RefreshQueue();
            return queue.ToArray();
        }

        private void RefreshQueue()
        {
            var freshFeed = new List<Notification>();
            foreach (var notification in queue)
            {
                if (notification.Status.Equals("Unread"))
                {
                    freshFeed.Add(notification);
                }
            }

            queue = freshFeed;
        }
    }
}
