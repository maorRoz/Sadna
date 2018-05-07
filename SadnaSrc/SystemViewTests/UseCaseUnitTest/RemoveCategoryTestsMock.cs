using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SystemViewTests
{
    [TestClass]
    public class RemoveCategoryTestsMock
    {
        private Mock<IAdminDL> _handler;
        private Mock<IUserSeller> _userService;
        private Mock<IMarketDB> _marketDbMocker;
        
        [TestInitialize]
        public void BuildStore()
        {
            _marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(_marketDbMocker.Object);
            MarketLog.SetDB(_marketDbMocker.Object);
            _handler = new Mock<IAdminDL>();
            _userService = new Mock<IUserSeller>();
        }
        [TestMethod]
        public void RemoveCategoryWhenCategoryNotExists()
        {
            RemoveCategorySlave slave = new RemoveCategorySlave(_handler.Object);
            slave.RemoveCategory("items");
            Assert.AreEqual((int)EditCategoryStatus.CategoryNotExistsInSystem, slave.Answer.Status);
        }
        [TestMethod]
        public void RemoveCategorySuccess()
        {
            RemoveCategorySlave slave = new RemoveCategorySlave(_handler.Object);
            _handler.Setup(x => x.GetCategoryByName("items")).Returns(new Category("items"));
            slave.RemoveCategory("items");
            Assert.AreEqual((int)EditCategoryStatus.Success, slave.Answer.Status);
        }
        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}