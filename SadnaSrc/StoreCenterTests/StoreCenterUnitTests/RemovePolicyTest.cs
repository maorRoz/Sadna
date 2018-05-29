using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;
using SadnaSrc.PolicyComponent;
using SadnaSrc.StoreCenter;

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]
    public class RemovePolicyTest
    {
        private Mock<IMarketDB> marketDbMocker;
        private Mock<IUserSeller> seller;
        private Mock<IStorePolicyManager> manager;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            seller = new Mock<IUserSeller>();
            manager = new Mock<IStorePolicyManager>();
        }

        [TestMethod]
        public void NoAuthority()
        {
            seller.Setup(x => x.CanDeclarePurchasePolicy())
                .Throws(new MarketException((int)PromoteStoreStatus.NoAuthority, ""));
            RemovePolicySlave slave = new RemovePolicySlave(seller.Object, manager.Object);
            slave.RemovePolicy("StockItem", "Cluckin Bell","#9");
            Assert.AreEqual((int)EditStorePolicyStatus.NoAuthority, slave.Answer.Status);
        }

        [TestMethod]
        public void BadPolicyType()
        {
            RemovePolicySlave slave = new RemovePolicySlave(seller.Object, manager.Object);
            slave.RemovePolicy("shit", "Cluckin Bell", "#9");
            Assert.AreEqual((int)EditStorePolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }

        [TestMethod]
        public void BadStockPolicyType()
        {
            RemovePolicySlave slave = new RemovePolicySlave(seller.Object, manager.Object);
            slave.RemovePolicy("StockItem", "Cluckin Bell", "");
            Assert.AreEqual((int)EditStorePolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }

        [TestMethod]
        public void BadStorePolicyType()
        {
            RemovePolicySlave slave = new RemovePolicySlave(seller.Object, manager.Object);
            slave.RemovePolicy("Store", "Cluckin Bell", "#9");
            Assert.AreEqual((int)EditStorePolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }
      
        [TestMethod]
        public void RemovePolicySuccess()
        {

            RemovePolicySlave slave = new RemovePolicySlave(seller.Object, manager.Object);
            slave.RemovePolicy("StockItem", "Cluckin Bell", "#9");
            Assert.AreEqual((int)EditStorePolicyStatus.Success, slave.Answer.Status);

        }

    }
}
