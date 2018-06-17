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

    public class EditDiscountTests
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
            MarketYard.SetDateTime(new DateTime(2018, 4, 14));

        }
        [TestMethod]
        public void EditDiscountWhenStoreNotExists()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "storeNotExists");
            MarketAnswer ans = liorSession.EditDiscount("BOX",null,false,null,null, "10",false);
            Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
        }
        [TestMethod]
        public void EditDiscountWhenHasNoPremmision()
        {
            userService.EnterSystem();
            userService.SignIn("Big Smoke", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditDiscount("BOX", null, false, null, null, "10", false);
            Assert.AreEqual((int)StoreEnum.NoPermission, ans.Status);
        }
        [TestMethod]
        public void EditDiscountWhenProductNotExists()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditDiscount("noProductLikeThis", null, false, null, null, "10", false);
            Assert.AreEqual((int)DiscountStatus.ProductNotFound, ans.Status);
        }
        [TestMethod]
        public void EditDiscountWhenDiscountNotExists()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 10, "desc", 3);
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", null, false, null, null, "10", false);
            Assert.AreEqual((int)DiscountStatus.DiscountNotFound, ans.Status);
        }

    
        [TestMethod]
        public void EditDiscountStartDateInPast()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 10, "desc", 3);
            liorSession.AddDiscountToProduct("NEWPROD", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 10, "VISIBLE", true);
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", null, false, "01/01/1990", null, null, false);
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, ans.Status);
        }
       

       
        [TestMethod]
        public void EditDiscountEndDateInPast()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("BOX", 10, "desc", 3);
            liorSession.AddDiscountToProduct("BOX", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 10, "VISIBLE", true);
            MarketAnswer ans = liorSession.EditDiscount("BOX", null, false, null, "01/01/1990", null, false);
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, ans.Status);
        }
      

        [TestMethod]
        public void EditDiscountStartDateSuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
          
            var product = handler.GetProductFromStore("X", "BOX");
            product.Discount.startDate = DateTime.Parse("06/06/2018");
            MarketAnswer ans = liorSession.EditDiscount("BOX", null, false, "06/06/2018", null, null, false);
            StockListItem find = handler.GetProductFromStore("X", "BOX");
            Assert.AreEqual(find.Discount.startDate, product.Discount.startDate);
            Assert.AreEqual((int)DiscountStatus.Success, ans.Status);
        }
        [TestMethod]
        public void EditDiscountEndDateSuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("BOX", 150, "desc", 3);
            liorSession.AddDiscountToProduct("BOX", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 100, "VISIBLE", false);
            var product = handler.GetProductFromStore("X", "BOX");
            product.Discount.EndDate = DateTime.Parse("05/09/2021");
            MarketAnswer ans = liorSession.EditDiscount("BOX", null, false, null, "05/09/2021", null, false);
            StockListItem find = handler.GetProductFromStore("X", "BOX");
            Assert.AreEqual(find.Discount.EndDate, product.Discount.EndDate);
            Assert.AreEqual((int)DiscountStatus.Success, ans.Status);

        }
        [TestMethod]
        public void EditDiscountDiscountAmountSuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("BOX", 150, "desc", 3);
            liorSession.AddDiscountToProduct("BOX", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 100, "VISIBLE", false);
            var product = handler.GetProductFromStore("X", "BOX");
            product.Discount.DiscountAmount = 80;
            MarketAnswer ans = liorSession.EditDiscount("BOX", null, false, null, null, "80", false);

            StockListItem find = handler.GetProductFromStore("X", "BOX");
            Assert.AreEqual(product.Discount.DiscountAmount, find.Discount.DiscountAmount);
            Assert.AreEqual((int)DiscountStatus.Success, ans.Status);
        }

        [TestMethod]
        public void EditDiscountPercentagesSuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("BOX", 150, "desc", 3);
            liorSession.AddDiscountToProduct("BOX", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, "VISIBLE", false);
            var product = handler.GetProductFromStore("X", "BOX");
            product.Discount.Percentages = true;
            MarketAnswer ans = liorSession.EditDiscount("BOX", null, false, null, null, "80", true);
            Assert.AreEqual((int)DiscountStatus.Success, ans.Status);
        }

        [TestMethod]
        public void EditDiscountDiscountTypeSuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("BOX", 150, "desc", 3);
            liorSession.AddDiscountToProduct("BOX", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, "VISIBLE", false);
            var product = handler.GetProductFromStore("X", "BOX");
            product.Discount.discountType = DiscountTypeEnum.Hidden;
            MarketAnswer ans = liorSession.EditDiscount("BOX", "shit", true, null, null, null, false);
            StockListItem find = handler.GetProductFromStore("X", "BOX");
            Assert.AreEqual((int)find.Discount.discountType, (int)product.Discount.discountType);
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
