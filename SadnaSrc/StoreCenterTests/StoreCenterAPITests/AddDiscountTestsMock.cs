using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using System.Threading.Tasks;

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]
    public class AddDiscountTestsMock
    {
        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IMarketDB> marketDbMocker;
        private Product prod;


        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
            MarketYard.SetDateTime(new DateTime(2018, 4, 14));
            prod = new Product("item", 1, "des");
            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
            handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns(prod);
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            handler.Setup(x => x.GetProductFromStore("X", "item")).Returns(new StockListItem(4, prod, null, PurchaseEnum.Immediate, "100"));
        }
        [TestMethod]
        public void AddDiscountNoStore()
        {
            handler.Setup(x => x.GetStorebyName("X")).Returns((Store)null);
            AddDiscountToProductSlave slave = new AddDiscountToProductSlave("noStore", userService.Object, handler.Object);
            slave.AddDiscountToProduct("item", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 10, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.NoStore, slave.answer.Status);
        }
        [TestMethod]
        public void AddDiscountNoPermission()
        {
           
            userService.Setup(x => x.CanDeclareDiscountPolicy()).Throws(new MarketException(0,""));
            AddDiscountToProductSlave slave = new AddDiscountToProductSlave("X", userService.Object, handler.Object);
            slave.AddDiscountToProduct("item", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 10, "HIDDEN", true);
            Assert.AreEqual((int)StoreEnum.NoPermission, slave.answer.Status);
        }

        [TestMethod]
        public void AddDiscountNoProduct()
        {
            handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns((Product)null);
            AddDiscountToProductSlave slave = new AddDiscountToProductSlave("X", userService.Object, handler.Object);
            slave.AddDiscountToProduct("item", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 10, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.ProductNotFound, slave.answer.Status);
        }
        
        [TestMethod]
        public void BadDiscountDates1()
        {            
            AddDiscountToProductSlave slave = new AddDiscountToProductSlave("X", userService.Object, handler.Object);
            slave.AddDiscountToProduct("item", DateTime.Parse("01/01/2020"), DateTime.Parse("20/01/2019"), 10, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, slave.answer.Status);
        }

        [TestMethod]
        public void BadDiscountPercentages1()
        {
            AddDiscountToProductSlave slave = new AddDiscountToProductSlave("X", userService.Object, handler.Object);
            slave.AddDiscountToProduct("item", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 100, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, slave.answer.Status);
        }

        [TestMethod]
        public void BadDiscountPercentages2()
        {
            AddDiscountToProductSlave slave = new AddDiscountToProductSlave("X", userService.Object, handler.Object);
            slave.AddDiscountToProduct("item", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 200, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, slave.answer.Status);
        }

        [TestMethod]
        public void BadDiscountAmmount1()
        {
            AddDiscountToProductSlave slave = new AddDiscountToProductSlave("X", userService.Object, handler.Object);
            slave.AddDiscountToProduct("item", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), -5, "HIDDEN", false);
            Assert.AreEqual((int)DiscountStatus.DiscountAmountIsNegativeOrZero, slave.answer.Status);
        }

        [TestMethod]
        public void BadDiscountAmmount2()
        {
            AddDiscountToProductSlave slave = new AddDiscountToProductSlave("X", userService.Object, handler.Object);
            slave.AddDiscountToProduct("item", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 0, "HIDDEN", false);
            Assert.AreEqual((int)DiscountStatus.DiscountAmountIsNegativeOrZero, slave.answer.Status);
        }

        [TestMethod]
        public void BadDiscountAmmount3()
        {
            AddDiscountToProductSlave slave = new AddDiscountToProductSlave("X", userService.Object, handler.Object);
            slave.AddDiscountToProduct("item", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 2000, "HIDDEN", false);
            Assert.AreEqual((int)DiscountStatus.DiscountGreaterThenProductPrice, slave.answer.Status);
        }
        
        [TestMethod]
        public void AddDiscountPass()
        {
            Product prod = new Product("item", 1, "des");
           
            AddDiscountToProductSlave slave = new AddDiscountToProductSlave("X", userService.Object, handler.Object);
            slave.AddDiscountToProduct("item", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 10, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.Success, slave.answer.Status);
        }


        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }


    }
}
