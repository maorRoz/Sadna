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
    public class AddDiscountTestsMock
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
        public void AddDiscountFail()
        {
            AddDiscountToProductSlave slave = new AddDiscountToProductSlave("noStore", userService.Object, handler.Object);
            slave.AddDiscountToProduct("item", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 10, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.NoStore, slave.answer.Status);
        }
        [TestMethod]
        public void AddDiscountPass()
        {
            Product p = new Product("item", 1, "des");
            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X",""));
            handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns(p);
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            handler.Setup(x => x.GetProductFromStore("X", "item")).Returns(new StockListItem(4, p, null, PurchaseEnum.Immediate, "100"));
            AddDiscountToProductSlave slave = new AddDiscountToProductSlave("X", userService.Object, handler.Object);
            slave.AddDiscountToProduct("item", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 10, "HIDDEN", true);
            Assert.AreEqual((int)DiscountStatus.Success, slave.answer.Status);
        }


        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
