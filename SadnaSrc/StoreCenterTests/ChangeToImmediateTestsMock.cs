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

namespace StoreCenterTests
    {
        [TestClass]
        public class ChangeToImmediateTestsMock
        {
            private Mock<IStoreDL> handler;
            Mock<IUserSeller> userService;
            Mock<IOrderSyncher> syncer;

        [TestInitialize]
            public void BuildStore()
            {
                handler = new Mock<IStoreDL>();
                userService = new Mock<IUserSeller>();
               syncer = new Mock<IOrderSyncher>();
        }
            [TestMethod]
            public void changeToImmediateFail()
            {
                handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
                ChangeProductPurchaseWayToImmediateSlave slave = new ChangeProductPurchaseWayToImmediateSlave("noStore", userService.Object,syncer.Object, handler.Object);
                slave.ChangeProductPurchaseWayToImmediate("newProd");
                Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
            }
            [TestMethod]
            public void changeToImmediatePass()
            {
            Product P = new Product("NEWPROD", 150, "desc");
            Discount discount = new Discount(discountTypeEnum.Visible, DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, false);
            StockListItem SLI = new StockListItem(10, P, discount, PurchaseEnum.Lottery, "BLA");
            LotterySaleManagmentTicket LSMT = new LotterySaleManagmentTicket("X", P, DateTime.Parse("31/12/2018"), DateTime.Parse("31/12/2019"));

            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
            handler.Setup(x => x.GetProductByNameFromStore("X", "NEWPROD")).Returns(P);
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            handler.Setup(x => x.GetProductFromStore("X", "NEWPROD")).Returns(SLI);
            handler.Setup(x => x.GetLotteryByProductID(P.SystemId)).Returns(LSMT);
            ChangeProductPurchaseWayToImmediateSlave slave = new ChangeProductPurchaseWayToImmediateSlave("X", userService.Object, syncer.Object, handler.Object);
            slave.ChangeProductPurchaseWayToImmediate("NEWPROD");
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
