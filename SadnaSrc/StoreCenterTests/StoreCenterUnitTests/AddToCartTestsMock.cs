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
using SadnaSrc.MarketData;

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]
    public class AddToCartTestsMock
    {
        private Mock<IStoreDL> handler;
        private Mock<IUserShopper> userService;
        private Mock<IMarketDB> marketDbMocker;
        private Product prod;
        private AddProductToCartSlave slave;

        //TODO: improve this


        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserShopper>();
            MarketYard.SetDateTime(new DateTime(2018, 4, 14));
            prod = new Product("item", 1, "des");
            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
            handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns(prod);
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            handler.Setup(x => x.GetProductFromStore("X", "item")).Returns(new StockListItem(4, prod, null, PurchaseEnum.Immediate, "100"));
            slave = new AddProductToCartSlave(userService.Object, handler.Object);

        }

        [TestMethod]
        public void NoStore()
        {
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
            slave.AddProductToCart("X", "item", 1);
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
        }

        [TestMethod]
        public void NoPermission()
        {
            userService.Setup(x => x.ValidateCanBrowseMarket()).Throws(new MarketException(0, ""));
            slave.AddProductToCart("X", "item", 1);
            Assert.AreEqual((int)StoreEnum.NoPermission, slave.answer.Status);
        }

        [TestMethod]
        public void NoProduct()
        {
            handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns((Product)null);
            slave.AddProductToCart("X", "item", 1);
            Assert.AreEqual((int)StoreEnum.ProductNotFound, slave.answer.Status);
        }

        [TestMethod]
        public void BadQuantity1()
        {
            slave.AddProductToCart("X", "item", -1);
            Assert.AreEqual((int)StoreEnum.QuantityIsNegative, slave.answer.Status);
        }
        
        [TestMethod]
        public void BadQuantity2()
        {
            slave.AddProductToCart("X", "item", 0);
            Assert.AreEqual((int)StoreEnum.QuantityIsNegative, slave.answer.Status);
        }

        [TestMethod]
        public void BadQuantity3()
        {
            slave.AddProductToCart("X", "item", 50);
            Assert.AreEqual((int)StoreEnum.QuantityIsTooBig, slave.answer.Status);
        }


        [TestMethod]
        public void AddToCartSuccess()
        {
            slave.AddProductToCart("X", "item", 1);
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

