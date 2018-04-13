﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            MarketAnswer ans = liorSession.AddProduct("name0", 1, "des", 4);
            Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
        }
        [TestMethod]
        public void EditProductWhenHasNoPremmision()
        {
            userService.EnterSystem();
            userService.SignIn("Big Smoke", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddProduct("name0", 1, "des", 4);
            Assert.AreEqual((int)StoreEnum.NoPremmision, ans.Status);
        }
        [TestMethod]
        public void EditProductWhenProductIsNotAvailableInStore()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddProduct("BOX", 1, "des", 4);
            Assert.AreEqual((int)StoreEnum.ProductNameNotAvlaiableInShop, ans.Status);
        }
        [TestMethod]
        public void EditProductNameSuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddProduct("item", 1, "des", -4);
            Assert.AreEqual((int)StoreEnum.quantityIsNegatie, ans.Status);
        }
        [TestMethod]
        public void EditProductBasePriceFailPriceZero()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddProduct("item", 1, "des", -4);
            Assert.AreEqual((int)StoreEnum.quantityIsNegatie, ans.Status);
        }
        [TestMethod]
        public void EditProductBasePriceFailPriceNegative()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddProduct("item", 1, "des", -4);
            Assert.AreEqual((int)StoreEnum.quantityIsNegatie, ans.Status);
        }
        [TestMethod]
        public void EditProductBasePriceSuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddProduct("item", 1, "des", -4);
            Assert.AreEqual((int)StoreEnum.quantityIsNegatie, ans.Status);
        }
        [TestMethod]
        public void EditProductDescriptionSuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddProduct("item", 1, "des", 4);
            ProductToDelete = handler.DataLayer.GetProductFromStore("X", "item");
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
