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
    public class AddToCartTestsMock
    {
        private Mock<I_StoreDL> handler;
        Mock<IUserShopper> userService;

        [TestInitialize]
        public void BuildStore()
        {
            handler = new Mock<I_StoreDL>();
            userService = new Mock<IUserShopper>();
            
        }
        [TestMethod]
        public void AddToCartFail()
        {
            AddProductToCartSlave slave = new AddProductToCartSlave(userService.Object, handler.Object);
            slave.AddProductToCart("noStore", "NEWPROD", 1);
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
        }
        [TestMethod]
        public void AddToCartPass()
        {
            Product P = new Product("NEWPROD", 150, "desc");
            StockListItem SLI = new StockListItem(10, P, null, PurchaseEnum.Immediate, "BLA");


            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
            handler.Setup(x => x.GetProductByNameFromStore("X", "NEWPROD")).Returns(P);
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            handler.Setup(x => x.GetProductFromStore("X", "NEWPROD")).Returns(SLI);

            AddProductToCartSlave slave = new AddProductToCartSlave(userService.Object, handler.Object);
            slave.AddProductToCart("X", "NEWPROD", 1);
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

