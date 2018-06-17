using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;
using SadnaSrc.MarketRecovery;
using SadnaSrc.StoreCenter;

namespace StoreCenterTests.StoreCenterUnitTests
{
	[TestClass]
	class GetAllCategoriesTestsMock
	{
		private Mock<IStoreDL> _handler;
		private Mock<IUserShopper> _userShopper;
		private Mock<IMarketBackUpDB> _marketDbMocker;
		private GetAllCategoryNamesSlave _slave;

		[TestInitialize]
		public void BuildInitialize()
		{
			_marketDbMocker = new Mock<IMarketBackUpDB>();
			MarketException.SetDB(_marketDbMocker.Object);
			MarketLog.SetDB(_marketDbMocker.Object);
			_handler = new Mock<IStoreDL>();
			_userShopper = new Mock<IUserShopper>();
			_slave = new GetAllCategoryNamesSlave(_userShopper.Object, _handler.Object);
		}

		[TestMethod]
		public void GetCategoriesTest()
		{
			string[] expectedDb = { "WanderlandItems", "Books" };
			_handler.Setup(x => x.GetAllCategorysNames()).Returns(expectedDb);
			string[] expectedCategoryNames = {"WanderlandItems", "Books"};
			_slave.GetAllCategoryNames();
			string[] actualCategoryNames = _slave.Answer.ReportList;
			Assert.AreEqual(expectedCategoryNames.Length, expectedCategoryNames.Length);
			for (int i = 0; i < expectedCategoryNames.Length; i++)
			{
				Assert.AreEqual(expectedCategoryNames[i], actualCategoryNames[i]);
			}
			Assert.AreEqual((int)GetCategoriesStatus.Success, _slave.Answer.Status);
		}

		[TestMethod]
		public void DidntEnterTest()
		{
			_userShopper.Setup(x => x.ValidateCanBrowseMarket()).Throws(new MarketException(0, ""));
			_slave.GetAllCategoryNames();
			Assert.AreEqual((int)GetCategoriesStatus.DidntEnterSystem, _slave.Answer.Status);
		}

		[TestCleanup]
		public void CleanUpOpenStoreTest()
		{
			MarketDB.Instance.CleanByForce();
			MarketYard.CleanSession();
		}
	}
}
