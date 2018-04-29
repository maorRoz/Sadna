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

        public void SaveUnreadNotification(Notification notification)
        {
            throw new NotImplementedException();
        }

        public Notification[] GetUnreadNotifications(int userId)
        {
            throw new NotImplementedException();
        }

        public int[] GetStoreOwnersIds(string store)
        {
            throw new NotImplementedException();
        }

        public int[] GetRefundedIds(string lottery)
        {
            throw new NotImplementedException();
        }

        public int GetLotteryWinner(string lottery)
        {
            throw new NotImplementedException();
        }

        public int[] GetLotteryLosers(string lottery)
        {
            throw new NotImplementedException();
        }

        public void HasBeenRead(string notificationId)
        {
            throw new NotImplementedException();
        }
    }
}
