using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SadnaSrc.Main;

namespace SadnaSrc.MarketFeed
{
    public class Publisher : IPublisher
    {
        private static Publisher _instance;

        public static Publisher Instance => _instance ?? (_instance = new Publisher());

        private readonly Dictionary<int, IFeedQueue> readers;

        public static IFeedDL FeedDl { get;set; }

        private Publisher()
        {
            if (FeedDl == null)
            {
                FeedDl = FeedDL.Instance;
            }
            readers = new Dictionary<int, IFeedQueue>();
            EstablishQueues();
        }

        private void EstablishQueues()
        {
            var userIds = FeedDl.GetUserIds();
            foreach (var userId in userIds)
            {
                readers.Add(userId, new FeedQueue(FeedDl, userId));
            }
        }

        public void NotifyClientBuy(string store,string product)
        {
            var owners = FeedDl.GetStoreOwnersIds(store);
            var soldMessage = product + " has been sold in " + store + "!";
            foreach (var owner in owners)
            {
                Publish(owner,soldMessage);
            }
        }

        public void NotifyLotteryFinish(string lottery, string store, string product)
        {
            var winner = FeedDl.GetLotteryWinner(lottery);
            var losers = FeedDl.GetLotteryLosers(lottery);
            var winnerMessage = "You've won the lottery on " + product + " in " + store + "!";
            var loserMessage = "You've lost the lottery on " + product + " in " + store + "...";

            Publish(winner, winnerMessage);

            foreach (var loser in losers)
            {
                Publish(loser,loserMessage);
            }
        }


        public void NotifyLotteryCanceled(int[] refundedIds)
        {
            var message = "You've been fully refunded on a lottery you were participating on";
            foreach (var refundedId in refundedIds)
            {
                Publish(refundedId,message);
            }

        }

        public void NotifyMessageReceived(int receiver)
        {
            var message = "You've got new message pending in your mailbox!";
            Publish(receiver,message);
        }

        private void Publish(int receiver, string message)
        {
            var newFeed = new Notification(receiver, message);
            var reader = readers[receiver];
            reader.AddFeed(newFeed);
            reader.Notify();
        }

        public void AddFeedQueue(int userId)
        {
            readers.Add(userId,new FeedQueue(FeedDl,userId));
        }

        public IFeedQueue GetFeedQueue(int userId)
        {
            return readers[userId];
        }

        public static void CleanPublisher()
        {
            FeedDl = null;
            _instance = null;
        }
    }
}
