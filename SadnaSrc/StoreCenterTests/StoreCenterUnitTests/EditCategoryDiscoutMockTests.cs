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
using SadnaSrc.MarketData;
using SadnaSrc.MarketRecovery;

namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]

    public class EditCategoryDisocuntMockTests
    {
        private Mock<IStoreDL> handler;
        private Mock<IUserSeller> userService;
        private Mock<IMarketBackUpDB> marketDbMocker;
        private EditCategoryDiscountSlave slave;
        private Product prod;
        private CategoryDiscount categoryDiscount;
        private StockListItem stock;
        private Category category;

        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketBackUpDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IStoreDL>();
            userService = new Mock<IUserSeller>();
            slave = new EditCategoryDiscountSlave("WWW", userService.Object, handler.Object);
            MarketYard.SetDateTime(new DateTime(2018, 4, 14));
            category = new Category("C0", "BLA");
            handler.Setup(x => x.GetCategoryByName("BLA")).Returns(category);
            categoryDiscount = new CategoryDiscount("d0", "BLA", "WWW", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 10);
            handler.Setup(x => x.GetCategoryDiscount("BLA", "WWW")).Returns(categoryDiscount);
            handler.Setup(x => x.IsStoreExistAndActive("WWW")).Returns(true);
        }
        [TestMethod]
        public void NoStore()
        {
            handler.Setup(x => x.IsStoreExistAndActive("WWW")).Returns(false);
            slave.EditCategoryDiscount("BLA","Start Date" ,"20/01/2019");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.Answer.Status);
        }
        [TestMethod]
        public void NoPermission()
        {

            userService.Setup(x => x.CanDeclareDiscountPolicy()).Throws(new MarketException(0, ""));
            slave.EditCategoryDiscount("BLA", "Start Date", "20/01/2019");
            Assert.AreEqual((int)StoreEnum.NoPermission, slave.Answer.Status);
        }

        [TestMethod]
        public void NoCategory()
        {
            Category fail = null;
            handler.Setup(x => x.GetCategoryByName("BLA")).Returns(fail);
            slave.EditCategoryDiscount("BLA", "Start Date", "20/01/2019");
            Assert.AreEqual((int)StoreEnum.CategoryNotExistsInSystem, slave.Answer.Status);
        }

        [TestMethod]
        public void BadDiscountDates1()
        {
            slave.EditCategoryDiscount("BLA", "Start Date", "20/01/2119");
            Assert.AreEqual((int)DiscountStatus.DatesAreWrong, slave.Answer.Status);
        }

        [TestMethod]
        public void DiscountAmountIs100()
        {
            slave.EditCategoryDiscount("BLA", "DiscountAmount", "100");
            Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, slave.Answer.Status);
        }

        [TestMethod]
        public void DisocuntAmountIsBigger100()
        {
            slave.EditCategoryDiscount("BLA", "DiscountAmount", "150");
            Assert.AreEqual((int)DiscountStatus.AmountIsHundredAndpresenteges, slave.Answer.Status);
        }

        [TestMethod]
        public void DisocuntAmountIsNegative()
        {
            slave.EditCategoryDiscount("BLA", "DiscountAmount", "-100");
            Assert.AreEqual((int)DiscountStatus.DiscountAmountIsNegativeOrZero, slave.Answer.Status);
        }

        [TestMethod]
        public void DisocuntAmountIsZero()
        {
            slave.EditCategoryDiscount("BLA", "DiscountAmount", "0");
            Assert.AreEqual((int)DiscountStatus.DiscountAmountIsNegativeOrZero, slave.Answer.Status);
        }

        [TestMethod]
        public void AddCategoryDiscountSuccess()
        {
            slave.EditCategoryDiscount("BLA", "DiscountAmount", "20");
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
