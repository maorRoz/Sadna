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
    public class addQuantityTestsMock
    {
        private Mock<IStoreDL> handler;
        Mock<IUserSeller> userService;

        [TestInitialize]
        public void BuildStore()
        {
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
        }
        [TestMethod]
        public void addDiscountFail()
        {
            AddQuanitityToProductSlave slave = new AddQuanitityToProductSlave("noStore", userService.Object, handler.Object);
            slave.AddQuanitityToProduct("item", 10);
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
        }
        [TestMethod]
        public void addDiscountPass()
        {
            Product p = new Product("item", 1, "des");
            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
            handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns(p);
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            handler.Setup(x => x.GetProductFromStore("X", "item")).Returns(new StockListItem(4, p, null, PurchaseEnum.Immediate, "100"));
            AddQuanitityToProductSlave slave = new AddQuanitityToProductSlave("X", userService.Object, handler.Object);
            slave.AddQuanitityToProduct("item", 10);
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
