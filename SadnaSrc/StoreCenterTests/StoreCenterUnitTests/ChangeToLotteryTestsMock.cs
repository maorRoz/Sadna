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
        public class ChangeToLotteryTestsMock
        {
            private Mock<IStoreDL> handler;
            private Mock<IUserSeller> userService;
            private Mock<IMarketDB> marketDbMocker;
            private Product prod;
            private ChangeProductPurchaseWayToLotterySlave slave;
            private StockListItem stock;


        //TODO: improve this

        [TestInitialize]
            public void BuildStore()
            {
                marketDbMocker = new Mock<IMarketDB>();
                MarketException.SetDB(marketDbMocker.Object);
                MarketLog.SetDB(marketDbMocker.Object);
                handler = new Mock<IStoreDL>();
                userService = new Mock<IUserSeller>();
                slave = new ChangeProductPurchaseWayToLotterySlave("X", userService.Object, handler.Object);
                MarketYard.SetDateTime(new DateTime(2018, 4, 14));
                prod = new Product("item", 1, "des");
                Discount discount = new Discount(DiscountTypeEnum.Visible, DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, false);
                stock = new StockListItem(10, prod, discount, PurchaseEnum.Immediate, "BLA");
                handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
                handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns(prod);
                handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
                handler.Setup(x => x.GetProductFromStore("X", "item")).Returns(stock);
        }
            [TestMethod]
            public void NoStore()
            {
                handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
                slave.ChangeProductPurchaseWayToLottery("item", DateTime.Parse("31/12/2018"), DateTime.Parse("31/12/2019"));
                Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
            }
            [TestMethod]
            public void NoPermission()
            {
                userService.Setup(x => x.CanManageProducts()).Throws(new MarketException(0, ""));
                slave.ChangeProductPurchaseWayToLottery("item", DateTime.Parse("31/12/2018"), DateTime.Parse("31/12/2019"));
                Assert.AreEqual((int)StoreEnum.NoPermission, slave.answer.Status);
            }

            [TestMethod]
            public void NoProduct()
            {
                handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns((Product)null);
                slave.ChangeProductPurchaseWayToLottery("item", DateTime.Parse("31/12/2018"), DateTime.Parse("31/12/2019"));
                Assert.AreEqual((int)StoreEnum.ProductNotFound, slave.answer.Status);
            }

            [TestMethod]
            public void AlreadyLottery()
            {
                stock.PurchaseWay = PurchaseEnum.Lottery;
                handler.Setup(x => x.GetProductFromStore("X", "item")).Returns(stock);
                slave.ChangeProductPurchaseWayToLottery("item", DateTime.Parse("31/12/2018"), DateTime.Parse("31/12/2019"));
                Assert.AreEqual((int)ChangeToLotteryEnum.LotteryExists, slave.answer.Status);
            }

            [TestMethod]
            public void BadDates()
            {
                slave.ChangeProductPurchaseWayToLottery("item", DateTime.Parse("31/12/2018"), DateTime.Parse("31/12/2017"));
                Assert.AreEqual((int)ChangeToLotteryEnum.DatesAreWrong, slave.answer.Status);
            }

        [TestMethod]
            public void ChangeToLotterySuccess()
            {
                slave.ChangeProductPurchaseWayToLottery("item", DateTime.Parse("31/12/2018"), DateTime.Parse("31/12/2019"));
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

