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
    [TestClass]
    public class EditProductTests
    {
        private MarketYard market;
        private IStoreDL handler;
        IUserService userService;
        [TestInitialize]
        public void BuildStore()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;
            handler = StoreDL.Instance;
            userService = market.GetUserService();
            userService.EnterSystem();
        }
        [TestMethod]
        public void EditProductWhenStoreNotExists()
        {
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "storeNotExists");
            MarketAnswer ans = liorSession.EditProduct("name0", "Name", "0");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
        }
        [TestMethod]
        public void EditProductWhenHasNoPremmision()
        {
            userService.SignIn("Big Smoke", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditProduct("name0", "Name", "0");
            Assert.AreEqual((int)StoreEnum.NoPermission, ans.Status);
        }
        [TestMethod]
        public void EditProductWhenProductIsNotAvailableInStore()
        {
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditProduct("name0", "Name", "0");
            Assert.AreEqual((int)StoreEnum.ProductNotFound, ans.Status);
        }
        
        [TestMethod]
        public void EditProductNameSuccessfully()
        {
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("GOLD", 5, "NONO", 8);
            MarketAnswer ans = liorSession.EditProduct("GOLD", "Name", "MOMO");
            Product find = handler.GetProductByNameFromStore(liorSession._storeName, "MOMO");
            Assert.IsNotNull(find);
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
        }
        
        [TestMethod]
        public void EditProductBasePriceSuccessfully()
        {
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("GOLD", 5, "NONO", 8);
            MarketAnswer ans = liorSession.EditProduct("GOLD", "BasePrice", "10");
            Product find = handler.GetProductByNameFromStore(liorSession._storeName, "GOLD");
            Assert.IsNotNull(find);
            Assert.AreEqual(10, find.BasePrice);
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
        }
        [TestMethod]
        public void EditProductDescriptionSuccessfully()
        {
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("GOLD", 5, "NONO", 8);
            MarketAnswer ans = liorSession.EditProduct("GOLD", "Description", "MOMO");
            Product find = handler.GetProductByNameFromStore(liorSession._storeName, "GOLD");
            Assert.IsNotNull(find);
            Assert.AreEqual("MOMO", find.Description);
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
        }

        [TestMethod]
        public void EditProductBadInputFail()
        {
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("GOLD", 5, "NONO", 8);
            MarketAnswer ans = liorSession.EditProduct("GOLD", "Description", "M'OMO");
            Product find = handler.GetProductByNameFromStore(liorSession._storeName, "GOLD");
            Assert.IsNotNull(find);
            Assert.AreEqual("NONO", find.Description);
            Assert.AreEqual((int)StoreEnum.BadInput, ans.Status);
        }


        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}