using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SystemViewTests
{
    [TestClass]
    public class AddCategoryTestsMock
    {
        private Mock<IAdminDL> handler;
        private Mock<IMarketDB> marketDbMocker;



        [TestInitialize]
        public void BuildStore()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            handler = new Mock<IAdminDL>();
        }
        [TestMethod]
        public void AddCategoryWhenCategoryAlreadyExists()
        {
            AddCategorySlave slave = new AddCategorySlave(handler.Object);
            handler.Setup(x => x.GetCategoryByName("items")).Returns(new Category("items"));
            slave.AddCategory("items");
            Assert.AreEqual((int)EditCategoryStatus.CategoryAlradyExist, slave.Answer.Status);
        }
        [TestMethod]
        public void AddCategorySuccess()
        {
            AddCategorySlave slave = new AddCategorySlave(handler.Object);
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