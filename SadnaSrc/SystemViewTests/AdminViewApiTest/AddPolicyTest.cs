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
        private Mock<IMarketDB> marketDbMocker;
        private Mock<IUserAdmin> admin;
        private Mock<IGlobalPolicyManager> manager;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            admin = new Mock<IUserAdmin>();
            manager = new Mock<IGlobalPolicyManager>();
        }

        [TestMethod]
        public void NoAuthority1()
        {
            admin.Setup(x => x.ValidateSystemAdmin())
                .Throws(new MarketException((int) ManageMarketSystem.NotSystemAdmin, ""));
            AddPolicySlave slave = new AddPolicySlave(admin.Object, manager.Object);
            slave.CreatePolicy("Global", null, "Quantity <=", "5", "");
            Assert.AreEqual((int)EditPolicyStatus.NoAuthority, slave.Answer.Status);
        }

        [TestMethod]
        public void NoAuthority2()
        {
            admin.Setup(x => x.ValidateSystemAdmin())
                .Throws(new MarketException((int)ManageMarketSystem.NotSystemAdmin, ""));
            AddPolicySlave slave = new AddPolicySlave(admin.Object, manager.Object);
            slave.SaveFullPolicy();
            Assert.AreEqual((int)EditPolicyStatus.NoAuthority, slave.Answer.Status);
        }

        [TestMethod]
        public void BadPolicyType()
        {           
            AddPolicySlave slave = new AddPolicySlave(admin.Object, manager.Object);
            slave.CreatePolicy("Shit", null, "Quantity <=", "5", "");
            Assert.AreEqual((int)EditPolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }
        [TestMethod]
        public void BadGlobalPolicyType()
        {
            AddPolicySlave slave = new AddPolicySlave(admin.Object, manager.Object);
            slave.CreatePolicy("Global", "shit", "Quantity <=", "5", "");
            Assert.AreEqual((int)EditPolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }

        [TestMethod]
        public void BadProductPolicyType()
        {
            AddPolicySlave slave = new AddPolicySlave(admin.Object, manager.Object);
            slave.CreatePolicy("Product", null, "Quantity <=", "5", "");
            Assert.AreEqual((int)EditPolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }

        [TestMethod]
        public void BadCategoryPolicyType()
        {
            AddPolicySlave slave = new AddPolicySlave(admin.Object, manager.Object);
            slave.CreatePolicy("Category", "", "Quantity <=", "5", "");
            Assert.AreEqual((int)EditPolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }

        [TestMethod]
        public void BadCondition()
        {

            AddPolicySlave slave = new AddPolicySlave(admin.Object, manager.Object);
            slave.CreatePolicy("Global", null, "shit", "5", "");
            Assert.AreEqual((int)EditPolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }

        [TestMethod]
        public void BadCombo1()
        {

            AddPolicySlave slave = new AddPolicySlave(admin.Object, manager.Object);
            slave.CreatePolicy("Global", null, "Quantity <=", "shit", "");
            Assert.AreEqual((int)EditPolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }

        [TestMethod]
        public void BadCombo2()
        {

            AddPolicySlave slave = new AddPolicySlave(admin.Object, manager.Object);
            slave.CreatePolicy("Global", null, "AND", "5", "");
            Assert.AreEqual((int)EditPolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }

        [TestMethod]
        public void BadCombo3()
        {

            AddPolicySlave slave = new AddPolicySlave(admin.Object, manager.Object);
            slave.CreatePolicy("Global", null, "AND", "5", null);
            Assert.AreEqual((int)EditPolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }

        [TestMethod]
        public void BadCombo4()
        {

            AddPolicySlave slave = new AddPolicySlave(admin.Object, manager.Object);
            slave.CreatePolicy("Global", null, "AND", "5", "Shit");
            Assert.AreEqual((int)EditPolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }

        [TestMethod]
        public void BadCombo5()
        {

            AddPolicySlave slave = new AddPolicySlave(admin.Object, manager.Object);
            slave.CreatePolicy("Global", null, "AND", "Shit", "5");
            Assert.AreEqual((int)EditPolicyStatus.InvalidPolicyData, slave.Answer.Status);
        }
        

        [TestMethod]
        public void AddGlobalPolicySuccess()
        {

            AddPolicySlave slave = new AddPolicySlave(admin.Object,manager.Object);
            slave.CreatePolicy("Global",null,"Quantity <=","5","");
            Assert.AreEqual((int)EditPolicyStatus.Success,slave.Answer.Status);
        }

        [TestMethod]
        public void AddProductPolicySuccess()
        {

            AddPolicySlave slave = new AddPolicySlave(admin.Object, manager.Object);
            slave.CreatePolicy("Product", "Shit", "Username =", "Shitty", "");
            Assert.AreEqual((int)EditPolicyStatus.Success, slave.Answer.Status);
        }

        [TestMethod]
        public void AddCategoryPolicySuccess()
        {

            AddPolicySlave slave = new AddPolicySlave(admin.Object, manager.Object);
            slave.CreatePolicy("Category", "Shitty Products", "Quantity <=", "5", "");
            Assert.AreEqual((int)EditPolicyStatus.Success, slave.Answer.Status);

        }

        [TestMethod]
        public void SavePolicySuccess()
        {

            AddPolicySlave slave = new AddPolicySlave(admin.Object, manager.Object);
            slave.CreatePolicy("Category", "Shitty Products", "Quantity <=", "5", "");
            slave.SaveFullPolicy();
            Assert.AreEqual((int)EditPolicyStatus.Success, slave.Answer.Status);

        }
    }
}
