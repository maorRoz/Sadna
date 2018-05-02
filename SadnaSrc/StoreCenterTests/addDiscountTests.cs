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
    public class AddDiscountTests
    {
        private MarketYard market;
        public StockListItem ProductToDelete;
        private I_StoreDL handler;
        IUserService userService;
            
        [TestInitialize]
        public void BuildStore()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;
            handler = StoreDL.GetInstance();
            userService = market.GetUserService();
        }
        [TestMethod]
        public void addDiscountWhenStoreNotExists()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "storeNotExists");
            MarketAnswer ans = liorSession.AddDiscountToProduct("BOX", DateTime.Parse("01/01/2019"), DateTime.Parse("31/01/2019"), 50, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.NoStore, ans.Status);
        }
        [TestMethod]
        public void addDiscountWhenHasNoPremission()
        {
            userService.EnterSystem();
            userService.SignIn("CJ", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddDiscountToProduct("BOX", DateTime.Parse("01/01/2019"), DateTime.Parse("31/01/2019"), 50, "HIDDEN", true);
            Assert.AreEqual((int)StoreEnum.NoPremmision, ans.Status);
        }
        [TestMethod]
        public void addDisocuntWhenProductNotExists()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddDiscountToProduct("Lox", DateTime.Parse("01/01/2019"), DateTime.Parse("31/01/2019"), 50, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.ProductNotFound, ans.Status);
        }
            [TestMethod]
        public void addDiscountWhenStartDateIsOld()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddDiscountToProduct("BOX", DateTime.Parse("01/01/1990"), DateTime.Parse("31/01/2019"), 50, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, ans.Status);
        }
        [TestMethod]
        public void addDiscountWhenEndDateIsOld()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddDiscountToProduct("BOX", DateTime.Parse("01/01/2019"), DateTime.Parse("31/01/1990"), 50, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, ans.Status);
        }
        [TestMethod]
        public void addDiscountWhenEndDateIsSmallerThenStartDateIsOld()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddDiscountToProduct("BOX", DateTime.Parse("01/01/2019"), DateTime.Parse("31/01/2018"), 50, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, ans.Status);
        }
        [TestMethod]
        public void addDiscountWhenEndDateIsEqualToStartDateIsOld()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddDiscountToProduct("BOX", DateTime.Parse("01/01/2019"), DateTime.Parse("01/01/2019"), 50, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, ans.Status);
        }
        [TestMethod]
        public void addDiscountWhenDiscountAmountIsMoreThen100Precent()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddDiscountToProduct("BOX", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 150, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, ans.Status);
        }
        [TestMethod]
        public void addDiscountWhenDiscountAmountIsEqualTo100Precent()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddDiscountToProduct("BOX", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 100, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, ans.Status);
        }
        [TestMethod]
        public void addDiscountWhenDiscountAmountNegative()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddDiscountToProduct("BOX", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), -5, "HIDDEN", false);
            Assert.AreEqual((int)DiscountStatus.discountAmountIsNegativeOrZero, ans.Status);
        }
        [TestMethod]
        public void addDiscountWhenDiscountAmountIsZero()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddDiscountToProduct("BOX", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 0, "HIDDEN", false);
            Assert.AreEqual((int)DiscountStatus.discountAmountIsNegativeOrZero, ans.Status);
        }
        [TestMethod]
        public void addDiscountWhenDiscountAmountIsMoreThenProductPrice()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddDiscountToProduct("BOX", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 9000, "HIDDEN", false);
            Assert.AreEqual((int)DiscountStatus.DiscountGreaterThenProductPrice, ans.Status);
        }
            [TestMethod]
        public void addDiscountWhenProductHasAnExistingDiscount()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.AddDiscountToProduct("BOX", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 10, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.thereIsAlreadyAnotherDiscount, ans.Status);
        }
        [TestMethod]
        public void addDiscountsuccessfully()
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
