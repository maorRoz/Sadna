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
    public class addQuantityTestsMock
    {
        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IMarketDB> marketDbMocker;
        private Product prod;
        private AddQuanitityToProductSlave slave;


        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
            slave = new AddQuanitityToProductSlave("X", userService.Object, handler.Object);
            MarketYard.SetDateTime(new DateTime(2018, 4, 14));
            prod = new Product("item", 1, "des");
            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
            handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns(prod);
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            handler.Setup(x => x.GetProductFromStore("X", "item")).Returns(new StockListItem(4, prod, null, PurchaseEnum.Immediate, "100"));
        }
        [TestMethod]
        public void NoStore()
        {
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
            slave.AddQuanitityToProduct("item", 10);
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
        }

        [TestMethod]
        public void NoPermission()
        {
            userService.Setup(x => x.CanManageProducts()).Throws(new MarketException(0, ""));
            slave.AddQuanitityToProduct("item", 10);
            Assert.AreEqual((int)StoreEnum.NoPermission, slave.answer.Status);
        }

        [TestMethod]
        public void NoProduct()
        {
            handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns((Product)null);
            slave.AddQuanitityToProduct("item", 10);
            Assert.AreEqual((int)StoreEnum.ProductNotFound, slave.answer.Status);
        }

        [TestMethod]
        public void BadQuantity()
        {
            slave.AddQuanitityToProduct("item", -10);
            Assert.AreEqual((int)StoreEnum.QuantityIsNegative, slave.answer.Status);
        }

        [TestMethod]
        public void AddQuantitySuccess()
        {
            slave.AddQuanitityToProduct("item", 10);
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
