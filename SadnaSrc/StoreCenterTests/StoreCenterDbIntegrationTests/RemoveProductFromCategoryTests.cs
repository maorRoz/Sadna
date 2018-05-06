﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.StoreCenter;

namespace StoreCenterTests.StoreCenterDbIntegrationTests
{
    [TestClass]
    public class RemoveProductFromCategoryTests
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
        public void RemoveProductFromCategoryWhenStoreNotExists()
        {
            _userService.EnterSystem();
            _userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)_market.GetStoreManagementService(_userService, "storeNotExists");
            MarketAnswer ans = liorSession.RemoveProductFromCategory("WanderlandItems", "BOX");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
        }
        [TestMethod]
        public void RemoveProductFromCategoryWhenHasNoPremmision()
        {
            _userService.EnterSystem();
            _userService.SignIn("Big Smoke", "123");
            StoreManagementService liorSession = (StoreManagementService)_market.GetStoreManagementService(_userService, "X");
            MarketAnswer ans = liorSession.RemoveProductFromCategory("WanderlandItems", "BOX");
            Assert.AreEqual((int)StoreEnum.NoPremmision, ans.Status);
        }

        [TestMethod]
        public void AddProductToCategoryWhenCategoryNotExists()
        {
            _userService.EnterSystem();
            _userService.SignIn("Arik1", "123");
            StoreManagementService liorSession =
                (StoreManagementService)_market.GetStoreManagementService(_userService, "T");
            MarketAnswer ans = liorSession.RemoveProductFromCategory("BB", "BOX");
            Assert.AreEqual((int)StoreEnum.CategoryNotExistsInStore, ans.Status);
        }
        [TestMethod]
        public void RemoveProductFromCategoryProductNotExists()
        {
            _userService.EnterSystem();
            _userService.SignIn("Arik1", "123");
            StoreManagementService liorSession =
                (StoreManagementService)_market.GetStoreManagementService(_userService, "T");
            MarketAnswer ans = liorSession.RemoveProductFromCategory("WanderlandItems", "GOLOGOLO");
            Assert.AreEqual((int)StoreEnum.ProductNotFound, ans.Status);
        }
        [TestMethod]
        public void RemoveProductFromCategoryProductNotInCategory()
        {
            _userService.EnterSystem();
            _userService.SignIn("Arik1", "123");
            StoreManagementService liorSession =
                (StoreManagementService)_market.GetStoreManagementService(_userService, "T");
            MarketAnswer ans = liorSession.RemoveProductFromCategory("WanderlandItems", "OnePunchManPoster");
            Assert.AreEqual((int)StoreEnum.ProductNotInCategory, ans.Status);
        }
        [TestMethod]
        public void RemoveProductFromCategoryToCategorySuccess()
        {
            _userService.EnterSystem();
            _userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)_market.GetStoreManagementService(_userService, "T");
            MarketAnswer ans = liorSession.RemoveProductFromCategory("WanderlandItems", "Fraid Egg");
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
