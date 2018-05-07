using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using System;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]
    public class RemoveDiscountFromProductTestsMock
    {

        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IMarketDB> marketDbMocker;
        private Product prod;
        private Discount discount;
        private StockListItem stock;
        private RemoveDiscountFromProductSlave slave;


        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
            prod = new Product("NEWPROD", 150, "desc");
            discount = new Discount(DiscountTypeEnum.Visible, DateTime.Parse("03/05/2020"), DateTime.Parse("30/06/2020"), 50, false);
            stock = new StockListItem(10, prod, discount, PurchaseEnum.Immediate, "BLA");
            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
            handler.Setup(x => x.GetProductByNameFromStore("X", "NEWPROD")).Returns(prod);
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            handler.Setup(x => x.GetProductFromStore("X", "NEWPROD")).Returns(stock);
            slave = new RemoveDiscountFromProductSlave("X", userService.Object, handler.Object);
        }
        [TestMethod]
        public void NoStore()
        {
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
            slave.RemoveDiscountFromProduct("NEWPROD");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.Answer.Status);
            
        }

        [TestMethod]
        public void NoPermission()
        {
            userService.Setup(x => x.CanDeclareDiscountPolicy()).Throws(new MarketException(0, ""));
            slave.RemoveDiscountFromProduct("NEWPROD");
            Assert.AreEqual((int)StoreEnum.NoPermission, slave.Answer.Status);
        }

        [TestMethod]
        public void NoProduct()
        {
            handler.Setup(x => x.GetProductByNameFromStore("X", "NEWPROD")).Returns((Product)null);
            slave.RemoveDiscountFromProduct("NEWPROD");
            Assert.AreEqual((int)StoreEnum.ProductNotFound, slave.Answer.Status);
        }

        [TestMethod]
        public void RemoveDiscountSuccess()
        {
           
            slave.RemoveDiscountFromProduct("NEWPROD");
            Assert.AreEqual((int)DiscountStatus.Success, slave.Answer.Status);

        }
    }
}
