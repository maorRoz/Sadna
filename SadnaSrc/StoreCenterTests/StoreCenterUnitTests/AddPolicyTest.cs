using SadnaSrc.StoreCenter;
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

namespace StoreCenterTests.StoreCenterUnitTests
{  
        [TestClass]
        public class AddPolicyTest
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
            public void NoAuthority1()
            {
                seller.Setup(x => x.CanDeclarePurchasePolicy())
                    .Throws(new MarketException((int)PromoteStoreStatus.NoAuthority, ""));
                AddPolicySlave slave = new AddPolicySlave(seller.Object, manager.Object);
                slave.CreatePolicy("StockItem", null, null , "Quantity <=", "5", "");
                Assert.AreEqual((int)EditStorePolicyStatus.NoAuthority, slave.Answer.Status);
            }

            [TestMethod]
            public void NoAuthority2()
            {
                seller.Setup(x => x.CanDeclarePurchasePolicy())
                    .Throws(new MarketException((int)PromoteStoreStatus.NoAuthority, ""));
                AddPolicySlave slave = new AddPolicySlave(seller.Object, manager.Object);
                slave.SaveFullPolicy();
                Assert.AreEqual((int)EditStorePolicyStatus.NoAuthority, slave.Answer.Status);
            }

            [TestMethod]
            public void BadPolicyType()
            {
                AddPolicySlave slave = new AddPolicySlave(seller.Object, manager.Object);
                slave.CreatePolicy("Shit", null,"#9", "Quantity <=", "5", "");
                Assert.AreEqual((int)EditStorePolicyStatus.InvalidPolicyData, slave.Answer.Status);
            }
            [TestMethod]
            public void BadStorePolicyType()
            {
                AddPolicySlave slave = new AddPolicySlave(seller.Object, manager.Object);
                slave.CreatePolicy("Store", "Cluckin Bell", "shit", "Quantity <=", "5", "");
                Assert.AreEqual((int)EditStorePolicyStatus.InvalidPolicyData, slave.Answer.Status);
            }

            [TestMethod]
            public void BadStockPolicyType()
            {
                AddPolicySlave slave = new AddPolicySlave(seller.Object, manager.Object);
                slave.CreatePolicy("StockItem","Cluckin Bell" , null, "Quantity <=", "5", "");
                Assert.AreEqual((int)EditStorePolicyStatus.InvalidPolicyData, slave.Answer.Status);
            }


            [TestMethod]
            public void BadCondition()
            {

                AddPolicySlave slave = new AddPolicySlave(seller.Object, manager.Object);
                slave.CreatePolicy("StockItem", "Cluckin Bell", "#9", "shit", "5", "");
                Assert.AreEqual((int)EditStorePolicyStatus.InvalidPolicyData, slave.Answer.Status);
            }

            [TestMethod]
            public void BadCombo1()
            {

                AddPolicySlave slave = new AddPolicySlave(seller.Object, manager.Object);
                slave.CreatePolicy("StockItem", "Cluckin Bell", "#9", "Quantity <=", "shit", "");
                Assert.AreEqual((int)EditStorePolicyStatus.InvalidPolicyData, slave.Answer.Status);
            }

            [TestMethod]
            public void BadCombo2()
            {

                AddPolicySlave slave = new AddPolicySlave(seller.Object, manager.Object);
                slave.CreatePolicy("StockItem", "Cluckin Bell", "#9", "AND", "5", "");
                Assert.AreEqual((int)EditStorePolicyStatus.InvalidPolicyData, slave.Answer.Status);
            }

            [TestMethod]
            public void BadCombo3()
            {

                AddPolicySlave slave = new AddPolicySlave(seller.Object, manager.Object);
                slave.CreatePolicy("StockItem", "Cluckin Bell", "#9", "AND", "5", null);
                Assert.AreEqual((int)EditStorePolicyStatus.InvalidPolicyData, slave.Answer.Status);
            }

            [TestMethod]
            public void BadCombo4()
            {

                AddPolicySlave slave = new AddPolicySlave(seller.Object, manager.Object);
                slave.CreatePolicy("StockItem", "Cluckin Bell", "#9", "AND", "5", "Shit");
                Assert.AreEqual((int)EditStorePolicyStatus.InvalidPolicyData, slave.Answer.Status);
            }

            [TestMethod]
            public void BadCombo5()
            {

                AddPolicySlave slave = new AddPolicySlave(seller.Object, manager.Object);
                slave.CreatePolicy("Stock Item", "Cluckin Bell", "#9", "AND", "Shit", "5");
                Assert.AreEqual((int)EditStorePolicyStatus.InvalidPolicyData, slave.Answer.Status);
            }


            [TestMethod]
            public void AddStockItemPolicySuccess()
            {

                AddPolicySlave slave = new AddPolicySlave(seller.Object, manager.Object);
                slave.CreatePolicy("StockItem", "Cluckin Bell", "#9", "Quantity <=", "5", "");
                Assert.AreEqual((int)EditStorePolicyStatus.Success, slave.Answer.Status);
            }

            [TestMethod]
            public void AddStorePolicySuccess()
            {

                AddPolicySlave slave = new AddPolicySlave(seller.Object, manager.Object);
                slave.CreatePolicy("Store", "Cluckin Bell", null, "Quantity <=", "5", "");
                Assert.AreEqual((int)EditStorePolicyStatus.Success, slave.Answer.Status);
            }
        

            [TestMethod]
            public void SavePolicySuccess()
            {

                AddPolicySlave slave = new AddPolicySlave(seller.Object, manager.Object);
                slave.CreatePolicy("StockItem", "Cluckin Bell", "#9", "Quantity <=", "5", "");
                slave.SaveFullPolicy();
                Assert.AreEqual((int)EditStorePolicyStatus.Success, slave.Answer.Status);

            }
    }
 }
