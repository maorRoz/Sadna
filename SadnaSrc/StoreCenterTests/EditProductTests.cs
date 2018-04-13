using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCenterTests
{
    [TestClass]
    public class EditProductTests
    {
        private MarketYard market;
        public StockListItem ProductToDelete;
        private ModuleGlobalHandler handler;
        IUserService userService;
        [TestInitialize]
        public void BuildStore()
        {
            market = MarketYard.Instance;
            handler = ModuleGlobalHandler.GetInstance();
            userService = market.GetUserService();
        }
        [TestMethod]
        public void EditProductWhenStoreNotExists()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "storeNotExists");
            MarketAnswer ans = liorSession.EditProduct("name0", "Name", "0");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
        }
        [TestMethod]
        public void EditProductWhenHasNoPremmision()
        {
            userService.EnterSystem();
            userService.SignIn("Big Smoke", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditProduct("name0", "Name", "0");
            Assert.AreEqual((int)ViewStoreStatus.InvalidUser, ans.Status);
        }
        [TestMethod]
        public void EditProductWhenProductIsNotAvailableInStore()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditProduct("name0", "Name", "0");
            Assert.AreEqual((int)StoreEnum.ProductNotFound, ans.Status);
        }
        [TestMethod]
        public void EditProductFailNameExists()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditProduct("BOX", "Name", "BOX");
            Assert.AreEqual((int)StoreEnum.ProductNameNotAvlaiableInShop, ans.Status);
        }
        [TestMethod]
        public void EditProductNameSuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("GOLD", 5, "NONO", 8);
            ProductToDelete = handler.DataLayer.GetProductFromStore(liorSession._storeName, "GOLD");
            MarketAnswer ans = liorSession.EditProduct("GOLD", "Name", "MOMO");
            Product find = handler.DataLayer.getProductByNameFromStore(liorSession._storeName, "MOMO");
            Assert.IsNotNull(find);
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
        }
        [TestMethod]
        public void EditProductBasePriceFailPriceZero()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("GOLD", 5, "NONO", 8);
            ProductToDelete = handler.DataLayer.GetProductFromStore(liorSession._storeName, "GOLD");
            MarketAnswer ans = liorSession.EditProduct("GOLD", "BasePrice", "0");
            Product find = handler.DataLayer.getProductByNameFromStore(liorSession._storeName, "GOLD");
            Assert.IsNotNull(find);
            Assert.AreEqual(5, find.BasePrice);
            Assert.AreEqual((int)StoreEnum.UpdateProductFail, ans.Status);
        }
        [TestMethod]
        public void EditProductBasePriceFailPriceIsNotNumber()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("GOLD", 5, "NONO", 8);
            ProductToDelete = handler.DataLayer.GetProductFromStore(liorSession._storeName, "GOLD");
            MarketAnswer ans = liorSession.EditProduct("GOLD", "BasePrice", "NotA-Number");
            Product find = handler.DataLayer.getProductByNameFromStore(liorSession._storeName, "GOLD");
            Assert.IsNotNull(find);
            Assert.AreEqual(5, find.BasePrice);
            Assert.AreEqual((int)StoreEnum.UpdateProductFail, ans.Status);
        }
        [TestMethod]
        public void EditProductBasePriceFailPriceNegative()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("GOLD", 5, "NONO", 8);
            ProductToDelete = handler.DataLayer.GetProductFromStore(liorSession._storeName, "GOLD");
            MarketAnswer ans = liorSession.EditProduct("GOLD", "BasePrice", "-4");
            Product find = handler.DataLayer.getProductByNameFromStore(liorSession._storeName, "GOLD");
            Assert.IsNotNull(find);
            Assert.AreEqual(5, find.BasePrice);
            Assert.AreEqual((int)StoreEnum.UpdateProductFail, ans.Status);
        }
        [TestMethod]
        public void EditProductBasePriceSuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("GOLD", 5, "NONO", 8);
            ProductToDelete = handler.DataLayer.GetProductFromStore(liorSession._storeName, "GOLD");
            MarketAnswer ans = liorSession.EditProduct("GOLD", "BasePrice", "10");
            Product find = handler.DataLayer.getProductByNameFromStore(liorSession._storeName, "GOLD");
            Assert.IsNotNull(find);
            Assert.AreEqual(10, find.BasePrice);
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
        }
        [TestMethod]
        public void EditProductDescriptionSuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("GOLD", 5, "NONO", 8);
            ProductToDelete = handler.DataLayer.GetProductFromStore(liorSession._storeName, "GOLD");
            MarketAnswer ans = liorSession.EditProduct("GOLD", "Description", "MOMO");
            Product find = handler.DataLayer.getProductByNameFromStore(liorSession._storeName, "GOLD");
            Assert.IsNotNull(find);
            Assert.AreEqual("MOMO", find.Description);
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
        }


        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            if (ProductToDelete != null)
            {
                handler.DataLayer.RemoveStockListItem(ProductToDelete);
            }
            userService.CleanSession();
            MarketYard.CleanSession();
        }
    }
}
