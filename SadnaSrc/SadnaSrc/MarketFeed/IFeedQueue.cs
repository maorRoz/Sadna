using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketFeed
{
    public interface IFeedQueue
    {
        void Attach(IObserver observer);
        void Notify();

        void LoadOfflineFeed(Notification[] notifications);

        void AddFeed(Notification notification);

        Notification[] GetPendingFeeds();
    }
}
