using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

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

        public int[] GetUserIds()
        {
            var userIds = new List<int>();
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Users", "SystemID","Name IS NOT NULL"))
            {
                while (dbReader.Read())
                {
                    userIds.Add(dbReader.GetInt32(0));
                }
            }

            return userIds.ToArray();
        }

        public void SaveUnreadNotification(Notification notification)
        {
            dbConnection.InsertTable("Notifications", "NotificationID,Receiver,Message,Status", 
                new[]{"@id","@receiver","@msg","@status"},notification.ToData());
        }

        public Notification[] GetUnreadNotifications(int userId)
        {
            var loadedFeedsList = new List<Notification>();
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("Notifications", "Message", "Receiver = " + userId +" AND Status ='Pending'"))
            {
                while (dbReader.Read())
                {
                    var loadedFeed = new Notification(userId,dbReader.GetString(0));
                    loadedFeedsList.Add(loadedFeed);
                }
            }

            return loadedFeedsList.ToArray();
        }

        public int[] GetStoreOwnersIds(string store)
        {
            var ownersIds = new List<int>();
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("StoreManagerPolicy", "SystemID", "Store = '"+ store +"' AND " +
                                                                                            "Action = 'StoreOwner'"))
            {
                while (dbReader.Read())
                {
                    var owner = dbReader.GetInt32(0);
                    ownersIds.Add(owner);
                }
            }

            return ownersIds.ToArray();
        }

        public int GetLotteryWinner(string lottery)
        {
            using (var dbReader = dbConnection.SelectFromTableWithCondition("LotteryTicket", "UserID","LotteryID =" +
                                                                                 " '"+lottery +"' AND Status ='WINNING'"))
            {
                if (dbReader.Read())
                {
                    return dbReader.GetInt32(0);
                }
            }

            throw new MarketException(MarketError.DbError,"shouldn't get here!");
        }

        public int[] GetLotteryLosers(string lottery)
        {
            var loserIds = new List<int>();
            using (var dbReader = dbConnection.SelectFromTableWithCondition("LotteryTicket", "UserID", "LotteryID =" +
                                                                               " '" + lottery + "' AND Status ='LOSING'"))
            {
                while (dbReader.Read())
                {
                    var loserId = dbReader.GetInt32(0);
                    loserIds.Add(loserId);
                }
            }

            return loserIds.ToArray();
        }

        public void HasBeenRead(string notificationId)
        {
            dbConnection.UpdateTable("Notifications","NotificationID = '"+notificationId +"'",new[]{"Status"},
                new[]{"@status"},new object[]{"Read"});
        }
    }
}
