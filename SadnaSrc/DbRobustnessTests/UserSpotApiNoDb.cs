using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace DbRobustnessTests
{
    [TestClass]
    public class UserSpotApiNoDb
    {
        private IUserService userService;
        private MarketAnswer answer;
        [TestInitialize]
        public void BuildNoDataTest()
        {
            MarketDB.ToDisable = false;
            MarketDB.Instance.InsertByForce();
            var marketSession = MarketYard.Instance;
            userService = marketSession.GetUserService();
            answer = userService.EnterSystem();
            Assert.AreEqual((int)EnterSystemStatus.Success, answer.Status);
        }
        [TestMethod]
        public void EnterSystemNoDBTest()
        {
            MarketDB.ToDisable = true;
            answer = MarketYard.Instance.GetUserService().EnterSystem();
            Assert.AreEqual((int)EnterSystemStatus.NoDB,answer.Status);
        }

        [TestMethod]
        public void SignUpNoDBTest()
        {
            MarketDB.ToDisable = true;
            answer = userService.SignUp("NoDbUser", "123", "123", "12345678");
            Assert.AreEqual((int)SignUpStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void SignInNoDBTest()
        {
            MarketDB.ToDisable = true;
            answer = userService.SignIn("BigSmoke", "123");
            Assert.AreEqual((int)SignInStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void ViewCartNoDBTest()
        {
            LoginUser();
            answer = userService.ViewCart();
            Assert.AreEqual((int)ViewCartStatus.Success, answer.Status);
        }

        [TestMethod]
        public void EditCartNoDBTest()
        {
            LoginUser();
            answer = userService.EditCartItem("Cluckin Bell", "#9", 5, 5.0);
            Assert.AreEqual((int)EditCartItemStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void RemoveFromCartNoDBTest()
        {
            LoginUser();
            answer = userService.RemoveFromCart("Cluckin Bell", "#9", 5.0);
            Assert.AreEqual((int)EditCartItemStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void ViewUsersNoDBTest()
        {
            LoginUser();
            answer = userService.ViewUsers();
            Assert.AreEqual((int) ViewUsersStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void GetControlledStoreNamesNoDBTest()
        {
            LoginUser();
            answer = userService.GetControlledStoreNames();
            Assert.AreEqual((int)GetControlledStoresStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void GetAllStoresNoDBTest()
        {
            LoginUser();
            answer = userService.GetAllStores();
            Assert.AreEqual((int)ViewStoresStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void GetStoreManagerPoliciesNoDBTest()
        {
            LoginUser();
            answer = userService.GetStoreManagerPolicies("The Red Rock");
            Assert.AreEqual((int)ViewStoresStatus.NoDB, answer.Status);
        }

        [TestMethod]
        public void GetUserDetailsNoDBTest()
        {
            LoginUser();
            answer = userService.GetUserDetails();
            Assert.AreEqual((int)GetUserDetailsStatus.Success, answer.Status);
        }

        [TestCleanup]
        public void CleanUpNoDataTest()
        {
            MarketDB.ToDisable = false;
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }

        private void LoginUser()
        {
            answer = userService.SignIn("Big Smoke", "123");
            Assert.AreEqual((int)SignInStatus.Success, answer.Status);
            MarketDB.ToDisable = true;
        }
    }
}
