using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using Moq;


namespace StoreCenterTests.StoreCenterUnitTests
{
    [TestClass]
    public class RemoveCategoryTestsMock
    {
        private Mock<IStoreDL> _handler;
        private Mock<IUserSeller> _userService;
        private Mock<IMarketDB> _marketDbMocker;
        
        [TestInitialize]
        public void BuildStore()
        {
            _marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(_marketDbMocker.Object);
            MarketLog.SetDB(_marketDbMocker.Object);
            _handler = new Mock<IStoreDL>();
            _userService = new Mock<IUserSeller>();
        }
        [TestMethod]
        public void RemoveCategoryWhenStoreNotExists()
        {
            RemoveCategorySlave slave = new RemoveCategorySlave("noStore", _userService.Object, _handler.Object);
            slave.RemoveCategory("items");
            Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.Answer.Status);

        }
        [TestMethod]
        public void RemoveCategoryWhenHasNoPremmision()
        {

            RemoveCategorySlave slave = new RemoveCategorySlave("T", _userService.Object, _handler.Object);
            _handler.Setup(x => x.IsStoreExistAndActive("T")).Returns(true);
            _handler.Setup(x => x.GetStorebyName("T")).Returns(new Store("S7", "T", "bla"));
            _userService.Setup(x => x.CanManageProducts()).Throws(new MarketException(9, "bla"));
            slave.RemoveCategory("items");
            Assert.AreEqual((int)StoreEnum.NoPremmision, slave.Answer.Status);
        }
        [TestMethod]
        public void RemoveCategoryWhenCategoryNotExists()
        {
            RemoveCategorySlave slave = new RemoveCategorySlave("T", _userService.Object, _handler.Object);
            _handler.Setup(x => x.IsStoreExistAndActive("T")).Returns(true);
            _handler.Setup(x => x.GetStorebyName("T")).Returns(new Store("S7", "T", "bla"));
            slave.RemoveCategory("items");
            Assert.AreEqual((int)StoreEnum.CategoryNotExistsInStore, slave.Answer.Status);
        }
        [TestMethod]
        public void RemoveCategorySuccess()
        {
            RemoveCategorySlave slave = new RemoveCategorySlave("T", _userService.Object, _handler.Object);
            _handler.Setup(x => x.IsStoreExistAndActive("T")).Returns(true);
            _handler.Setup(x => x.GetStorebyName("T")).Returns(new Store("S7", "T", "bla"));
            _handler.Setup(x => x.getCategoryByName("items")).Returns(new Category("items"));
            slave.RemoveCategory("items");
            Assert.AreEqual((int)StoreEnum.Success, slave.Answer.Status);
        }
        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}