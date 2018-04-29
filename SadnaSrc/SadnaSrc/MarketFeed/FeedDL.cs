using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.MarketFeed
{
    class FeedDL : IFeedDL
    {
        private static FeedDL _instance;

        public static FeedDL Instance => _instance ?? (_instance = new FeedDL());

        private MarketDB dbConnection;
        private FeedDL()
        {
            dbConnection = MarketDB.Instance;
        }

        public Dictionary<int, IFeedQueue> GetReaders()
        {
            throw new NotImplementedException();
        }

        public void SaveNotification(Notification notification)
        {
            throw new NotImplementedException();
        }

        private Notification[] GetNotifications(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
