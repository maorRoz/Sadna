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
        public class ViewStoreStockTestsMock
        {
            private Mock<IStoreDL> handler;
            private Mock<IUserShopper> userService;
            private Mock<IMarketDB> marketDbMocker;

            
            //TODO: improve this

            [TestInitialize]
            public void BuildStore()
            {
                marketDbMocker = new Mock<IMarketDB>();
                MarketException.SetDB(marketDbMocker.Object);
                MarketLog.SetDB(marketDbMocker.Object);
                handler = new Mock<IStoreDL>();
                userService = new Mock<IUserShopper>();

            }
            [TestMethod]
            public void ViewStoreStockFail()
            {
                ViewStoreStockSlave slave = new ViewStoreStockSlave(userService.Object, handler.Object);
                slave.ViewStoreStock("noStore");
                Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
            }

            //TODO : FIX THIS

            /*
            [TestMethod]
            public void ViewStoreStockPass()
            {
                Product p = new Product("P-4", "item", 1, "des");
                handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
                handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns(p);
                handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
                handler.Setup(x => x.GetProductFromStore("X", "item")).Returns(new StockListItem(4, p, null, PurchaseEnum.Immediate, "100"));
                handler.Setup(x => x.GetStockListItembyProductID("P-4")).Returns(new StockListItem(4, p, null, PurchaseEnum.Immediate, "100"));
                LinkedList<string> IDS = new LinkedList<string>();
                IDS.AddLast(p.SystemId);
                handler.Setup(x => x.GetAllStoreProductsID("S-4")).Returns(IDS);
                ViewStoreStockSlave slave = new ViewStoreStockSlave(userService.Object, handler.Object);
                slave.ViewStoreStock("X");
                Assert.AreEqual((int)StoreEnum.Success, slave.answer.Status);
            }
            */

            [TestCleanup]
            public void CleanUpOpenStoreTest()
            {
                MarketDB.Instance.CleanByForce();
                MarketYard.CleanSession();
            }
        }
    }

