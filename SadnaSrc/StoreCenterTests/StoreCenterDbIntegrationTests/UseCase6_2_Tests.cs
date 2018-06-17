using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.StoreCenter;

namespace StoreCenterTests.StoreCenterDbIntegrationTests
{
    [TestClass]
    public class UseCase6_2_Tests
    {
        private StoreManagementService storeManagerServiceSession;
        private IUserService userServiceSession;
        private MarketYard marketSession;

        private string ownerName = "CJ";
        private string ownerPass = "123";

        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            userServiceSession = marketSession.GetUserService();
            marketSession.GetStorePolicyManager().SyncWithDB();
        }

        [TestMethod]
        public void NotStoreOwnerTest()
        {
            userServiceSession.EnterSystem();
            userServiceSession.SignIn("Ryder", "123");
            storeManagerServiceSession = (StoreManagementService)marketSession.GetStoreManagementService(userServiceSession,"Cluckin Bell");
            Assert.AreEqual((int)EditStorePolicyStatus.NoAuthority, storeManagerServiceSession.CreatePolicy("StockItem", "Cluckin Bell","#9 Large", "Quantity <=", "5", "0").Status);
        }
        
        [TestMethod]
        public void AddSimplePolicySuccess()
        {
            DoSignInToStoreOwner();
            storeManagerServiceSession = (StoreManagementService)marketSession.GetStoreManagementService(userServiceSession, "Cluckin Bell");
            Assert.AreEqual((int)EditStorePolicyStatus.Success, storeManagerServiceSession.CreatePolicy("StockItem", "Cluckin Bell", "#9 Large", "Quantity <=", "5", "0").Status);
            Assert.AreEqual((int)EditStorePolicyStatus.Success, storeManagerServiceSession.SavePolicy().Status);
        }

        [TestMethod]
        public void AddComplexPolicySuccess1()
        {
            DoSignInToStoreOwner();
            storeManagerServiceSession = (StoreManagementService)marketSession.GetStoreManagementService(userServiceSession, "Cluckin Bell");
            AddStorePolicy();
        }

        [TestMethod]
        public void AddComplexPolicySuccess2()
        {
            DoSignInToStoreOwner();
            storeManagerServiceSession = (StoreManagementService)marketSession.GetStoreManagementService(userServiceSession, "Cluckin Bell");
            AddStockItemPolicy();
        }

        [TestMethod]
        public void ViewPoliciesTest1()
        {
            DoSignInToStoreOwner();
            storeManagerServiceSession = (StoreManagementService)marketSession.GetStoreManagementService(userServiceSession, "Cluckin Bell");
            MarketAnswer ans = storeManagerServiceSession.ViewPolicies();
            Assert.AreEqual((int)ViewStorePolicyStatus.Success, ans.Status);
            Assert.AreEqual(0, ans.ReportList.Length);
        }

        [TestMethod]
        public void ViewPoliciesTest2()
        {
            DoSignInToStoreOwner();
            storeManagerServiceSession = (StoreManagementService)marketSession.GetStoreManagementService(userServiceSession, "Cluckin Bell");
            AddStockItemPolicy();
            MarketAnswer ans = storeManagerServiceSession.ViewPolicies();
            Assert.AreEqual((int)ViewStorePolicyStatus.Success, ans.Status);
            Assert.AreEqual(1, ans.ReportList.Length);
        }

        [TestMethod]
        public void ViewPoliciesTest3()
        {
            DoSignInToStoreOwner();
            storeManagerServiceSession = (StoreManagementService)marketSession.GetStoreManagementService(userServiceSession, "Cluckin Bell");
            AddStockItemPolicy(); 
            AddStorePolicy();
            MarketAnswer ans = storeManagerServiceSession.ViewPolicies();
            Assert.AreEqual((int)ViewStorePolicyStatus.Success, ans.Status);
            Assert.AreEqual(2, ans.ReportList.Length);
        }

        [TestMethod]
        public void RemovePolicy1()
        {
            DoSignInToStoreOwner();
            storeManagerServiceSession = (StoreManagementService)marketSession.GetStoreManagementService(userServiceSession, "Cluckin Bell");
            AddStockItemPolicy();
            AddStorePolicy();
            MarketAnswer ans = storeManagerServiceSession.RemovePolicy("StockItem", "Cluckin Bell","#9 Large");
            Assert.AreEqual((int)EditPolicyStatus.Success, ans.Status);
            ans = storeManagerServiceSession.ViewPolicies();
            Assert.AreEqual(1, ans.ReportList.Length);
        }
       
        [TestCleanup]
        public void AdminTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
            MarketYard.Instance.GetGlobalPolicyManager().CleanSession();
        }

       

        private void AddStockItemPolicy()
        {
            Assert.AreEqual((int)EditStorePolicyStatus.Success, storeManagerServiceSession.CreatePolicy("StockItem", "Cluckin Bell", "#9 Large", "Quantity <=", "5", "0").Status);
            Assert.AreEqual((int)EditStorePolicyStatus.Success, storeManagerServiceSession.CreatePolicy("StockItem", "Cluckin Bell", "#9 Large", "Address =", "Sunnyvale", "0").Status);
            Assert.AreEqual((int)EditStorePolicyStatus.Success, storeManagerServiceSession.CreatePolicy("StockItem", "Cluckin Bell", "#9 Large", "AND", "0", "1").Status);
            Assert.AreEqual((int)EditStorePolicyStatus.Success, storeManagerServiceSession.SavePolicy().Status);
        }

        private void AddStorePolicy()
        {
            Assert.AreEqual((int)EditStorePolicyStatus.Success, storeManagerServiceSession.CreatePolicy("Store", "Cluckin Bell", null, "Quantity <=", "5", "0").Status);
            Assert.AreEqual((int)EditStorePolicyStatus.Success, storeManagerServiceSession.CreatePolicy("Store", "Cluckin Bell", null, "Address =", "Sunnyvale", "0").Status);
            Assert.AreEqual((int)EditStorePolicyStatus.Success, storeManagerServiceSession.CreatePolicy("Store", "Cluckin Bell", null, "AND", "0", "1").Status);
            Assert.AreEqual((int)EditStorePolicyStatus.Success, storeManagerServiceSession.SavePolicy().Status); ;
        }
        

        private void DoSignInToStoreOwner()
        {
            userServiceSession.EnterSystem();
            userServiceSession.SignIn(ownerName, ownerPass);
        }
    }
}
