using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
	public class GetAllDiscountCategoriesStoreTestsMock
	{
		private Mock<IStoreDL> _handler;
		private Mock<IUserShopper> _userShopper;
		private Mock<IMarketBackUpDB> _marketDbMocker;
		private GetAllDiscountCategoriesInStoreSlave _slave;

		[TestInitialize]
		public void BuildInitialize()
		{
			_marketDbMocker = new Mock<IMarketBackUpDB>();
			MarketException.SetDB(_marketDbMocker.Object);
			MarketLog.SetDB(_marketDbMocker.Object);
			_handler = new Mock<IStoreDL>();
			_userShopper = new Mock<IUserShopper>();
			_slave = new GetAllDiscountCategoriesInStoreSlave(_userShopper.Object, _handler.Object);
			_handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
		}

		[TestMethod]
		public void GetCategoriesTest()
		{
			string[] expectedDb = { "WanderlandItems", "Books" };
			_handler.Setup(x => x.GetAllCategorysNames()).Returns(expectedDb);
			string[] expectedCategoryNames = {  };
			_slave.GetAllDiscountCategoriesNameInStore("X");
			string[] actualCategoryNames = _slave.Answer.ReportList;
			Assert.AreEqual(expectedCategoryNames.Length, expectedCategoryNames.Length);
			for (int i = 0; i < expectedCategoryNames.Length; i++)
			{
				Assert.AreEqual(expectedCategoryNames[i], actualCategoryNames[i]);
			}
			Assert.AreEqual((int)GetCategoriesDiscountStatus.Success, _slave.Answer.Status);
		}

		[TestMethod]
		public void DidntEnterTest()
		{
			_userShopper.Setup(x => x.ValidateCanBrowseMarket()).Throws(new MarketException(0, ""));
			_slave.GetAllDiscountCategoriesNameInStore("X");
			Assert.AreEqual((int)GetCategoriesDiscountStatus.DidntEnterSystem, _slave.Answer.Status);
		}

		[TestMethod]
		public void NoStore()
		{
			_handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
			_slave.GetAllDiscountCategoriesNameInStore("X");
			Assert.AreEqual((int)GetCategoriesDiscountStatus.NoStore, _slave.Answer.Status);
		}

		[TestCleanup]
		public void CleanUpOpenStoreTest()
		{
			MarketDB.Instance.CleanByForce();
			MarketYard.CleanSession();
		}
	}
}
