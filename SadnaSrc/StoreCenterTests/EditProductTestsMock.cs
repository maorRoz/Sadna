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

namespace StoreCenterTests
{
    [TestClass]

    public class EditProductTestsMock
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
        public void EditProductFail()
        {
            EditProductSlave slave = new EditProductSlave("noStore", userService.Object, handler.Object);
            slave.EditProduct("BOX", "price", "10");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
        }
        [TestMethod]
        public void EditProductPass()
        {
            Product P = new Product("NEWPROD", 150, "desc");
            Discount discount = new Discount(discountTypeEnum.Visible, DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, false);
            StockListItem SLI = new StockListItem(10, P, discount, PurchaseEnum.Immediate, "BLA");
            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
            handler.Setup(x => x.GetProductByNameFromStore("X", "NEWPROD")).Returns(P);
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            handler.Setup(x => x.GetProductFromStore("X", "NEWPROD")).Returns(SLI);
            EditProductSlave slave = new EditProductSlave("X", userService.Object, handler.Object);
            slave.EditProduct("NEWPROD", "BasePrice", "10");
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
