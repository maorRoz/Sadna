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
using SadnaSrc.MarketData;
using SadnaSrc.MarketRecovery;

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]
    public class AddCategoryDiscountTestsMock
    {
        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IMarketBackUpDB> marketDbMocker;
        private AddCategoryDiscountSlave slave;

        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketBackUpDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
            slave = new AddCategoryDiscountSlave("WWW", userService.Object, handler.Object);
            MarketYard.SetDateTime(new DateTime(2018, 4, 14));
            Category category = new Category("C0","BLA");
            handler.Setup(x => x.GetCategoryByName("BLA")).Returns(category);
            CategoryDiscount nullValue = null;
            handler.Setup(x => x.GetCategoryDiscount("BLA", "WWW")).Returns(nullValue);
            handler.Setup(x => x.IsStoreExistAndActive("WWW")).Returns(true);
        }
        [TestMethod]
        public void NoStore()
        {
            handler.Setup(x => x.IsStoreExistAndActive("WWW")).Returns(false);
            slave.AddCategoryDiscount("BLA", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 10);
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.Answer.Status);
        }
        [TestMethod]
        public void NoPermission()
        {

            userService.Setup(x => x.CanDeclareDiscountPolicy()).Throws(new MarketException(0, ""));
            slave.AddCategoryDiscount("BLA", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 10);
            Assert.AreEqual((int)StoreEnum.NoPermission, slave.Answer.Status);
        }

        [TestMethod]
        public void NoCategory()
        {
            Category fail = null;
            handler.Setup(x => x.GetCategoryByName("BLA")).Returns(fail);
            slave.AddCategoryDiscount("BLA", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 10);
            Assert.AreEqual((int)StoreEnum.CategoryNotExistsInSystem, slave.Answer.Status);
        }

        [TestMethod]
        public void BadDiscountDates1()
        {
            slave.AddCategoryDiscount("BLA", DateTime.Parse("20/01/2019"), DateTime.Parse("01/01/2019"), 10);
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, slave.Answer.Status);
        }

        [TestMethod]
        public void DiscountAmountIs100()
        {
            slave.AddCategoryDiscount("BLA", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 100);
            Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, slave.Answer.Status);
        }

        [TestMethod]
        public void DisocuntAmountIsBigger100()
        {
            slave.AddCategoryDiscount("BLA", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 200);
            Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, slave.Answer.Status);
        }

        [TestMethod]
        public void DisocuntAmountIsNegative()
        {
            slave.AddCategoryDiscount("BLA", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), -5);
            Assert.AreEqual((int)DiscountStatus.DiscountAmountIsNegativeOrZero, slave.Answer.Status);
        }

        [TestMethod]
        public void DisocuntAmountIsZero()
        {
            slave.AddCategoryDiscount("BLA", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 0);
            Assert.AreEqual((int)DiscountStatus.DiscountAmountIsNegativeOrZero, slave.Answer.Status);
        }

        [TestMethod]
        public void AddCategoryDiscountSuccess()
        {
            slave.AddCategoryDiscount("BLA", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 10);
            Assert.AreEqual((int)DiscountStatus.Success, slave.Answer.Status);
        }


        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }


    }
}
