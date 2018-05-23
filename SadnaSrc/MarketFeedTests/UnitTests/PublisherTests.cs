using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.MarketFeed;
using Moq;

namespace MarketFeedTests.UnitTests
{
    [TestClass]
    public class PublisherTests
    {

        private Publisher publisher;
        private Mock<IFeedDL> feedDbMocker;
        private int userId1 = 6001;
        private int userId2 = 6002;
        private int userId3 = 6003;
        private int userId4 = 6004;
        private string storeTest = "PublisherTestStore";
        private string productTest = "";
        private string lotteryTest = "PublisherTestLottery";
        private string soldMessageExpected;
        private string winnerMessageExpected;
        private string loserMessageExpected;
        private string refundMessageExpected;
        private string newMessageExpected;

        [TestInitialize]
        public void PublisherTestBuilder()
        {
            soldMessageExpected = productTest + " has been sold in " + storeTest + "!";
            winnerMessageExpected = "You've won the lottery on " + productTest + " in " + storeTest + "!";
            loserMessageExpected = "You've lost the lottery on " + productTest + " in " + storeTest + "...";
            refundMessageExpected = "You've been fully refunded on a lottery you were participating on";
            newMessageExpected = "You've got new message pending in your mailbox!";
            feedDbMocker = new Mock<IFeedDL>();
            feedDbMocker.Setup(x => x.GetUserIds()).Returns(new[] { userId1, userId2, userId3,userId4 });
            feedDbMocker.Setup(x => x.GetStoreOwnersIds(storeTest)).Returns(new[] { userId1, userId2, userId3 });
            feedDbMocker.Setup(x => x.GetLotteryWinner(lotteryTest)).Returns(userId2);
            feedDbMocker.Setup(x => x.GetLotteryLosers(lotteryTest)).Returns(new []{userId1,userId3});
            Publisher.FeedDl = feedDbMocker.Object;
            publisher = Publisher.Instance;
        }


        [TestMethod]
        public void QueueForEachUserTest()
        {
            try
            {
                publisher.GetFeedQueue(userId1);
                publisher.GetFeedQueue(userId2);
                publisher.GetFeedQueue(userId3);
                publisher.GetFeedQueue(userId4);
            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void NewQueueAddTest()
        {
            var userId5 = 6005;
            try
            {
                publisher.AddFeedQueue(userId5);
                publisher.GetFeedQueue(userId5);

            }
            catch (Exception)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void PublishMessageToEachStoreOwnerTest()
        {
            publisher.NotifyClientBuy(storeTest, productTest);
            CheckOneNewMessageInQueue(userId1, soldMessageExpected);
            CheckOneNewMessageInQueue(userId2, soldMessageExpected);
            CheckOneNewMessageInQueue(userId3, soldMessageExpected);
            CheckQueueIsEmpty(userId4);
        }

        [TestMethod]
        public void PublishMessageAfterLotterySoldOutTest()
        {
            publisher.NotifyLotteryFinish(lotteryTest,storeTest,productTest);
            CheckOneNewMessageInQueue(userId2, winnerMessageExpected);
            CheckOneNewMessageInQueue(userId1, loserMessageExpected);
            CheckOneNewMessageInQueue(userId3, loserMessageExpected);
            CheckQueueIsEmpty(userId4);
        }

        [TestMethod]
        public void PublishMessageAfterLotteryCancelTest()
        {
            publisher.NotifyLotteryCanceled(new []{userId1,userId2,userId3});
            CheckOneNewMessageInQueue(userId1, refundMessageExpected);
            CheckOneNewMessageInQueue(userId2, refundMessageExpected);
            CheckOneNewMessageInQueue(userId3, refundMessageExpected);
            CheckQueueIsEmpty(userId4);
        }

        [TestMethod]
        public void PublishMessageAfterNewChatMessageSentTest()
        {
            publisher.NotifyMessageReceived(userId1);
            CheckOneNewMessageInQueue(userId1, newMessageExpected);
            CheckQueueIsEmpty(userId2);
            CheckQueueIsEmpty(userId3);
            CheckQueueIsEmpty(userId4);
        }

        [TestMethod]
        public void PublishDifferenetMessegesTest()
        {
            publisher.NotifyMessageReceived(userId1);
            publisher.NotifyLotteryCanceled(new[] { userId1, userId2, userId3 });
            publisher.NotifyLotteryFinish(lotteryTest, storeTest, productTest);
            var expected1 = new[] 
            { 
                newMessageExpected,
                refundMessageExpected,
                loserMessageExpected
            };
            var expected2 = new[]
            {
                refundMessageExpected,
                winnerMessageExpected
            };
            var expected3 = new[]
            {
                refundMessageExpected,
                loserMessageExpected
            };
            CheckManyNewMessagesInQueue(userId1, expected1);
            CheckManyNewMessagesInQueue(userId2, expected2);
            CheckManyNewMessagesInQueue(userId3, expected3);
            CheckQueueIsEmpty(userId4);
        }

        [TestCleanup]
        public void CleanPublisher()
        {
            Publisher.CleanPublisher();
        }



        private void CheckQueueIsEmpty(int userId)
        {
            var queue = publisher.GetFeedQueue(userId);
            Assert.AreEqual(0, queue.GetPendingFeeds().Length);
        }
        private void CheckOneNewMessageInQueue(int userId, string expectedMessage)
        {
            var queue = publisher.GetFeedQueue(userId);
            var notification = new Notification(userId, expectedMessage);
            Assert.AreEqual(1, queue.GetPendingFeeds().Length);
            Assert.AreEqual(notification, queue.GetPendingFeeds()[0]);
        }

        private void CheckManyNewMessagesInQueue(int userId, string[] expectedMessage)
        {
            var queue = publisher.GetFeedQueue(userId);
            Assert.AreEqual(expectedMessage.Length, queue.GetPendingFeeds().Length);
            for (int i = 0; i < expectedMessage.Length; i++)
            {
                var expectedNotification = new Notification(userId, expectedMessage[i]);
                Assert.AreEqual(expectedNotification, queue.GetPendingFeeds()[i]);
            }
        }


    }
}
