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
    public class AddPolicyTest
    {
        private Mock<IUserAdmin> admin;
        private Mock<IGlobalPolicyManager> manager;
        private AddPolicySlave slave;

        [TestInitialize]
        public void MarketBuilder()
        {
            admin = new Mock<IUserAdmin>();
            manager = new Mock<IGlobalPolicyManager>();
        }

        [TestMethod]
        public void AddPolicySuccess()
        {

            AddPolicySlave slave = new AddPolicySlave(admin.Object,manager.Object);
        }
        
        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketYard.CleanSession();
        }


    }
}
