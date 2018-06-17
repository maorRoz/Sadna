using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace StoreCenterTests.StoreCenterDbIntegrationTests
{
	[TestClass]
	public class GetAllDiscountCategoriesTests
	{
		private MarketYard market;
		private IStoreShoppingService storeService;
		IUserService userService;


		[TestInitialize]
		public void BuildInitialize()
		{
			MarketDB.Instance.InsertByForce();
			market = MarketYard.Instance;
			userService = market.GetUserService();
			storeService = market.GetStoreShoppingService(ref userService);
		}

		[TestMethod]
		public void GetCategoriesTest()
		{
			string[] expected = {  };
			userService.EnterSystem();
			MarketAnswer ans = storeService.GetAllDiscountCategoriesInStore("X");
			Assert.AreEqual((int)GetCategoriesDiscountStatus.Success, ans.Status);
			string[] actual = ans.ReportList;
			Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}

		[TestMethod]
		public void DidntEnterTest()
		{
			Assert.AreEqual((int)GetCategoriesDiscountStatus.DidntEnterSystem, storeService.GetAllDiscountCategoriesInStore("X").Status);
		}

		[TestMethod]
		public void ViewStoreStoreNotFound()
		{
			MarketAnswer ans = storeService.GetAllDiscountCategoriesInStore("notStore");
			Assert.AreEqual((int)GetCategoriesDiscountStatus.NoStore, ans.Status);
		}

		[TestCleanup]
		public void CleanUpOpenStoreTest()
		{
			MarketDB.Instance.CleanByForce();
			MarketYard.CleanSession();
		}
	}
}
