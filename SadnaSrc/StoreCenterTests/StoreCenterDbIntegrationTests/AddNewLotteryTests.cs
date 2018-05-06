using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCenterTests.StoreCenterDbIntegrationTests
{
    //TODO: maybe remove these tests

    [TestClass]
    public class AddNewLotteryTests
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
            MarketYard.SetDateTime(new DateTime(2018, 4, 14));
        }
        [TestMethod]
        public void AddLotteryWhenStoreNotExists()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "storeNotExists");
            MarketAnswer ans = liorSession.AddNewLottery("name0", 1, "des", DateTime.Parse("30/10/2019"), DateTime.Parse("30/12/2019"));
            Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
        }
        [TestMethod]
        public void AddLotteryWhenHasNoPremmision()
        {
            userService.EnterSystem();
            userService.SignIn("Big Smoke", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddNewLottery("name0", 1, "des", DateTime.Parse("30/10/2019"), DateTime.Parse("30/12/2019"));
            Assert.AreEqual((int)StoreEnum.NoPermission, ans.Status);
        }
        [TestMethod]
        public void AddLotteryWhenProductNameIsNotAvailableInStore()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddNewLottery("BOX", 1, "des", DateTime.Parse("30/10/2019"), DateTime.Parse("30/12/2019"));
            Assert.AreEqual((int)StoreEnum.ProductNameNotAvlaiableInShop, ans.Status);
        }
        [TestMethod]
        public void AddLotteryOldDates()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddNewLottery("name0", 1, "des", DateTime.Parse("30/12/2000"), DateTime.Parse("30/10/2000"));
            Assert.AreEqual((int)StoreEnum.DatesAreWrong, ans.Status);
        }
        [TestMethod]
        public void AddLotterySuccess()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddNewLottery("name0", 1, "des", DateTime.Parse("30/10/2019"), DateTime.Parse("30/12/2019"));
            ProductToDelete = handler.GetProductFromStore("X", "name0");
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
