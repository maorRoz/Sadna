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
using SadnaSrc.StoreCenter;

namespace StoreCenterTests.StoreCenterUnitTests
{
	[TestClass]
	public class SearchProductTestsMock
	{
		private Mock<IStoreDL> _handler;
		private Mock<IUserShopper> _userShopper;
		private Mock<IMarketDB> _marketDbMocker;
		private SearchProductSlave _slave;

		[TestInitialize]
		public void BuildStore()
		{
			_marketDbMocker = new Mock<IMarketDB>();
			MarketException.SetDB(_marketDbMocker.Object);
			MarketLog.SetDB(_marketDbMocker.Object);
			_handler = new Mock<IStoreDL>();
			_userShopper = new Mock<IUserShopper>();
			_slave = new SearchProductSlave(_userShopper.Object, _handler.Object);
			/*_handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
			_handler.Setup(x => x.GetProductByNameFromStore("X", "NEWPROD")).Returns(prod);
			_handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
			_handler.Setup(x => x.GetProductFromStore("X", "NEWPROD")).Returns(stock);*/
		}

		/*[TestMethod]
		public void NoStore()
		{
			handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
			slave.RemoveProduct("NEWPROD");
			Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.Answer.Status);

		}

		[TestMethod]
		public void NoPermission()
		{
			userService.Setup(x => x.CanManageProducts()).Throws(new MarketException(0, ""));
			slave.RemoveProduct("NEWPROD");
			Assert.AreEqual((int)StoreEnum.NoPermission, slave.Answer.Status);
		}

		[TestMethod]
		public void NoProduct()
		{
			handler.Setup(x => x.GetProductByNameFromStore("X", "NEWPROD")).Returns((Product)null);
			slave.RemoveProduct("NEWPROD");
			Assert.AreEqual((int)StoreEnum.ProductNotFound, slave.Answer.Status);
		}

		[TestMethod]
		public void RemoveProductSuccess()
		{

			slave.RemoveProduct("NEWPROD");
			Assert.AreEqual((int)StoreEnum.Success, slave.Answer.Status);
		}

		[TestCleanup]
		public void CleanUpOpenStoreTest()
		{
			MarketDB.Instance.CleanByForce();
			MarketYard.CleanSession();
		}*/
	}
}

