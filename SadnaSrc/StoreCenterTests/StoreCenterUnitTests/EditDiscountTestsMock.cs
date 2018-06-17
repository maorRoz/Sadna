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
using SadnaSrc.MarketData;
using SadnaSrc.MarketRecovery;

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]

    public class EditDiscountTestsMock
    {
        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IMarketBackUpDB> marketDbMocker;
        private EditDiscountSlave slave;
        private Product prod;
        private Discount discount;
        private StockListItem stock;

        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketBackUpDB>();
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
            slave.EditDiscount("NEWPROD", null, false, null, null, "25", true);
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
        }

        [TestMethod]
        public void NoPermission()
        {
            userService.Setup(x => x.CanDeclareDiscountPolicy()).Throws(new MarketException(0, ""));
            slave.EditDiscount("NEWPROD", null, false, null, null, "25", true);
            Assert.AreEqual((int)StoreEnum.NoPermission, slave.answer.Status);
        }
     
        [TestMethod]
        public void NoDiscount()
        {
            stock = new StockListItem(10, prod, null, PurchaseEnum.Immediate, "BLA");
            handler.Setup(x => x.GetProductFromStore("X", "NEWPROD")).Returns(stock);
            slave.EditDiscount("NEWPROD", null, false, null, null, "25", true);
            Assert.AreEqual((int)DiscountStatus.DiscountNotFound, slave.answer.Status);
        }

        [TestMethod]
        public void InvalidDiscountType()
        {

            slave.EditDiscount("NEWPROD", "sdsd", false, null, null, "25", true);
            Assert.AreEqual((int)DiscountStatus.InvalidDiscountType, slave.answer.Status);
        }

        [TestMethod]
        public void StartDateInvalid1()
        {
            slave.EditDiscount("NEWPROD", null, false, "shit", null, null, true);
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, slave.answer.Status);
        }

        [TestMethod]
        public void StartDateInvalid2()
        {
            slave.EditDiscount("NEWPROD", null, false, "30/08/2020", null, null, true);
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, slave.answer.Status);
        }

        [TestMethod]
        public void EndDateInvalid1()
        {
            slave.EditDiscount("NEWPROD", null, false, null, "shit", null, true);
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, slave.answer.Status);
        }

        [TestMethod]
        public void EndDateInvalid2()
        {
            slave.EditDiscount("NEWPROD", null, false, null, "03/02/2020", null, true);
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, slave.answer.Status);
        }


        [TestMethod]
        public void DiscountBadPercentage1()
        {
            slave.EditDiscount("NEWPROD", null, false, null, null, "-2", true);
            Assert.AreEqual((int)DiscountStatus.InvalidDiscountAmount, slave.answer.Status);

        }

        [TestMethod]
        public void DiscountBadPercentage2()
        {
            slave.EditDiscount("NEWPROD", null, false, null, null, "500", true);
            Assert.AreEqual((int)DiscountStatus.InvalidDiscountAmount, slave.answer.Status);

        }

        [TestMethod]
        public void DiscountBadPercentage3S()
        {
            slave.EditDiscount("NEWPROD", null, false, null, null, "SHit", true);
            Assert.AreEqual((int)DiscountStatus.InvalidDiscountAmount, slave.answer.Status);

        }

        [TestMethod]
        public void DiscountBadAmmount1()
        {
            discount = new Discount(DiscountTypeEnum.Visible, DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, false);
            stock = new StockListItem(10, prod, discount, PurchaseEnum.Immediate, "BLA");
            handler.Setup(x => x.GetProductFromStore("X", "NEWPROD")).Returns(stock);
            slave.EditDiscount("NEWPROD", null, false, null, null, "500", false);
            Assert.AreEqual((int)DiscountStatus.InvalidDiscountAmount, slave.answer.Status);
        }

        [TestMethod]
        public void DiscountBadAmmount2()
        {
            discount = new Discount(DiscountTypeEnum.Visible, DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, false);
            stock = new StockListItem(10, prod, discount, PurchaseEnum.Immediate, "BLA");
            handler.Setup(x => x.GetProductFromStore("X", "NEWPROD")).Returns(stock);
            slave.EditDiscount("NEWPROD", null, false, null, null, "-2", false);
            Assert.AreEqual((int)DiscountStatus.InvalidDiscountAmount, slave.answer.Status);
        }

        [TestMethod]
        public void DiscountBadAmmount3()
        {
            discount = new Discount(DiscountTypeEnum.Visible, DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, false);
            stock = new StockListItem(10, prod, discount, PurchaseEnum.Immediate, "BLA");
            handler.Setup(x => x.GetProductFromStore("X", "NEWPROD")).Returns(stock);
            slave.EditDiscount("NEWPROD", null, false, null, null, "shit", false);
            Assert.AreEqual((int)DiscountStatus.InvalidDiscountAmount, slave.answer.Status);
        }
       
        [TestMethod]
        public void EditDiscountSuccess1()
        {
            slave.EditDiscount("NEWPROD", null, false, null, null, "25", true);
            Assert.AreEqual((int)StoreEnum.Success, slave.answer.Status);
        }

        [TestMethod]
        public void EditDiscountSuccess2()
        {
            slave.EditDiscount("NEWPROD", null, false, null, "03/9/2020", "25", true);
            Assert.AreEqual((int)StoreEnum.Success, slave.answer.Status);
        }

        [TestMethod]
        public void EditDiscountSuccess3()
        {
            slave.EditDiscount("NEWPROD", "SHIT", true, null, null, "25", true);
            Assert.AreEqual((int)StoreEnum.Success, slave.answer.Status);
        }

        [TestMethod]
        public void EditDiscountSuccess4()
        {
            slave.EditDiscount("NEWPROD", null, false, "03/2/2020", null, "25", true);
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
