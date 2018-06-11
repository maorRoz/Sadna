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
using SadnaSrc.MarketRecovery;
using SadnaSrc.PolicyComponent;
using SadnaSrc.StoreCenter;

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]
    public class ViewPoliciesTest
    {
        private Mock<IMarketBackUpDB> marketDbMocker;
        private Mock<IUserSeller> seller;
        private Mock<IStorePolicyManager> manager;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketBackUpDB>();
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
            ViewPoliciesSlave slave = new ViewPoliciesSlave(seller.Object, manager.Object);
            slave.ViewPolicies();
            Assert.AreEqual((int)ViewStorePolicyStatus.NoAuthority, slave.Answer.Status);
        }


        [TestMethod]
        public void ViewPoliciesSuccess()
        {

            ViewPoliciesSlave slave = new ViewPoliciesSlave(seller.Object, manager.Object);
            slave.ViewPolicies();
            Assert.AreEqual((int)ViewStorePolicyStatus.Success, slave.Answer.Status);

        }

    }
}
