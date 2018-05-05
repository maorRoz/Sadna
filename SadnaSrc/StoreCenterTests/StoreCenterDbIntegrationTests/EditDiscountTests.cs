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
        }
        [TestMethod]
        public void EditDiscountWhenStoreNotExists()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "storeNotExists");
            MarketAnswer ans = liorSession.EditDiscount("BOX", "DiscountAmount", "10");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
        }
        [TestMethod]
        public void EditDiscountWhenHasNoPremmision()
        {
            userService.EnterSystem();
            userService.SignIn("Big Smoke", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditDiscount("BOX", "DiscountAmount", "10");
            Assert.AreEqual((int)StoreEnum.NoPermission, ans.Status);
        }
        [TestMethod]
        public void EditDiscountWhenProductNotExists()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditDiscount("notExists", "DiscountAmount", "10");
            Assert.AreEqual((int)StoreEnum.ProductNotFound, ans.Status);
        }
        [TestMethod]
        public void EditDiscountWhenDiscountNotExists()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 10, "desc", 3);
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", "DiscountAmount", "10");
            Assert.AreEqual((int)DiscountStatus.DiscountNotFound, ans.Status);
        }

        [TestMethod]
        public void EditDiscountInsertInvalidDiscountType()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditDiscount("BOX", "discountType", "agado");
            Assert.AreEqual((int)StoreEnum.EnumValueNotExists, ans.Status);
        }
        [TestMethod]
        public void EditDiscountStartDateInvalid()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditDiscount("BOX", "startDate", "agado");
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, ans.Status);
        }
        [TestMethod]
        public void EditDiscountStartDateInPast()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditDiscount("BOX", "startDate", "01/01/1990");
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, ans.Status);
        }
        [TestMethod]
        public void EditDiscountStartDateLaterThenEndDate()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditDiscount("BOX", "startDate", "31/12/2590");
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, ans.Status);
        }
        
        [TestMethod]
        public void EditDiscountEndDateInvalid()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditDiscount("BOX", "EndDate", "agado");
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, ans.Status);
        }
        [TestMethod]
        public void EditDiscountEndDateInPast()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 10, "desc", 3);
            liorSession.AddDiscountToProduct("NEWPROD", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 10, "VISIBLE", true);
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", "EndDate", "31/12/1998");
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, ans.Status);
        }
        [TestMethod]
        public void EditDiscountEndDateSoonerThenStartDate()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 10, "desc", 3);
            liorSession.AddDiscountToProduct("NEWPROD", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 10, "VISIBLE", true);
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", "EndDate", "31/12/2018");
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, ans.Status);
        }
        [TestMethod]
        public void EditDiscountDiscountAmountInvalid()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditDiscount("BOX", "DiscountAmount", "ag88ado");
            Assert.AreEqual((int)DiscountStatus.discountAmountIsNotNumber, ans.Status);
        }
        [TestMethod]
        public void EditDiscountDiscountAmountIsMoreThen100AndPercentages()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 10, "desc", 3);
            liorSession.AddDiscountToProduct("NEWPROD", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 10, "VISIBLE", true);
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", "DiscountAmount", "500");
            Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, ans.Status);
        }
        [TestMethod]
        public void EditDiscountDiscountAmountIs100AndPercentages()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 10, "desc", 3);
            liorSession.AddDiscountToProduct("NEWPROD", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 10, "VISIBLE", true);
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", "DiscountAmount", "100");
            Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, ans.Status);
        }
        [TestMethod]
        public void EditDiscountDiscountAmountIsGreaterThenProductPriceNoPercentages()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 100, "desc", 3);
            liorSession.AddDiscountToProduct("NEWPROD", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 10, "VISIBLE", false);
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", "DiscountAmount", "150");
            Assert.AreEqual((int)DiscountStatus.DiscountGreaterThenProductPrice, ans.Status);
        }
        [TestMethod]
        public void EditDiscountDiscountAmountIsNegative()
        {

            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditDiscount("BOX", "DiscountAmount", "-30");
            Assert.AreEqual((int)DiscountStatus.discountAmountIsNegativeOrZero, ans.Status);
        }
        [TestMethod]
        public void EditDiscountDiscountAmountIsZero()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditDiscount("BOX", "DiscountAmount", "0");
            Assert.AreEqual((int)DiscountStatus.discountAmountIsNegativeOrZero, ans.Status);
        }
        
        [TestMethod]
        public void EditDiscountPercentagesIsNotBoolean()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditDiscount("BOX", "Percentages", "agado");
            Assert.AreEqual((int)DiscountStatus.precentegesIsNotBoolean, ans.Status);
        }
        [TestMethod]
        public void EditDiscountPercentagesWhenDiscountAmountIsMoreThen100()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 150, "desc", 3);
            liorSession.AddDiscountToProduct("NEWPROD", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 101, "VISIBLE", false);
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", "Percentages", "True");
            Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, ans.Status);
        }
        [TestMethod]
        public void EditDiscountPercentagesWhenDiscountAmountIs100()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 150, "desc", 3);
            liorSession.AddDiscountToProduct("NEWPROD", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 100, "VISIBLE", false);
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", "Percentages", "True");
            Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, ans.Status);
        }
        [TestMethod]
        public void EditDiscountNoLegalAttrebute() {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 150, "desc", 3);
            liorSession.AddDiscountToProduct("NEWPROD", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 100, "VISIBLE", false);
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", "golo-golo", "true");
            Assert.AreEqual((int)DiscountStatus.NoLegalAttrebute, ans.Status);
        }


        [TestMethod]
        public void EditDiscountStartDateSuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 150, "desc", 3);
            liorSession.AddDiscountToProduct("NEWPROD", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 100, "VISIBLE", false);
            var product = handler.GetProductFromStore("X", "NEWPROD");
            product.Discount.startDate = DateTime.Parse("05/04/2020");
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", "startDate", "05/04/2020");
            StockListItem find = handler.GetProductFromStore("X", "NEWPROD");
            Assert.AreEqual(find.Discount.startDate, product.Discount.startDate);
            Assert.AreEqual((int)DiscountStatus.Success, ans.Status);
        }
        [TestMethod]
        public void EditDiscountEndDateSuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 150, "desc", 3);
            liorSession.AddDiscountToProduct("NEWPROD", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 100, "VISIBLE", false);
            var product = handler.GetProductFromStore("X", "NEWPROD");
            product.Discount.EndDate = DateTime.Parse("05/09/2021");
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", "EndDate", "05/09/2021");
            StockListItem find = handler.GetProductFromStore("X", "NEWPROD");
            Assert.AreEqual(find.Discount.EndDate, product.Discount.EndDate);
            Assert.AreEqual((int)DiscountStatus.Success, ans.Status);

        }
        [TestMethod]
        public void EditDiscountDiscountAmountSuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 150, "desc", 3);
            liorSession.AddDiscountToProduct("NEWPROD", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 100, "VISIBLE", false);
            var product = handler.GetProductFromStore("X", "NEWPROD");
            product.Discount.DiscountAmount = 130;
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", "DiscountAmount", "130");
            StockListItem find = handler.GetProductFromStore("X", "NEWPROD");
            Assert.AreEqual(product.Discount.DiscountAmount, find.Discount.DiscountAmount);
            Assert.AreEqual((int)DiscountStatus.Success, ans.Status);
        }
        [TestMethod]
        public void EditDiscountPercentagesSuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 150, "desc", 3);
            liorSession.AddDiscountToProduct("NEWPROD", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, "VISIBLE", false);
            var product = handler.GetProductFromStore("X", "NEWPROD");
            product.Discount.Percentages = true;
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", "Percentages", "true");
            Assert.AreEqual((int)DiscountStatus.Success, ans.Status);
        }

        [TestMethod]
        public void EditDiscountDiscountTypeSuccessfully()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 150, "desc", 3);
            liorSession.AddDiscountToProduct("NEWPROD", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, "VISIBLE", false);
            var product = handler.GetProductFromStore("X", "NEWPROD");
            product.Discount.discountType = discountTypeEnum.Hidden;
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", "discountType", "Hidden");
            StockListItem find = handler.GetProductFromStore("X", "NEWPROD");
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
