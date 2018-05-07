using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.AdminView;
using SadnaSrc.Main;

namespace SystemViewTests
{
    [TestClass]
    public class AddCategoryTests
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
        public void AddCategoryWhenCategoryAlreadyExists()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService) marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.AddCategory("WanderlandItems");
            Assert.AreEqual((int)EditCategoryStatus.CategoryAlradyExist, ans.Status);
        }
        [TestMethod]
        public void AddCategorySuccess()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.AddCategory("Good item");
            Assert.AreEqual((int)EditCategoryStatus.Success, ans.Status);
        }


        [TestCleanup]
        public void CleanUpTest()
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
