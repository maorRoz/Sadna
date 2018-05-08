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
            discount = new Discount(DiscountTypeEnum.Visible, DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, true);
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

        [TestMethod]
        public void InvalidDiscountType()
        {

            slave.EditDiscount("NEWPROD", "asd", "agado");
            Assert.AreEqual((int)DiscountStatus.NoLegalAttrebute, slave.answer.Status);
        }

        [TestMethod]
        public void StartDateInvalid1()
        {
            slave.EditDiscount("NEWPROD", "startdate", "agado");
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, slave.answer.Status);
        }

        [TestMethod]
        public void StartDateInvalid2()
        {
            slave.EditDiscount("NEWPROD", "startdate", "30/08/2020");
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, slave.answer.Status);
        }

        [TestMethod]
        public void EndDateInvalid1()
        {
            slave.EditDiscount("NEWPROD", "EndDate", "agado");
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, slave.answer.Status);
        }

        [TestMethod]
        public void EndDateInvalid2()
        {
            slave.EditDiscount("NEWPROD", "EndDate", "03/04/2020");
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, slave.answer.Status);
        }


        [TestMethod]
        public void DiscountAmountInvalid()
        {
            slave.EditDiscount("NEWPROD", "DiscountAmount", "satan");
            Assert.AreEqual((int)DiscountStatus.DiscountAmountIsNotNumber, slave.answer.Status);
          
        }

        [TestMethod]
        public void DiscountBadPercentage1()
        {
            slave.EditDiscount("NEWPROD", "DiscountAmount", "500");
            Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, slave.answer.Status);

        }

        [TestMethod]
        public void DiscountBadPercentage2()
        {
            slave.EditDiscount("NEWPROD", "DiscountAmount", "100");
            Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, slave.answer.Status);

        }

        [TestMethod]
        public void DiscountBadAmmount1()
        {
            discount = new Discount(DiscountTypeEnum.Visible, DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, false);
            stock = new StockListItem(10, prod, discount, PurchaseEnum.Immediate, "BLA");
            handler.Setup(x => x.GetProductFromStore("X", "NEWPROD")).Returns(stock);
            slave.EditDiscount("NEWPROD", "DiscountAmount", "500");
            Assert.AreEqual((int)DiscountStatus.DiscountGreaterThenProductPrice, slave.answer.Status);
        }

        [TestMethod]
        public void DiscountBadAmmount2()
        {
            discount = new Discount(DiscountTypeEnum.Visible, DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, false);
            stock = new StockListItem(10, prod, discount, PurchaseEnum.Immediate, "BLA");
            handler.Setup(x => x.GetProductFromStore("X", "NEWPROD")).Returns(stock);
            slave.EditDiscount("NEWPROD", "DiscountAmount", "-2");
            Assert.AreEqual((int)DiscountStatus.DiscountAmountIsNegativeOrZero, slave.answer.Status);
        }

        [TestMethod]
        public void DiscountBadAmmount3()
        {
            discount = new Discount(DiscountTypeEnum.Visible, DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, false);
            stock = new StockListItem(10, prod, discount, PurchaseEnum.Immediate, "BLA");
            handler.Setup(x => x.GetProductFromStore("X", "NEWPROD")).Returns(stock);
            slave.EditDiscount("NEWPROD", "DiscountAmount", "0");
            Assert.AreEqual((int)DiscountStatus.DiscountAmountIsNegativeOrZero, slave.answer.Status);
        }
       
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
