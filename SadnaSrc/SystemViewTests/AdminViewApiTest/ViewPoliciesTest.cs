using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.AdminView;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;
using SadnaSrc.MarketRecovery;
using SadnaSrc.PolicyComponent;

namespace SystemViewTests.AdminViewApiTest
{
    [TestClass]
    public class ViewPoliciesTest
    {
        private Mock<IMarketBackUpDB> marketDbMocker;
        private Mock<IUserAdmin> admin;
        private Mock<IGlobalPolicyManager> manager;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketBackUpDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            admin = new Mock<IUserAdmin>();
            manager = new Mock<IGlobalPolicyManager>();
        }

        [TestMethod]
        public void NoAuthority()
        {
            admin.Setup(x => x.ValidateSystemAdmin())
                .Throws(new MarketException((int)ManageMarketSystem.NotSystemAdmin, ""));
            ViewPoliciesSlave slave = new ViewPoliciesSlave(admin.Object, manager.Object);
            slave.ViewPolicies();
            Assert.AreEqual((int)ViewPolicyStatus.NoAuthority, slave.Answer.Status);
        }

      
        [TestMethod]
        public void ViewPoliciesSuccess()
        {

            ViewPoliciesSlave slave = new ViewPoliciesSlave(admin.Object, manager.Object);
            slave.ViewPolicies();
            Assert.AreEqual((int)ViewPolicyStatus.Success, slave.Answer.Status);

        }

    }
}
