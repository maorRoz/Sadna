using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.AdminView;
using SadnaSrc.PolicyComponent;

namespace SystemViewTests.UseCaseUnitTest
{
    [TestClass]
    public class RemoveAndViewPoliciesTest
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
        public void NoSystemEnterRemove()
        {
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.RemovePolicy("Product", "#9");
            Assert.AreEqual((int)EditPolicyStatus.NoAuthority, ans.Status);
        }

        [TestMethod]
        public void NoAuthorityRemove()
        {
            userServiceSession.EnterSystem();
            userServiceSession.SignIn("Big Smoke", "123");
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.RemovePolicy("Product", "#9");
            Assert.AreEqual((int)EditPolicyStatus.NoAuthority, ans.Status);
        }

        [TestMethod]
        public void NoPolicyToRemove()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.RemovePolicy("Product", "#9");
            Assert.AreEqual((int)EditPolicyStatus.Success, ans.Status);
            Assert.AreEqual(1, PolicyDL.Instance.GetAllPolicies().Count);

        }

        [TestMethod]
        public void RemoveSimplePolicySuccess()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.RemovePolicy("Category", "WanderlandItems");
            Assert.AreEqual((int)EditPolicyStatus.Success, ans.Status);
            Assert.AreEqual(0, PolicyDL.Instance.GetAllPolicies().Count);

        }

        [TestMethod]
        public void NoSystemEnterView()
        {
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.ViewPolicies();
            Assert.AreEqual((int)ViewPolicyStatus.NoAuthority, ans.Status);
        }

        [TestMethod]
        public void NoAuthorityView()
        {
            userServiceSession.EnterSystem();
            userServiceSession.SignIn("Big Smoke", "123");
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.ViewPolicies();
            Assert.AreEqual((int)ViewPolicyStatus.NoAuthority, ans.Status);
        }

        [TestMethod]
        public void ViewPoliciesSuccesful1()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.ViewPolicies();
            Assert.AreEqual((int)ViewPolicyStatus.Success, ans.Status);
            Assert.AreEqual(3, ((AdminAnswer)ans).ReportList.Length);
        }
        
        [TestCleanup]
        public void AdminTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }

        private void DoSignInToAdmin()
        {
            userServiceSession.EnterSystem();
            userServiceSession.SignIn(adminName, adminPass);
        }
       
    }
}
