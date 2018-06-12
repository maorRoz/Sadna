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
using SadnaSrc.MarketRecovery;

namespace StoreCenterTests.StoreCenterUnitTests
{
        [TestClass]
        public class ChangeToImmediateTestsMock
        {
            private Mock<IStoreDL> handler;
            private Mock<IUserSeller> userService;
            private Mock<IOrderSyncher> syncer;
            private Mock<IMarketBackUpDB> marketDbMocker;
            private ChangeProductPurchaseWayToImmediateSlave slave;
            private Product prod;
            private Discount discount;

            [TestInitialize]
            public void BuildStore()
            {
                marketDbMocker = new Mock<IMarketBackUpDB>();
                MarketException.SetDB(marketDbMocker.Object);
                MarketLog.SetDB(marketDbMocker.Object);
                handler = new Mock<IStoreDL>();
                userService = new Mock<IUserSeller>();
                syncer = new Mock<IOrderSyncher>();
                MarketYard.SetDateTime(new DateTime(2018, 4, 14));
                prod = new Product("item", 1, "des");
                discount = new Discount(DiscountTypeEnum.Visible, DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, false);
                handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
                handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns(prod);
                handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
                handler.Setup(x => x.GetProductFromStore("X", "item")).Returns(new StockListItem(10, prod, discount, PurchaseEnum.Lottery, "BLA"));
                handler.Setup(x => x.GetLotteryByProductID(prod.SystemId)).Returns(new LotterySaleManagmentTicket("X", prod, DateTime.Parse("31/12/2018"), DateTime.Parse("31/12/2019")));
                slave = new ChangeProductPurchaseWayToImmediateSlave("X", userService.Object, syncer.Object, handler.Object);
        }
        [TestMethod]
            public void NoStore()
            {
                handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
                slave.ChangeProductPurchaseWayToImmediate("item");
                Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
            }

            [TestMethod]
            public void NoPermission()
            {
                userService.Setup(x => x.CanManageProducts()).Throws(new MarketException(0, ""));
                slave.ChangeProductPurchaseWayToImmediate("item");
                Assert.AreEqual((int)StoreEnum.NoPermission, slave.answer.Status);
            }

            [TestMethod]
            public void NoProduct()
            {
                handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns((Product)null);
                slave.ChangeProductPurchaseWayToImmediate("item");
                Assert.AreEqual((int)StoreEnum.ProductNotFound, slave.answer.Status);
            }
      
            [TestMethod]
            public void ChangeToImmediateSuccess()
            {
              slave.ChangeProductPurchaseWayToImmediate("item");
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
