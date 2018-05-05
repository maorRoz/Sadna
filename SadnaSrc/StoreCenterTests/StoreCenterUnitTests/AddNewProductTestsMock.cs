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
    public class AddNewProductTestsMock
    {

        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IMarketDB> marketDbMocker;

        //TODO: improve this


        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
        }
        [TestMethod]
        public void AddProductFail()
        {
            AddNewProductSlave slave = new AddNewProductSlave(userService.Object, "bla", handler.Object);
            slave.AddNewProduct("p", 9, "bla", 4);
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.Answer.Status);

        }
        [TestMethod]
        public void AddProductSuccess()
        {

            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            AddNewProductSlave slave = new AddNewProductSlave(userService.Object, "X", handler.Object);
            slave.AddNewProduct("p", 9, "bla", 4);
            Assert.AreEqual((int)StoreEnum.Success, slave.Answer.Status);

        }
    }
}
