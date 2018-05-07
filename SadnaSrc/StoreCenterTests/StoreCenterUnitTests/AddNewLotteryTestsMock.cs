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
    public class AddNewLotteryTestsMock
    {

        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IMarketDB> marketDbMocker;
        private Product prod;
        private AddNewLotterySlave slave;

        //TODO: improve this

        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
            MarketYard.SetDateTime(new DateTime(2018, 4, 14));
            prod = new Product("item", 1, "des");
            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
            handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns((Product) null);
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            handler.Setup(x => x.GetProductFromStore("X", "item")).Returns(new StockListItem(4, prod, null, PurchaseEnum.Immediate, "100"));
            slave = new AddNewLotterySlave("X", userService.Object, handler.Object);
        }

        [TestMethod]
        public void NoStore()
        {
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
            slave.AddNewLottery("item", 1, "des", DateTime.Parse("30/10/2019"), DateTime.Parse("30/12/2019"));
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);
        }

        [TestMethod]
        public void NoPermission()
        {
            userService.Setup(x => x.CanManageProducts()).Throws(new MarketException(0, ""));
            slave.AddNewLottery("item", 1, "des", DateTime.Parse("30/10/2019"), DateTime.Parse("30/12/2019"));
            Assert.AreEqual((int)StoreEnum.NoPermission, slave.answer.Status);
        }

        [TestMethod]
        public void ProductNameTaken()
        {
            handler.Setup(x => x.GetProductByNameFromStore("X", "item")).Returns(prod);
            slave.AddNewLottery("item", 1, "des", DateTime.Parse("30/10/2019"), DateTime.Parse("30/12/2019"));
            Assert.AreEqual((int)StoreEnum.ProductNameNotAvlaiableInShop, slave.answer.Status);
        }
        
        [TestMethod]
        public void BadLotteryDates1()
        {
            slave.AddNewLottery("item", 1, "des", DateTime.Parse("30/12/2019"), DateTime.Parse("30/12/2019"));
            Assert.AreEqual((int)StoreEnum.DatesAreWrong, slave.answer.Status);
        }

     
        [TestMethod]
        public void AddNewLotterySuccess()
        {
            slave.AddNewLottery("item", 1, "des", DateTime.Parse("30/10/2019"), DateTime.Parse("30/12/2019"));
            Assert.AreEqual((int)StoreEnum.Success, slave.answer.Status);

        }
    }
}
