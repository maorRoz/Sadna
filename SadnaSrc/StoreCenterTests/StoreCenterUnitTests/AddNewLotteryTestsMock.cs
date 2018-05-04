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
        Mock<IUserSeller> userService;
        [TestInitialize]
        public void BuildStore()
        {
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
        }
        [TestMethod]
        public void AddNewLotteryFail()
        {
            AddNewLotterySlave slave = new AddNewLotterySlave("bla", userService.Object, handler.Object);
            slave.AddNewLottery("name0", 1, "des", DateTime.Parse("30/10/2019"), DateTime.Parse("30/12/2019"));
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.answer.Status);

        }
        [TestMethod]
        public void AddNewLotterySuccess()
        {
            handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
            handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
            AddNewLotterySlave slave = new AddNewLotterySlave("X", userService.Object, handler.Object);
            slave.AddNewLottery("NEWPROD", 1, "des", DateTime.Parse("30/10/2019"), DateTime.Parse("30/12/2019"));
            Assert.AreEqual((int)StoreEnum.Success, slave.answer.Status);

        }
    }
}
