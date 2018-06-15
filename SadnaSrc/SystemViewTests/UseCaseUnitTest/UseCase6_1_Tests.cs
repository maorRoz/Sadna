using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace SystemViewTests.UseCaseUnitTest
{
    [TestClass]
    public class UseCase6_1_Tests
    {
        private SystemAdminService adminServiceSession;
        private IUserService userServiceSession;
        private MarketYard marketSession;

        private string adminName = "Arik1";
        private string adminPass = "123";

        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            userServiceSession = marketSession.GetUserService();
            marketSession.GetGlobalPolicyManager().SyncWithDB();
        }

        [TestMethod]
        public void NotSystemAdminTest()
        {
            userServiceSession.EnterSystem();
            userServiceSession.SignIn("Arik2", "123");
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)EditPolicyStatus.NoAuthority, adminServiceSession.CreatePolicy("Global",null,"Quantity <=","5","0").Status);
        }

        [TestMethod]
        public void AddSimplePolicySuccess()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)EditPolicyStatus.Success, adminServiceSession.CreatePolicy("Global", null, "Quantity <=", "5", "0").Status);
            Assert.AreEqual((int)EditPolicyStatus.Success, adminServiceSession.SavePolicy().Status);
        }

        [TestMethod]
        public void AddComplexPolicySuccess1()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)EditPolicyStatus.Success, adminServiceSession.CreatePolicy("Global", null, "Quantity <=", "5", "0").Status);
            Assert.AreEqual((int)EditPolicyStatus.Success, adminServiceSession.CreatePolicy("Global", null, "Address =", "Sunnyvale", "0").Status);
            Assert.AreEqual((int)EditPolicyStatus.Success, adminServiceSession.CreatePolicy("Global", null, "AND", "0", "1").Status);
            Assert.AreEqual((int)EditPolicyStatus.Success, adminServiceSession.SavePolicy().Status);
        }

        [TestMethod]
        public void AddComplexPolicySuccess2()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            Assert.AreEqual((int)EditPolicyStatus.Success, adminServiceSession.CreatePolicy("Product", "Hash", "Quantity <=", "10", "0").Status);
            Assert.AreEqual((int)EditPolicyStatus.Success, adminServiceSession.CreatePolicy("Product", "Hash", "Username =", "Ricky", "0").Status);
            Assert.AreEqual((int)EditPolicyStatus.Success, adminServiceSession.CreatePolicy("Product", "Hash", "AND", "0", "1").Status);
            Assert.AreEqual((int)EditPolicyStatus.Success, adminServiceSession.SavePolicy().Status);
        }

        [TestMethod]
        public void ViewPoliciesTest1()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.ViewPolicies();
            Assert.AreEqual((int)ViewPolicyStatus.Success, ans.Status);
            Assert.AreEqual(1, ans.ReportList.Length);
        }

        [TestMethod]
        public void ViewPoliciesTest2()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            AddGlobalPolicy();
            MarketAnswer ans = adminServiceSession.ViewPolicies();
            Assert.AreEqual((int)ViewPolicyStatus.Success, ans.Status);
            Assert.AreEqual(2, ans.ReportList.Length);
        }

        [TestMethod]
        public void ViewPoliciesTest3()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            AddGlobalPolicy();
            AddProductPolicy();
            MarketAnswer ans = adminServiceSession.ViewPolicies();
            Assert.AreEqual((int)ViewPolicyStatus.Success, ans.Status);
            Assert.AreEqual(3, ans.ReportList.Length);
        }

        [TestMethod]
        public void RemovePolicy1()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            AddGlobalPolicy();
            AddProductPolicy();
            MarketAnswer ans = adminServiceSession.RemovePolicy("Global", null);
            Assert.AreEqual((int)EditPolicyStatus.Success, ans.Status);
            ans = adminServiceSession.ViewPolicies();
            Assert.AreEqual(2, ans.ReportList.Length);
        }

        [TestMethod]
        public void RemovePolicy2()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            AddGlobalPolicy();
            AddProductPolicy();
            AddCategoryPolicy();
            MarketAnswer ans = adminServiceSession.RemovePolicy("Global", null);
            Assert.AreEqual((int)EditPolicyStatus.Success, ans.Status);
            ans = adminServiceSession.RemovePolicy("Product", "#45 With Cheese");
            Assert.AreEqual((int)EditPolicyStatus.Success, ans.Status);
            ans = adminServiceSession.ViewPolicies();
            Assert.AreEqual(2, ans.ReportList.Length);
        }

        [TestCleanup]
        public void AdminTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
            MarketYard.Instance.GetGlobalPolicyManager().CleanSession();
        }

        private void DoSignInToAdmin()
        {
            userServiceSession.EnterSystem();
            userServiceSession.SignIn(adminName, adminPass);
        }

        private void AddGlobalPolicy()
        {
            adminServiceSession.CreatePolicy("Global", null, "Quantity <=", "5", "0");
            adminServiceSession.CreatePolicy("Global", null, "Address =", "Sunnyvale", "0");
            adminServiceSession.CreatePolicy("Global", null, "AND", "0", "1");
            adminServiceSession.SavePolicy();
        }

        private void AddProductPolicy()
        {
            adminServiceSession.CreatePolicy("Product", "Hash", "Quantity <=", "10", "0");
            adminServiceSession.CreatePolicy("Product", "Hash", "Username =", "Ricky", "0");
            adminServiceSession.CreatePolicy("Product", "Hash", "AND", "0", "1");
            adminServiceSession.SavePolicy();
        }

        private void AddCategoryPolicy()
        {
            adminServiceSession.CreatePolicy("Category", "Hash", "Quantity <=", "10", "0");
            adminServiceSession.CreatePolicy("Category", "Hash", "Username =", "Ricky", "0");
            adminServiceSession.CreatePolicy("Category", "Hash", "AND", "0", "1");
            adminServiceSession.SavePolicy();
        }


    }
}
