using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.MarketFeed
{
    public class Publisher
    {
        private static Publisher _instance;

        public static Publisher Instance => _instance ?? (_instance = new Publisher());

        private Dictionary<int, IFeedQueue> readers;

        public static IFeedDL FeedDl { get;set; }

        private Publisher()
        {
            if (FeedDl == null)
            {
                FeedDl = FeedDL.Instance;
            }

            readers = FeedDl.GetReaders();
        }

        public void NotifyClientBuy()
        {

        }

        public void NotifyLotteryFinish()
        {

        }

        public void NotifyLotteryCanceled()
        {

        }

        public void NotifyMessageReceived()
        {

        }

        public void AddFeedQueue(int userId)
        {
            readers.Add(userId,new FeedQueue(FeedDl));
        }

        public IFeedQueue GetFeedQueue(int userId)
        {
            return readers[userId];
        }
    }
}
