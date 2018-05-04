﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace StoreCenterTests
{
    [TestClass]

    public class RemoveProductTestsMock
    {
        private Mock<I_StoreDL> handler;
        Mock<IUserSeller> userService;
        Mock<IOrderSyncher> syncer;

        [TestInitialize]
        public void BuildStore()
        {
            handler = new Mock<I_StoreDL>();
            userService = new Mock<IUserSeller>();
            syncer = new Mock<IOrderSyncher>();
        }
        [TestMethod]
        public void RemoveProductFail()
        {
            RemoveProductSlave slave = new  RemoveProductSlave(syncer.Object,"noStore", userService.Object, handler.Object);
            slave.RemoveProduct("BOX");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.Answer.Status);
        }
        [TestMethod]
        public void RemoveProductPass()
        {
            Product P = new Product("NEWPROD", 150, "desc");
            Discount discount = new Discount(discountTypeEnum.Visible, DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, false);
            StockListItem SLI = new StockListItem(10, P, discount, PurchaseEnum.Immediate, "BLA");
            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
            handler.Setup(x => x.GetProductByNameFromStore("X", "NEWPROD")).Returns(P);
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            handler.Setup(x => x.GetProductFromStore("X", "NEWPROD")).Returns(SLI);
            RemoveProductSlave slave = new RemoveProductSlave(syncer.Object, "X", userService.Object, handler.Object);
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
