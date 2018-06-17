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

    public class RemoveProductTestsMock
    {
        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IOrderSyncher> syncer;
        private Mock<IMarketBackUpDB> marketDbMocker;
        private Product prod;
        private Discount discount;
        private StockListItem stock;
        private RemoveProductSlave slave;



        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketBackUpDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
            syncer = new Mock<IOrderSyncher>();
            prod = new Product("NEWPROD", 150, "desc");
            discount = new Discount(DiscountTypeEnum.Visible, DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, false);
            stock = new StockListItem(10, prod, discount, PurchaseEnum.Immediate, "BLA");
            slave = new RemoveProductSlave(syncer.Object, "X", userService.Object, handler.Object);
            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
            handler.Setup(x => x.GetProductByNameFromStore("X", "NEWPROD")).Returns(prod);
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            handler.Setup(x => x.GetProductFromStore("X", "NEWPROD")).Returns(stock);
        }

        [TestMethod]
        public void NoStore()
        {
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
            slave.RemoveProduct("NEWPROD");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.Answer.Status);

        }

        [TestMethod]
        public void NoPermission()
        {
            userService.Setup(x => x.CanManageProducts()).Throws(new MarketException(0, ""));
            slave.RemoveProduct("NEWPROD");
            Assert.AreEqual((int)StoreEnum.NoPermission, slave.Answer.Status);
        }

        [TestMethod]
        public void NoProduct()
        {
            handler.Setup(x => x.GetProductByNameFromStore("X", "NEWPROD")).Returns((Product)null);
            slave.RemoveProduct("NEWPROD");
            Assert.AreEqual((int)StoreEnum.ProductNotFound, slave.Answer.Status);
        }
       
        [TestMethod]
        public void RemoveProductSuccess()
        {
           
            slave.RemoveProduct("NEWPROD");
            Assert.AreEqual((int)StoreEnum.Success, slave.Answer.Status);
        }

        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
