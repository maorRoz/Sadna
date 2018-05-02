using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketFeed
{
    public interface IPublisher
    {
        IFeedQueue GetFeedQueue(int userId);
        void NotifyClientBuy(string store, string product);
        void NotifyLotteryFinish(string lottery, string store, string product);
        void NotifyLotteryCanceled(int[] refundedIds);
        void NotifyMessageReceived(int receiver);
    }
}
