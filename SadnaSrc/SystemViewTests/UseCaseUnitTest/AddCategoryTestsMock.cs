using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;
using SadnaSrc.MarketRecovery;

namespace SystemViewTests
{
    [TestClass]
    public class AddCategoryTestsMock
    {
        private Mock<IAdminDL> handler;
        private Mock<IMarketBackUpDB> marketDbMocker;
        private Mock<IUserAdmin> adminValidatorMocker;



        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketBackUpDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IAdminDL>();
            adminValidatorMocker = new Mock<IUserAdmin>();
        }
        [TestMethod]
        public void AddCategoryWhenCategoryAlreadyExists()
        {
            AddCategorySlave slave = new AddCategorySlave(handler.Object, adminValidatorMocker.Object);
            handler.Setup(x => x.GetCategoryByName("items")).Returns(new Category("items"));
            slave.AddCategory("items");
            Assert.AreEqual((int)EditCategoryStatus.CategoryAlradyExist, slave.Answer.Status);
        }
        [TestMethod]
        public void AddCategorySuccess()
        {
            AddCategorySlave slave = new AddCategorySlave(handler.Object, adminValidatorMocker.Object);
            slave.AddCategory("items");
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