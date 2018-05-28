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
			Product[] expected =
			{
				new Product("P1", "BOX", 100, "this is a plastic box"),
				new Product("P10", "LittleCake", 100, "eat my"),
				new Product("P11", "LittleDrink", 200, "drink my"),
				new Product("P12", "CheshireCat", 200, "smile"),
				new Product("P13", "WhiteRabbit", 200, "you are late"),
				new Product("P14", "RedQueen", 200, "Cutoff his head"),
				new Product("P15", "Time", 200, "Dont kill my"),
				new Product("P16", "The March Hare", 200, "Tea?"),
				new Product("P17", "nonsense ", 200, "no sense!"),
				new Product("P18", "Pizza", 60, "food"),
				new Product("P19", "#9", 5, "its just a fucking burger, ok?"),
				new Product("P2", "Golden BOX", 1000, "this is a golden box"),
				new Product("P20", "#45 With Cheese", 18, "its just a fucking cheesburger, ok?"),
				new Product("P21", "Fraid Egg", 10, "yami"),
				new Product("P22", "OnePunchManPoster", 10, "yami"),
				new Product("P3", "DeleteMy BOX", 10, "this is a trush"),
				new Product("P4", "Bamba", 6, "munch"),
				new Product("P5", "Goldstar", 11, "beer"),
				new Product("P6", "OCB", 10, "accessories"),
				new Product("P7", "Coated Peanuts", 10, "munch"),
				new Product("P8", "Alice", 10, "popo"),
				new Product("P9", "TheHatter", 10, "popo"),

			};
			_handler.Setup(x => x.GetAllProducts()).Returns(expected);
		}

		[TestMethod]
		public void SearchByNameNoFilteringSuccessTest()
		{
			Product[] product = {new Product("P1", "BOX", 100, "this is a plastic box")};
			_handler.Setup(x => x.GetProductsByName("Box")).Returns(product);
			_slave.SearchProduct("Name", "BOX", 0,0,"None");
			string[] expected = {new Product("P1", "BOX", 100, "this is a plastic box").ToString()};
			string[] received = _slave.Answer.ReportList;
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], received[i]);
			}
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

	*/
		[TestMethod]
		public void NoProduct()
		{
			/*handler.Setup(x => x.GetProductByNameFromStore("X", "NEWPROD")).Returns((Product)null);
			slave.RemoveProduct("NEWPROD");
			Assert.AreEqual((int)StoreEnum.ProductNotFound, slave.Answer.Status);*/
		}

	
		[TestMethod]
		public void NullDataGiven()
		{
			_slave.SearchProduct("Name","",0,0,"None");
			Assert.AreEqual((int)SearchProductStatus.NullValue, _slave.Answer.Status);
		}

	
		[TestCleanup]
		public void CleanUpOpenStoreTest()
		{
			MarketDB.Instance.CleanByForce();
			MarketYard.CleanSession();
		}
	}
}

