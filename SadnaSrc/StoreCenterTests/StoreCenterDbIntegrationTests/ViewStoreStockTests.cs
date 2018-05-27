using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.StoreCenter;
using System;
using SadnaSrc.MarketData;


namespace StoreCenterTests.StoreCenterDbIntegrationTestss
{
    [TestClass]
    public class ViewStoreStockTests
    {
        private MarketYard market;
        IUserService userService;
        IUserService userService2;
        [TestInitialize]
        public void BuildStore()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;
            userService = market.GetUserService();
            userService2 = market.GetUserService();
            MarketYard.SetDateTime(DateTime.Parse("14/04/2018"));
        }
        [TestMethod]
        public void ViewStoreStockWhenStoreNotExists()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.LoginShoper("Arik3", "123");
            MarketAnswer ans = liorSession.ViewStoreStock("STORE_NOT_EXISTS");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
        }
        [TestMethod]
        public void ViewStoreStockWhenHasNoPremmision()
        {

            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            MarketAnswer ans = liorSession.ViewStoreStock("X");
            Assert.AreEqual((int)StoreEnum.NoPermission, ans.Status);
        }

        [TestMethod]
        public void ViewStoreStockSuccessAfterItemPurched()
        {
            userService2.EnterSystem();
            userService2.SignIn("Arik1", "123");
            StoreManagementService anotherSession =
                (StoreManagementService) market.GetStoreManagementService(userService2, "X");
            MarketAnswer ans = anotherSession.AddNewLottery("blabla", 8, "da", DateTime.Parse("11/01/2020"), DateTime.Parse("12/01/2021"));
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
            MarketYard.SetDateTime(DateTime.Parse("13/01/2020"));
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.LoginShoper("Arik3", "123");
            ans = liorSession.ViewStoreStock("X");
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
            string[] result1 = ans.ReportList;
            IOrderService order = market.GetOrderService(ref userService);
            ans = order.BuyLotteryTicket("blabla", "X", 1, 8);
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
            ans = liorSession.ViewStoreStock("X");
            string[] result2 = ans.ReportList;
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
            Assert.AreNotEqual(result1.Length, result2.Length);
        }
        [TestMethod]
        public void ViewStoreStockSuccessAfterDateExpiredOrNotBegan()
        {
            userService2.EnterSystem();
            userService2.SignIn("Arik1", "123");
            StoreManagementService anotherSession =
                (StoreManagementService)market.GetStoreManagementService(userService2, "X");
            MarketAnswer ans = anotherSession.AddNewLottery("blabla", 8, "da", DateTime.Parse("11/01/2020"), DateTime.Parse("12/01/2021"));
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
            MarketYard.SetDateTime(DateTime.Parse("13/01/2020"));
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.LoginShoper("Arik3", "123");
            ans = liorSession.ViewStoreStock("X");
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
            string[] result1 = ans.ReportList;
            MarketYard.SetDateTime(DateTime.Parse("25/01/2022"));
            ans = liorSession.ViewStoreStock("X");
            string[] result2 = ans.ReportList;
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
            Assert.AreNotEqual(result1.Length, result2.Length);
            MarketYard.SetDateTime(DateTime.Parse("15/01/2013"));
            ans = liorSession.ViewStoreStock("X");
            string[] result3 = ans.ReportList;
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
            Assert.AreNotEqual(result1.Length, result3.Length);
            Assert.AreEqual(result3.Length, result2.Length);


        }

        [TestMethod]
        public void ViewStoreStockSuccessCheckQuantityUpdated()
        {
            IStoreDL datalayer = StoreDL.Instance;
            userService2.EnterSystem();
            userService2.SignIn("Arik1", "123");
            StoreManagementService anotherSession =
                (StoreManagementService)market.GetStoreManagementService(userService2, "X");
            MarketAnswer ans = anotherSession.AddNewLottery("blabla", 8, "da", DateTime.Parse("11/01/2020"), DateTime.Parse("12/01/2021"));

            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
            anotherSession.AddQuanitityToProduct("blabla", 3);
            StockListItem sli = datalayer.GetProductFromStore("X","blabla");
            MarketYard.SetDateTime(DateTime.Parse("13/01/2020"));
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.LoginShoper("Arik3", "123");
            ans = liorSession.ViewStoreStock("X");
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
            string[] result1 = ans.ReportList;
            IOrderService order = market.GetOrderService(ref userService);
            ans = order.BuyLotteryTicket("blabla", "X", 1, 8);
            Assert.AreEqual((int)StoreEnum.Success, ans.Status);
            ans = liorSession.ViewStoreStock("X");
            string[] result2 = ans.ReportList;
            Assert.AreNotEqual(result2[result2.Length-1],result1[result1.Length-1]);            
            StockListItem sli2 = datalayer.GetProductFromStore("X","blabla");
            Assert.AreEqual(sli2.Quantity+1, sli.Quantity);
        }
        [TestMethod]
        public void ViewStoreStockSuccess()
        {
            StoreShoppingService liorSession = (StoreShoppingService)market.GetStoreShoppingService(ref userService);
            liorSession.LoginShoper("Arik3", "123");
            MarketAnswer ans = liorSession.ViewStoreStock("X");

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
