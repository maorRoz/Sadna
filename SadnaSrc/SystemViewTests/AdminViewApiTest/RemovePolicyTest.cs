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
    public class RemovePolicyTest
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
            RemovePolicySlave slave = new RemovePolicySlave(admin.Object, manager.Object);
            slave.RemovePolicy("Global", null);
            Assert.AreEqual((int)EditPolicyStatus.NoAuthority, slave.Answer.Status);
        }

        [TestMethod]
        public void BadPolicyType()
        {
            RemovePolicySlave slave = new RemovePolicySlave(admin.Object, manager.Object);
            slave.RemovePolicy("shit", null);
            Assert.AreEqual((int)EditPolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }

        [TestMethod]
        public void BadGlobalPolicyType()
        {
            RemovePolicySlave slave = new RemovePolicySlave(admin.Object, manager.Object);
            slave.RemovePolicy("Global", "shit");
            Assert.AreEqual((int)EditPolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }

        [TestMethod]
        public void BadProductPolicyType()
        {
            RemovePolicySlave slave = new RemovePolicySlave(admin.Object, manager.Object);
            slave.RemovePolicy("Product", null);
            Assert.AreEqual((int)EditPolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }

        [TestMethod]
        public void BadCategoryType()
        {
            RemovePolicySlave slave = new RemovePolicySlave(admin.Object, manager.Object);
            slave.RemovePolicy("Category", null);
            Assert.AreEqual((int)EditPolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }

        [TestMethod]
        public void RemovePolicySuccess()
        {

            RemovePolicySlave slave = new RemovePolicySlave(admin.Object, manager.Object);
            slave.RemovePolicy("Category", "Shit");
            Assert.AreEqual((int)EditPolicyStatus.Success, slave.Answer.Status);

        }

    }
}
