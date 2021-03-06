﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class ChangeProductPurchesWayToLotteryTests
    {
        private MarketYard market;
        public StockListItem ProductToDelete;
        private IStoreDL handler;
        IUserService userService;
        public LotterySaleManagmentTicket LotteryToDelete;

        [TestInitialize]
        public void BuildStore()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;
            handler = StoreDL.Instance;
            userService = market.GetUserService();
        }
        [TestMethod]
        public void ChangeToLotteryStoreNotFound()
        {

            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "NotAStore");
            MarketAnswer ans = liorSession.ChangeProductPurchaseWayToLottery("BOX", DateTime.Parse("30/03/2019"), DateTime.Parse("31/12/2019"));
            Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
        }
        [TestMethod]
        public void ChangeToLotteryNoPremission()
        {
            userService.EnterSystem();
            userService.SignIn("Big Smoke", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.ChangeProductPurchaseWayToLottery("BOX", DateTime.Parse("30/03/2019"), DateTime.Parse("31/12/2019"));
            Assert.AreEqual((int)StoreEnum.NoPermission, ans.Status);
        }
        [TestMethod]
        public void ChangeToLotteryProductNotFound()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.ChangeProductPurchaseWayToLottery("noPorduct", DateTime.Parse("30/03/2019"), DateTime.Parse("31/12/2019"));
            Assert.AreEqual((int)StoreEnum.ProductNotFound, ans.Status);
        }
     
        [TestMethod]
        public void ChangeToLotterySuccess()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            Product P = new Product("P1345678", "OBJ", 9, "des");
            ProductToDelete = new StockListItem(4, P, null, PurchaseEnum.Immediate, "S1");
            handler.AddStockListItemToDataBase(ProductToDelete);
            MarketAnswer ans = liorSession.ChangeProductPurchaseWayToLottery("OBJ", DateTime.Parse("31/12/2018"), DateTime.Parse("31/12/2020"));
            StockListItem find = handler.GetProductFromStore("X", "OBJ");
            Assert.AreEqual((int)PurchaseEnum.Lottery, (int)find.PurchaseWay);
            LotteryToDelete = handler.GetLotteryByProductID(P.SystemId);
            Assert.IsNotNull(LotteryToDelete);
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
