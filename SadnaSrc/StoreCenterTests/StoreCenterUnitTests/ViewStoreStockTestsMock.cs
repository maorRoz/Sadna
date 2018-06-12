﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        public class ViewStoreStockTestsMock
        {
            private Mock<IStoreDL> handler;
            private Mock<IUserShopper> userService;
            private Mock<IMarketBackUpDB> marketDbMocker;
            private ViewStoreStockSlave slave;
            private string[] ids;

        [TestInitialize]
            public void BuildStore()
            {
                marketDbMocker = new Mock<IMarketBackUpDB>();
                MarketException.SetDB(marketDbMocker.Object);
                MarketLog.SetDB(marketDbMocker.Object);
                handler = new Mock<IStoreDL>();
                userService = new Mock<IUserShopper>();
                handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
                handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
                ids = new []
                {
                    "S-4"
                };
                handler.Setup(x => x.GetAllStoreProductsID("S-4")).Returns(ids);


            slave = new ViewStoreStockSlave(userService.Object, handler.Object);

        }
            [TestMethod]
            public void NoPermission()
            {
                userService.Setup(x => x.ValidateCanBrowseMarket()).Throws(new MarketException(0, ""));
                slave.ViewStoreStock("X");
                Assert.AreEqual((int)StoreEnum.NoPermission, slave.answer.Status);
            }

            [TestMethod]
            public void NoStore()
            {
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
            slave.ViewStoreStock("X");
                Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
            }


        [TestMethod]
            public void ViewStoreStockPass()
            {
                Product p = new Product("P-4", "item", 1, "des");
                Store S = new Store("X", "");
                handler.Setup(x => x.GetStorebyName("X")).Returns(S);
                handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns(p);
                handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
                handler.Setup(x => x.GetProductFromStore("X", "item")).Returns(new StockListItem(4, p, null, PurchaseEnum.Immediate, "100"));
                handler.Setup(x => x.GetStockListItembyProductID("P-4")).Returns(new StockListItem(4, p, null, PurchaseEnum.Immediate, "100"));
                var IDS = new[]
                {
                    p.SystemId
                };
                handler.Setup(x => x.GetAllStoreProductsID(S.SystemId)).Returns(IDS);
                ViewStoreStockSlave slave = new ViewStoreStockSlave(userService.Object, handler.Object);
                slave.ViewStoreStock("X");
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

