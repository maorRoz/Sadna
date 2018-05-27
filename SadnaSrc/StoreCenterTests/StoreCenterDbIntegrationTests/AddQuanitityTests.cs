using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.MarketData;

namespace StoreCenterTests.StoreCenterDbIntegrationTests
{
    //TODO: maybe remove these tests

    [TestClass]
    public class AddQuantityTests
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
        }

        [TestMethod]
        public void AddQuanitityWhenStoreNotExists()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "storeNotExists");
            MarketAnswer ans = liorSession.AddQuanitityToProduct("BOX", 1);
            Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
        }
        [TestMethod]
        public void AddQuanitityWhenHasNoPremmision()
        {
            userService.EnterSystem();
            userService.SignIn("Big Smoke", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddQuanitityToProduct("BOX", 1);
            Assert.AreEqual((int)StoreEnum.NoPermission, ans.Status);
        }
        [TestMethod]
        public void AddQuanitiyWhenProductIsNotAvailableInStore()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddQuanitityToProduct("LOX", 1);
            Assert.AreEqual((int)StoreEnum.ProductNotFound, ans.Status);
        }
        
        [TestMethod]
        public void AddQuanitiySuccess()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("new", 5, "MOMO", 5);
            ProductToDelete = handler.GetProductFromStore(liorSession._storeName, "new");
            MarketAnswer ans = liorSession.AddQuanitityToProduct("new", 10);
            StockListItem find = handler.GetProductFromStore(liorSession._storeName, "new");
            Assert.AreEqual(find.Quantity, 15);
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