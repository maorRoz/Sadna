using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketFeed
{
    public interface IFeedDL
    {
        Dictionary<int, IFeedQueue> GetReaders();
        void SaveUnreadNotification(Notification notification);

        Notification[] GetUnreadNotifications(int userId);

        int[] GetStoreOwnersIds(string store);

        int[] GetRefundedIds(string lottery);

        int GetLotteryWinner(string lottery);

        int[] GetLotteryLosers(string lottery);

        void HasBeenRead(string notificationId);
    }
}
