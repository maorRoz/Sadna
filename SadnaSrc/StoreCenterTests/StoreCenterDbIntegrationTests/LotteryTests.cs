using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCenterTests.StoreCenterDbIntegrationTests
{
    [TestClass]
    public class LotteryTests
    {
        private MarketYard market;
        public StockListItem ProductToDelete;
        private StockSyncher handler; // need to be here
        IUserService userService;

        [TestInitialize]
        public void BuildStore()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;
            handler = StockSyncher.Instance; 
            userService = market.GetUserService();
        }
        [TestMethod]
        public void HasActiveLotteryNoItem()
        {
            bool ans = handler.HasActiveLottery("MONO", "MOMO", 5);
            Assert.IsFalse(ans);

        }
        [TestMethod]
        public void HasActiveLotteryPurchesWayNotLottery()
        {
            bool ans = handler.HasActiveLottery("T", "WhiteRabbit", 5);
            Assert.IsFalse(ans);
        }
        [TestMethod]
        public void HasActiveLotteryNoLotteryFound()
        {
            bool ans = handler.HasActiveLottery("T", "RedQueen", 5);
            Assert.IsFalse(ans);
        }
        [TestMethod]
        public void HasActiveLotteryLotteryIsNotActive()
        {
            bool ans = handler.HasActiveLottery("T", "Time", 5);
            Assert.IsFalse(ans);
        }
        [TestMethod]
        public void HasActiveLotteryLotteryAmountNegative()
        {
            bool ans = handler.HasActiveLottery("T", "The March Hare", -5);
            Assert.IsFalse(ans);
        }
        [TestMethod]
        public void HasActiveLotteryLotteryAmountZero()
        {

            bool ans = handler.HasActiveLottery("T", "The March Hare", 0);
            Assert.IsFalse(ans);
        }
        [TestMethod]
        public void HasActiveLotteryLotteryCannotPurches()
        {
            bool ans = handler.HasActiveLottery("T", "The March Hare", 9999);
            Assert.IsFalse(ans);
        }
        [TestMethod]
        public void HasActiveLotteryLotteryDatesNotLegal()
        {
            bool ans = handler.HasActiveLottery("T", "nonsense", 2);
            Assert.IsFalse(ans);
        }
        [TestMethod]
        public void HasActiveLotteryLotterySuccess()
        {
            bool ans = handler.HasActiveLottery("T", "The March Hare", -5);
            Assert.IsFalse(ans);
        }


        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
