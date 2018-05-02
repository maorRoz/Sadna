using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketFeed
{
    public interface IFeedDL
    {
        int[] GetUserIds();
        void SaveUnreadNotification(Notification notification);

        Notification[] GetUnreadNotifications(int userId);

        int[] GetStoreOwnersIds(string store);

        int GetLotteryWinner(string lottery);

        int[] GetLotteryLosers(string lottery);

        void HasBeenRead(string notificationId);
    }
}
