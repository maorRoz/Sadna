using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace DbRobustnessTests
{
    [TestClass]
    public class AdminViewApiNoDb
    {
        private ISystemAdminService adminService;
        private MarketAnswer answer;
        [TestInitialize]
        public void BuildNoDataTest()
        {
            MarketDB.ToDisable = false;
            MarketDB.Instance.InsertByForce();
            var marketSession = MarketYard.Instance;
            var userService = marketSession.GetUserService();
            answer = userService.EnterSystem();
            Assert.AreEqual((int)EnterSystemStatus.Success, answer.Status);
            answer = userService.SignIn("Arik1", "123");
            Assert.AreEqual((int)SignInStatus.Success, answer.Status);
            adminService = marketSession.GetSystemAdminService(userService);
            MarketDB.ToDisable = true;
        }
        [TestMethod]
        public void RemoveUserNoDBTest()
        {
            answer = adminService.RemoveUser("Big Smoke");
            Assert.AreEqual((int)RemoveUserStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void ViewPurchaseHistoryByUserNoDBTest()
        {
            answer = adminService.ViewPurchaseHistoryByUser("Big Smoke");
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void ViewPurchaseHistoryByStoreNoDBTest()
        {
            answer = adminService.ViewPurchaseHistoryByStore("Clunkin Bell");
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.NoDB, answer.Status);
        }


        [TestMethod]
        public void AddCategoryNoDBTest()
        {
            answer = adminService.AddCategory("NoDB Stuff");
            Assert.AreEqual((int)EditCategoryStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void RemoveCategoryNoDBTest()
        {
            answer = adminService.RemoveCategory("WanderlandItems");
            Assert.AreEqual((int)EditCategoryStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void CreatePolicyNoDBTest()
        {
            answer = adminService.CreatePolicy("Global", null, "Quantity <=", "5", "");
            Assert.AreEqual((int)EditPolicyStatus.Success, answer.Status);
        }

        [TestMethod]
        public void ViewPoliciesNoDBTest()
        {
            answer = adminService.ViewPolicies();
            Assert.AreEqual((int)ViewPolicyStatus.Success, answer.Status);
        }

        [TestMethod]
        public void SavePolicyNoDBTest()
        {
            answer = adminService.SavePolicy();
            Assert.AreEqual((int)EditPolicyStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void ViewLogNODBTest()
        {
            answer = adminService.ViewLog();
            Assert.AreEqual((int)ViewSystemLogStatus.Success, answer.Status);
        }

        [TestMethod]
        public void ViewErrorNODBTest()
        {
            answer = adminService.ViewLog();
            Assert.AreEqual((int)ViewSystemErrorStatus.Success, answer.Status);
        }


        [TestCleanup]
        public void CleanUpNoDataTest()
        {
            MarketDB.ToDisable = false;
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
