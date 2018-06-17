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

    public class RemoveCategoryDiscountMockTests
    {

        private Mock<IStoreDL> _handler;
        private Mock<IUserSeller> _userService;
        private Mock<IMarketBackUpDB> _marketDbMocker;
        private RemoveCategoryDiscountSlave _slave;
        private Category _category;
        private CategoryDiscount _categoryDiscount;


        [TestInitialize]
        public void BuildStore()
        {
            _marketDbMocker = new Mock<IMarketBackUpDB>();
            MarketException.SetDB(_marketDbMocker.Object);
            MarketLog.SetDB(_marketDbMocker.Object);
            _handler = new Mock<IStoreDL>();
            _userService = new Mock<IUserSeller>();
            _slave = new RemoveCategoryDiscountSlave("WWW", _userService.Object, _handler.Object);
            MarketYard.SetDateTime(new DateTime(2018, 4, 14));
            _category = new Category("C0", "BLA");
            _handler.Setup(x => x.GetCategoryByName("BLA")).Returns(_category);
            _categoryDiscount = new CategoryDiscount("d0","BLA","WWW", DateTime.Parse("01/01/2019"), DateTime.Parse("20/01/2019"), 10);
            _handler.Setup(x => x.GetCategoryDiscount("BLA", "WWW")).Returns(_categoryDiscount);
            _handler.Setup(x => x.IsStoreExistAndActive("WWW")).Returns(true);
        }
        [TestMethod]
        public void NoStore()
        {
            _handler.Setup(x => x.IsStoreExistAndActive("WWW")).Returns(false);
            _slave.RemoveCategoryDiscount("BLA");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, _slave.Answer.Status);

        }

        [TestMethod]
        public void NoPermission()
        {
            _userService.Setup(x => x.CanDeclareDiscountPolicy()).Throws(new MarketException(0, ""));
            _slave.RemoveCategoryDiscount("BLA");
            Assert.AreEqual((int)StoreEnum.NoPermission, _slave.Answer.Status);
        }

        [TestMethod]
        public void NoCategory()
        {
            Category nullvalue = null;
            _handler.Setup(x => x.GetCategoryByName("BLA")).Returns(nullvalue);
            _slave.RemoveCategoryDiscount("BLA");
            Assert.AreEqual((int)StoreEnum.CategoryNotExistsInSystem, _slave.Answer.Status);
        }
        [TestMethod]
        public void NoCategoryDiscount()
        {
            CategoryDiscount nullvalue = null;
            _handler.Setup(x => x.GetCategoryDiscount("BLA", "WWW")).Returns(nullvalue);
            _slave.RemoveCategoryDiscount("BLA");
            Assert.AreEqual((int)StoreEnum.CategoryDiscountNotExistsInStore, _slave.Answer.Status);
        }

        [TestMethod]
        public void RemoveDiscountSuccess()
        {

            _slave.RemoveCategoryDiscount("BLA");
            Assert.AreEqual((int)DiscountStatus.Success, _slave.Answer.Status);

        }
    }
}
