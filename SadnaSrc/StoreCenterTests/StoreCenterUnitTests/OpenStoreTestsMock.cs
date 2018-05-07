using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]
    public class OpenStoreTestsMock
    {
        private Mock<IStoreDL> handler;
        private Mock<IUserShopper> userService;
        private Mock<IMarketDB> marketDbMocker;
        private OpenStoreSlave slave;
        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserShopper>();

            slave = new OpenStoreSlave(userService.Object, handler.Object);
        }

        [TestMethod]
        public void NotRegistered()
        {
            userService.Setup(x => x.ValidateRegistered()).Throws(new MarketException(0, ""));
            slave.OpenStore("newStoreName", "bla");
            Assert.AreEqual((int)OpenStoreStatus.InvalidUser, slave.Answer.Status);
        }
        [TestMethod]
        public void StoreNameTaken()
        {
            handler.Setup(x => x.GetStorebyName("newStoreName")).Returns(new Store("newStoreName", ""));
            slave.OpenStore("newStoreName", "bla");
            Assert.AreEqual((int)OpenStoreStatus.AlreadyExist, slave.Answer.Status);
        }
        [TestMethod]
        public void OpenStoreSuccess()
        {
            slave.OpenStore("newStoreName", "bla");
            Assert.AreEqual((int)OpenStoreStatus.Success, slave.Answer.Status);    
        }
    }
}
