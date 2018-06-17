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
	public class GetProductInfoMockTests
	{
		private Mock<IStoreDL> handler;
		private Mock<IUserSeller> userService;
		private Mock<IMarketBackUpDB> marketDbMocker;
		private GetProductInfoSlave slave;
		private Product prod;

		[TestInitialize]
		public void BuildStore()
		{
			marketDbMocker = new Mock<IMarketBackUpDB>();
			MarketException.SetDB(marketDbMocker.Object);
			MarketLog.SetDB(marketDbMocker.Object);
			handler = new Mock<IStoreDL>();
			userService = new Mock<IUserSeller>();
			slave = new GetProductInfoSlave("X", userService.Object, handler.Object);
			prod = new Product("NEWPROD", 150, "desc");
			handler.Setup(x => x.GetStorebyName("X")).Returns(new Store("X", ""));
			handler.Setup(x => x.GetProductByNameFromStore("X", "NEWPROD")).Returns(prod);
			handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(true);
		}

		[TestMethod]
		public void NoStore()
		{
			handler.Setup(x => x.IsStoreExistAndActive("X")).Returns(false);
			slave.GetProductInfo("BOX");
			Assert.AreEqual((int)StoreEnum.StoreNotExists, slave.Answer.Status);
		}

		[TestMethod]
		public void NoPermission()
		{
			userService.Setup(x => x.CanManageProducts()).Throws(new MarketException(0, ""));
			slave.GetProductInfo("BOX");
			Assert.AreEqual((int)ViewProductInfoStatus.NoAuthority, slave.Answer.Status);
		}

		[TestMethod]
		public void NoProduct()
		{
			handler.Setup(x => x.GetProductByNameFromStore("X", "NEWPROD")).Returns((Product)null);
			slave.GetProductInfo("NEWPROD");
			Assert.AreEqual((int)StoreEnum.ProductNotFound, slave.Answer.Status);
		}

		[TestMethod]
		public void GetStoreInfoSuccess()
		{
			slave.GetProductInfo("NEWPROD");
			Assert.AreEqual((int)ViewProductInfoStatus.Success, slave.Answer.Status);
			string expected = " name: NEWPROD base price: 150 description: desc";
			Assert.AreEqual(expected, slave.Answer.ReportList[0]);
		}

		[TestCleanup]
		public void CleanUpOpenStoreTest()
		{
			MarketDB.Instance.CleanByForce();
			MarketYard.CleanSession();
		}
	}
}
