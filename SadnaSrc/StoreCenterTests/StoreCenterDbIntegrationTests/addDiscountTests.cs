using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.StoreCenter;

namespace StoreCenterTests.StoreCenterDbIntegrationTests
{

    //TODO: maybe remove these tests
    [TestClass]
    public class AddDiscountTests
    {
        private MarketYard market;
        public StockListItem ProductToDelete;
        private IStoreDL handler;
        IUserService userService;
            
        [TestInitialize]
        public void BuildStore()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;
            handler = StoreDL.Instance;
            userService = market.GetUserService();
            MarketYard.SetDateTime(new DateTime(2018,4,14));
        }

        [TestMethod]
        public void AddDiscountWhenStoreNotExists()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "storeNotExists");
            MarketAnswer ans = liorSession.AddDiscountToProduct("Golden BOX", DateTime.Parse("01/01/2019"), DateTime.Parse("31/01/2019"), 50, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.NoStore, ans.Status);
        }

        [TestMethod]
        public void AddDiscountWhenHasNoPremission()
        {
            userService.EnterSystem();
            userService.SignIn("Big Smoke", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddDiscountToProduct("Golden BOX", DateTime.Parse("01/01/2019"), DateTime.Parse("31/01/2019"), 50, "HIDDEN", true);
            Assert.AreEqual((int)StoreEnum.NoPermission, ans.Status);
        }

        [TestMethod]
        public void AddDisocuntWhenProductNotExists()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddDiscountToProduct("Silver BOX", DateTime.Parse("01/01/2019"), DateTime.Parse("31/01/2019"), 50, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.ProductNotFound, ans.Status);
        }

        [TestMethod]
        public void AddDiscountOldDates()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddDiscountToProduct("Golden BOX", DateTime.Parse("01/01/2000"), DateTime.Parse("31/01/2000"), 50, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, ans.Status);
        }

        [TestMethod]
        public void AddDiscountWhenProductHasAnExistingDiscount()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddDiscountToProduct("BOX", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 10, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.ThereIsAlreadyAnotherDiscount, ans.Status);
        }
        [TestMethod]
        public void AddDiscountsuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("item", 1, "des", 4);
            MarketAnswer ans = liorSession.AddDiscountToProduct("item", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 10, "HIDDEN", true);
            ProductToDelete = handler.GetProductFromStore("X", "item");
            Discount find = ProductToDelete.Discount;
            Assert.IsNotNull(find);
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
        }
        
      
        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
