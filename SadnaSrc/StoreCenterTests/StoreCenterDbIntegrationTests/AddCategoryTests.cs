using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.StoreCenter;

namespace StoreCenterTests.StoreCenterDbIntegrationTests
{
    [TestClass]
    public class AddCategoryTests
    {
        private MarketYard _market;
        IUserService _userService;
        [TestInitialize]
        public void BuildStore()
        {
            MarketDB.Instance.InsertByForce();
            _market = MarketYard.Instance;
            _userService = _market.GetUserService();
        }
        [TestMethod]
        public void AddCategoryWhenStoreNotExists()
        {
            _userService.EnterSystem();
            _userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)_market.GetStoreManagementService(_userService, "storeNotExists");
            MarketAnswer ans = liorSession.AddCategory("Good item");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
        }
        [TestMethod]
        public void AddCategoryWhenHasNoPremmision()
        {
            _userService.EnterSystem();
            _userService.SignIn("Big Smoke", "123");
            StoreManagementService liorSession = (StoreManagementService)_market.GetStoreManagementService(_userService, "X");
            MarketAnswer ans = liorSession.AddCategory("Good item");
            Assert.AreEqual((int)StoreEnum.NoPremmision, ans.Status);
        }
        [TestMethod]
        public void AddCategoryWhenCategoryAlreadyExists()
        {
            _userService.EnterSystem();
            _userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)_market.GetStoreManagementService(_userService, "T");
            MarketAnswer ans = liorSession.AddCategory("WanderlandItems");
            Assert.AreEqual((int)StoreEnum.CategoryExistsInStore, ans.Status);
        }
        [TestMethod]
        public void AddCategorySuccess()
        {
            _userService.EnterSystem();
            _userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)_market.GetStoreManagementService(_userService, "T");
            MarketAnswer ans = liorSession.AddCategory("Good item");
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
        }


        [TestCleanup]
        public void CleanUpTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
