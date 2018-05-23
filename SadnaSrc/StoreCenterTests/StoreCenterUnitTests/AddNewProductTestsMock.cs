using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.MarketData;

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]
    public class AddNewProductTestsMock
    {

        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IMarketDB> marketDbMocker;
        private AddNewProductSlave slave;

        //TODO: improve this


        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
            MarketYard.SetDateTime(new DateTime(2018, 4, 14));
            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
            handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns((Product)null);
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            slave = new AddNewProductSlave(userService.Object, "X", handler.Object);
        }


        [TestMethod]
        public void NoStore()
        {
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
            slave.AddNewProduct("item", 1, "des", 5);
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
        }

        [TestMethod]
        public void NoPermission()
        {
            userService.Setup(x => x.CanManageProducts()).Throws(new MarketException(0, ""));
            slave.AddNewProduct("item", 1, "des", 5);
            Assert.AreEqual((int)StoreEnum.NoPermission, slave.answer.Status);
        }

        [TestMethod]
        public void ProductNameTaken()
        {
            handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns(new Product("item", 1, "des"));
            slave.AddNewProduct("item", 1, "des", 5);
            Assert.AreEqual((int)StoreEnum.ProductNameNotAvlaiableInShop, slave.answer.Status);
        }

        [TestMethod]
        public void BadQuantity1()
        {
            slave.AddNewProduct("item", 1, "des", -5);
            Assert.AreEqual((int)StoreEnum.QuantityIsNegative, slave.answer.Status);
        }

        [TestMethod]
        public void BadQuantity2()
        {
            slave.AddNewProduct("item", 1, "des", 0);
            Assert.AreEqual((int)StoreEnum.QuantityIsNegative, slave.answer.Status);
        }


        [TestMethod]
        public void AddProductSuccess()
        {
            slave.AddNewProduct("item", 1, "des", 5);
            Assert.AreEqual((int)StoreEnum.Success, slave.answer.Status);
        }
    }
}
