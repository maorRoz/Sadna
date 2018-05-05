using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using Moq;


namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]
    public class AddCategoryTestsMock
    {
        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IMarketDB> marketDbMocker;

        //TODO: improve this


        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
        }
        [TestMethod]
        public void AddCategoryWhenStoreNotExists()
        {
            AddCategorySlave slave = new AddCategorySlave("noStore", userService.Object, handler.Object);
            slave.AddCategory("items");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.Answer.Status);

        }
        [TestMethod]
        public void AddCategoryWhenHasNoPremmision()
        {

            AddCategorySlave slave = new AddCategorySlave("T", userService.Object, handler.Object);
            handler.Setup(x => x.GetStorebyName("T")).Returns(new Store("S7", "T", "bla"));
            userService.Setup(x => x.CanManageProducts()).Throws(new Exception());
            slave.AddCategory("items");
            Assert.AreEqual((int)StoreEnum.NoPremmision, slave.Answer.Status);
        }
        [TestMethod]
        public void AddCategoryWhenCategoryAlreadyExists()
        {
            AddCategorySlave slave = new AddCategorySlave("T", userService.Object, handler.Object);
            handler.Setup(x => x.GetStorebyName("T")).Returns(new Store("S7", "T", "bla"));
            handler.Setup(x => x.getCategoryByName("S7", "items")).Returns(new Category("items", "S7"));
            slave.AddCategory("items");
            Assert.AreEqual((int)StoreEnum.CategoryExistsInStore, slave.Answer.Status);
        }
        [TestMethod]
        public void AddCategorySuccess()
        {
            AddCategorySlave slave = new AddCategorySlave("T", userService.Object, handler.Object);
            handler.Setup(x => x.GetStorebyName("T")).Returns(new Store("S7", "T", "bla"));
            slave.AddCategory("items");
            Assert.AreEqual((int)StoreEnum.Success, slave.Answer.Status);
        }
        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}

/**
 * using Microsoft.VisualStudio.TestTools.UnitTesting;
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
**/