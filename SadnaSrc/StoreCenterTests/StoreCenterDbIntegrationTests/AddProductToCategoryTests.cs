using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.StoreCenter;

namespace StoreCenterTests.StoreCenterDbIntegrationTests
{
    [TestClass]
    public class AddProductToCategoryTests
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
        public void AddProductToCategoryWhenStoreNotExists()
        {
            _userService.EnterSystem();
            _userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)_market.GetStoreManagementService(_userService, "storeNotExists");
            MarketAnswer ans = liorSession.AddProductToCategory("WanderlandItems", "BOX");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
        }
        [TestMethod]
        public void AddProductToCategoryWhenHasNoPremmision()
        {
            _userService.EnterSystem();
            _userService.SignIn("Big Smoke", "123");
            StoreManagementService liorSession = (StoreManagementService)_market.GetStoreManagementService(_userService, "X");
            MarketAnswer ans = liorSession.AddProductToCategory("WanderlandItems", "BOX");
            Assert.AreEqual((int)StoreEnum.NoPermission, ans.Status);
        }

        [TestMethod]
        public void AddProductToCategoryWhenCategoryNotExists()
        {
            _userService.EnterSystem();
            _userService.SignIn("Arik1", "123");
            StoreManagementService liorSession =
                (StoreManagementService) _market.GetStoreManagementService(_userService, "T");
            MarketAnswer ans = liorSession.AddProductToCategory("BB", "BOX");
            Assert.AreEqual((int) StoreEnum.CategoryNotExistsInStore, ans.Status);
        }
        [TestMethod]
        public void AddProductToCategoryWhenProductNotExists()
        {
            _userService.EnterSystem();
            _userService.SignIn("Arik1", "123");
            StoreManagementService liorSession =
                (StoreManagementService)_market.GetStoreManagementService(_userService, "T");
            MarketAnswer ans = liorSession.AddProductToCategory("WanderlandItems", "BOX");
            Assert.AreEqual((int)StoreEnum.ProductNotFound, ans.Status);
        }
        [TestMethod]
        public void AddProductToCategoryWhenProductInCategory()
        {
            _userService.EnterSystem();
            _userService.SignIn("Arik1", "123");
            StoreManagementService liorSession =
                (StoreManagementService)_market.GetStoreManagementService(_userService, "T");
            MarketAnswer ans = liorSession.AddProductToCategory("WanderlandItems", "Fraid Egg");
            Assert.AreEqual((int)StoreEnum.ProductAlreadyInCategory, ans.Status);
        }
        [TestMethod] 
        public void AddProductToCategorySuccess()
        {
            _userService.EnterSystem();
            _userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)_market.GetStoreManagementService(_userService, "T");
            MarketAnswer ans = liorSession.AddProductToCategory("WanderlandItems", "OnePunchManPoster");
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
