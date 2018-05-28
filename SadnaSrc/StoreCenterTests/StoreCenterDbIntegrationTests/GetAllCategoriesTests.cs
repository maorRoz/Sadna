using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.StoreCenter;

namespace StoreCenterTests.StoreCenterDbIntegrationTests
{
	[TestClass]
	class GetAllCategoriesTests
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
	        string[] expected = { "WanderlandItems", "Books" };
	        userService.EnterSystem();
	        MarketAnswer ans = storeService.GetAllCategoryNames();
            Assert.AreEqual((int)GetCategoriesStatus.Success, ans.Status);
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
            Assert.AreEqual((int)GetCategoriesStatus.DidntEnterSystem, storeService.GetAllCategoryNames().Status);
	    }

	    [TestCleanup]
	    public void CleanUpOpenStoreTest()
	    {
	        MarketDB.Instance.CleanByForce();
	        MarketYard.CleanSession();
	    }
    }
}
