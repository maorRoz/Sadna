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

    public class EditProductTestsMock
    {
        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IMarketBackUpDB> marketDbMocker;
        private EditProductSlave slave;
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
            slave = new EditProductSlave("X", userService.Object, handler.Object);
        }
        [TestMethod]
        public void NoStore()
        {
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
            slave.EditProduct("BOX", "price", "10");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
        }

        [TestMethod]
        public void NoPermission()
        {
            userService.Setup(x => x.CanManageProducts()).Throws(new MarketException(0, ""));
            slave.EditProduct("BOX", "price", "10");
            Assert.AreEqual((int)StoreEnum.NoPermission, slave.answer.Status);
        }

        [TestMethod]
        public void NoProduct()
        {
            handler.Setup(x => x.GetProductByNameFromStore("X", "NEWPROD")).Returns((Product)null);
            slave.EditProduct("NEWPROD", "price", "10");
            Assert.AreEqual((int)StoreEnum.ProductNotFound, slave.answer.Status);
        }

        [TestMethod]
        public void ProductNameExists()
        {
            slave.EditProduct("NEWPROD", "Name", "NEWPROD");
            Assert.AreEqual((int)StoreEnum.ProductNameNotAvlaiableInShop, slave.answer.Status);            
        }

        [TestMethod]
        public void BadPrice1()
        {
            slave.EditProduct("NEWPROD", "BasePrice", "0");
            Assert.AreEqual((int)StoreEnum.UpdateProductFail, slave.answer.Status);
        }

        [TestMethod]
        public void BadPrice2()
        {
            slave.EditProduct("NEWPROD", "BasePrice", "-50");
            Assert.AreEqual((int)StoreEnum.UpdateProductFail, slave.answer.Status);
        }

        [TestMethod]
        public void BadPrice3()
        {
            slave.EditProduct("NEWPROD", "BasePrice", "asd");
            Assert.AreEqual((int)StoreEnum.UpdateProductFail, slave.answer.Status);

        }
        [TestMethod]
        public void EditProductPriceSuccess()
        {
            slave.EditProduct("NEWPROD", "BasePrice", "10");
            Assert.AreEqual((int)StoreEnum.Success, slave.answer.Status);
        }

        [TestMethod]
        public void EditProductNameSuccess()
        {
            slave.EditProduct("NEWPROD", "Name", "OLDPROD");
            Assert.AreEqual((int)StoreEnum.Success, slave.answer.Status);
        }


        [TestMethod]
        public void EditProductDescSuccess()
        {
            slave.EditProduct("NEWPROD", "Description", "good shit");
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
