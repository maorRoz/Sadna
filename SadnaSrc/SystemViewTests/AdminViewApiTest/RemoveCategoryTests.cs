using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.AdminView;
using SadnaSrc.Main;

namespace SystemViewTests
{
    [TestClass]
    public class RemoveCategoryTests
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
        public void RemoveCategoryWhenCategoryNotExists()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.RemoveCategory("bad item");
            Assert.AreEqual((int)EditCategoryStatus.CategoryNotExistsInSystem, ans.Status);
        }
        [TestMethod]
        public void RemoveCategorySuccess()
        {
            DoSignInToAdmin();
            adminServiceSession = (SystemAdminService)marketSession.GetSystemAdminService(userServiceSession);
            MarketAnswer ans = adminServiceSession.RemoveCategory("WanderlandItems");
            Assert.AreEqual((int) EditCategoryStatus.Success, ans.Status);
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
