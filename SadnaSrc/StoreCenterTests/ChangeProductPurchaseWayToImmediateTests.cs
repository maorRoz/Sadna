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
    public class ChangeProductPurchaseWayToImmediateTests
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
            handler = StoreDL.GetInstance();
            userService = market.GetUserService();
        }
        [TestMethod]
        public void ChangeToImmediateStoreNotFound()
        {

            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "NotAStore");
            MarketAnswer ans = liorSession.ChangeProductPurchaseWayToImmediate("BOX");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
        }
        [TestMethod]
        public void ChangeToImmediateNoPremission()
        {
            userService.EnterSystem();
            userService.SignIn("Big Smoke", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.ChangeProductPurchaseWayToImmediate("BOX");
            Assert.AreEqual((int)StoreEnum.NoPremmision, ans.Status);
        }
        [TestMethod]
        public void ChangeToImmediateProductNotFound()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.ChangeProductPurchaseWayToImmediate("noPorduct");
            Assert.AreEqual((int)StoreEnum.ProductNotFound, ans.Status);
        }
        [TestMethod]
        public void ChangeToImmediateSuccessLottery()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            Product P = new Product("P1345678", "OBJ", 9, "des");
            ProductToDelete = new StockListItem(4, P, null, PurchaseEnum.Lottery, "S1");
            LotteryToDelete = new LotterySaleManagmentTicket("L1000", "X", P, DateTime.Parse("31/12/2018"), DateTime.Parse("31/12/2020"));
            handler.AddStockListItemToDataBase(ProductToDelete);
            handler.AddLottery(LotteryToDelete);
            MarketAnswer ans = liorSession.ChangeProductPurchaseWayToImmediate("OBJ");
            StockListItem find = handler.GetProductFromStore("X", "OBJ");
            Assert.AreEqual((int)PurchaseEnum.Immediate, (int)find.PurchaseWay);
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
        }
        [TestMethod]
        public void ChangeToImmediateSuccessImmediate()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.ChangeProductPurchaseWayToImmediate("BOX");
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
