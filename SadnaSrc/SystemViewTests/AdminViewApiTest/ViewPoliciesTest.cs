using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.AdminView;
using SadnaSrc.MarketHarmony;
using SadnaSrc.PolicyComponent;

namespace SystemViewTests.AdminViewApiTest
{
    [TestClass]
    public class ViewPoliciesTest
    {
        private Mock<IUserAdmin> admin;
        private Mock<IGlobalPolicyManager> manager;

        [TestInitialize]
        public void MarketBuilder()
        {
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

        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketYard.CleanSession();
        }


    }
}
