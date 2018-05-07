using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]

    public class EditDiscountTestsMock
    {
        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IMarketDB> marketDbMocker;
        private EditDiscountSlave slave;
        private Product prod;
        private Discount discount;
        private StockListItem stock;

        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
            prod = new Product("NEWPROD", 150, "desc");
            discount = new Discount(DiscountTypeEnum.Visible, DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, false);
            stock = new StockListItem(10, prod, discount, PurchaseEnum.Immediate, "BLA");
            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
            handler.Setup(x => x.GetProductByNameFromStore("X", "NEWPROD")).Returns(prod);
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            handler.Setup(x => x.GetProductFromStore("X", "NEWPROD")).Returns(stock);
            slave = new EditDiscountSlave("X", userService.Object, handler.Object);
        }

        [TestMethod]
        public void NoStore()
        {
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
            slave.EditDiscount("NEWPROD", "DiscountAmount", "10");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
        }

        [TestMethod]
        public void NoPermission()
        {
            userService.Setup(x => x.CanDeclareDiscountPolicy()).Throws(new MarketException(0, ""));
            slave.EditDiscount("NEWPROD", "DiscountAmount", "10");
            Assert.AreEqual((int)StoreEnum.NoPermission, slave.answer.Status);
        }

        [TestMethod]
        public void NoProduct()
        {
            handler.Setup(x => x.GetProductByNameFromStore("X", "NEWPROD")).Returns((Product)null);
            slave.EditDiscount("NEWPROD", "DiscountAmount", "10");
            Assert.AreEqual((int)StoreEnum.ProductNotFound, slave.answer.Status);
        }

        [TestMethod]
        public void NoDiscount()
        {
            stock = new StockListItem(10, prod, null, PurchaseEnum.Immediate, "BLA");
            handler.Setup(x => x.GetProductFromStore("X", "NEWPROD")).Returns(stock);
            slave.EditDiscount("NEWPROD", "DiscountAmount", "10");
            Assert.AreEqual((int)DiscountStatus.DiscountNotFound, slave.answer.Status);
        }
        /*

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
            Assert.AreEqual((int)DiscountStatus.DiscountAmountIsNotNumber, ans.Status);
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
            Assert.AreEqual((int)DiscountStatus.DiscountAmountIsNegativeOrZero, ans.Status);
        }
        [TestMethod]
        public void EditDiscountDiscountAmountIsZero()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditDiscount("BOX", "DiscountAmount", "0");
            Assert.AreEqual((int)DiscountStatus.DiscountAmountIsNegativeOrZero, ans.Status);
        }

        [TestMethod]
        public void EditDiscountPercentagesIsNotBoolean()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            MarketAnswer ans = liorSession.EditDiscount("BOX", "Percentages", "agado");
            Assert.AreEqual((int)DiscountStatus.PrecentegesIsNotBoolean, ans.Status);
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
        public void EditDiscountNoLegalAttrebute()
        {
            userService.EnterSystem();
            userService.SignIn("Arik1", "123");
            StoreManagementService liorSession = (StoreManagementService)market.GetStoreManagementService(userService, "X");
            liorSession.AddNewProduct("NEWPROD", 150, "desc", 3);
            liorSession.AddDiscountToProduct("NEWPROD", DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 100, "VISIBLE", false);
            MarketAnswer ans = liorSession.EditDiscount("NEWPROD", "golo-golo", "true");
            Assert.AreEqual((int)DiscountStatus.NoLegalAttrebute, ans.Status);
        }
        */
        [TestMethod]
        public void EditDiscountSuccess()
        {
            slave.EditDiscount("NEWPROD", "DiscountAmount", "10");
            Assert.AreEqual((int)StoreEnum.Success, slave.answer.Status);
        }

        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
