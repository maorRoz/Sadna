using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.AdminView;
using SadnaSrc.PolicyComponent;

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
        }

        [TestMethod]
        public void NoSystemEnter()
        {
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.CreatePolicy("Product", "#9", "Quantity >=", "2", "");
            Assert.AreEqual((int)EditPolicyStatus.NoAuthority, ans.Status);

        }

        [TestMethod]
        public void NoAuthority()
        {
            userServiceSession.EnterSystem();
            userServiceSession.SignIn("Big Smoke", "123");
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.CreatePolicy("Product", "#9", "Quantity >=", "2", "");
            Assert.AreEqual((int)EditPolicyStatus.NoAuthority, ans.Status);

        }

        [TestMethod]
        public void BadInput()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.CreatePolicy("Product", null, "Quantity >=", "2", "");
            Assert.AreEqual((int)EditPolicyStatus.InvalidPolicyData, ans.Status);

        }

        [TestMethod]
        public void AddSimpleGlobalPolicyTest()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.CreatePolicy("Global", null, "Quantity >=", "2", "");
            Assert.AreEqual((int)EditPolicyStatus.Success, ans.Status);
            ans = adminServiceSession.SavePolicy();
            Assert.AreEqual((int)EditPolicyStatus.Success, ans.Status);
            Assert.AreEqual("2", marketSession.GetGlobalPolicyManager().GetPolicyData(PolicyType.Global,null)[3]);
            Assert.AreEqual(3, PolicyDL.Instance.GetAllPolicies().Count);

        }

        [TestMethod]
        public void AddSimpleProductPolicyTest()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.CreatePolicy("Product", "#9", "Quantity >=", "2", "");
            Assert.AreEqual((int)EditPolicyStatus.Success, ans.Status);
            ans = adminServiceSession.SavePolicy();
            Assert.AreEqual((int)EditPolicyStatus.Success, ans.Status);
            Assert.AreEqual("2", marketSession.GetGlobalPolicyManager().GetPolicyData(PolicyType.Product, "#9")[3]);
            Assert.AreEqual(3, PolicyDL.Instance.GetAllPolicies().Count);

        }

        [TestMethod]
        public void AddComplexPolicyTest1()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            adminServiceSession.CreatePolicy("Product", "#9", "Quantity >=", "2", "");
            adminServiceSession.CreatePolicy("Product", "#9", "Username =", "Big Smoke", "");
            adminServiceSession.CreatePolicy("Product", "#9", "AND", "0", "1");
            MarketAnswer ans = adminServiceSession.SavePolicy();
            Assert.AreEqual((int)EditPolicyStatus.Success, ans.Status);
            Assert.AreEqual("AND", marketSession.GetGlobalPolicyManager().GetPolicyData(PolicyType.Product, "#9")[1]);
            Assert.AreEqual(3, PolicyDL.Instance.GetAllPolicies().Count);

        }

        [TestMethod]
        public void AddComplexPolicyTest2()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            adminServiceSession.CreatePolicy("Product", "#9", "Quantity >=", "2", "");
            adminServiceSession.CreatePolicy("Product", "#9", "Username =", "Big Smoke", "");
            adminServiceSession.CreatePolicy("Product", "#9", "Quantity <=", "6", "");
            adminServiceSession.CreatePolicy("Product", "#9", "Address =", "Vinewood", "");
            adminServiceSession.CreatePolicy("Product", "#9", "AND", "0", "1");           
            adminServiceSession.CreatePolicy("Product", "#9", "OR", "2", "3");
            adminServiceSession.CreatePolicy("Product", "#9", "AND", "4", "5");
            MarketAnswer ans = adminServiceSession.SavePolicy();
            Assert.AreEqual((int)EditPolicyStatus.Success, ans.Status);
            Assert.AreEqual("AND", marketSession.GetGlobalPolicyManager().GetPolicyData(PolicyType.Product, "#9")[1]);
            Assert.AreEqual(5, marketSession.GetGlobalPolicyManager().GetPolicyData(PolicyType.Product, "#9")[2].Length);
            Assert.AreEqual(3, PolicyDL.Instance.GetAllPolicies().Count);

        }

        [TestCleanup]
        public void AdminTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
            PolicyHandler.Instance.CleanSession();
        }

        private void DoSignInToAdmin()
        {
            userServiceSession.EnterSystem();
            userServiceSession.SignIn(adminName, adminPass);
        }
    }
}
