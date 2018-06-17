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
	public class GetProductInfoTests
	{
		private MarketYard market;
		private IStoreDL handler;
		IUserService userService;
		[TestInitialize]
		public void BuildStore()
		{
			MarketDB.Instance.InsertByForce();
			market = MarketYard.Instance;
			handler = StoreDL.Instance;
			userService = market.GetUserService();
			userService.EnterSystem();
		}
		[TestMethod]
		public void GetProductInfoStoreNotExists()
		{
			userService.SignIn("Arik1", "123");
			StoreManagementService service = (StoreManagementService)market.GetStoreManagementService(userService, "storeNotExists");
			MarketAnswer ans = service.GetProductInfo("name0");
			Assert.AreEqual((int)StoreEnum.StoreNotExists, ans.Status);
		}

		[TestMethod]
		public void GetProductInfoNoPremmision()
		{
			userService.SignIn("Big Smoke", "123");
			StoreManagementService service = (StoreManagementService)market.GetStoreManagementService(userService, "X");
			MarketAnswer ans = service.GetProductInfo("name0");
			Assert.AreEqual((int)ViewProductInfoStatus.NoAuthority, ans.Status);
		}

		[TestMethod]
		public void ProductInfoProductNotAvailable()
		{
			userService.SignIn("Arik1", "123");
			StoreManagementService service = (StoreManagementService)market.GetStoreManagementService(userService, "X");
			MarketAnswer ans = service.GetProductInfo("name0");
			Assert.AreEqual((int)StoreEnum.ProductNotFound, ans.Status);
		}

		[TestMethod]
		public void GetProductInfoSuccess()
		{
			userService.SignIn("Arik1", "123");
			StoreManagementService service = (StoreManagementService)market.GetStoreManagementService(userService, "X");
			service.AddNewProduct("GOLD", 5, "NONO", 8);
			MarketAnswer ans = service.GetProductInfo("GOLD");
			Assert.AreEqual((int)ViewProductInfoStatus.Success, ans.Status);
			string expected = " name: GOLD base price: 5 description: NONO";
			Assert.AreEqual(expected, ans.ReportList[0]);
		}


		[TestCleanup]
		public void CleanUpOpenStoreTest()
		{
			MarketDB.Instance.CleanByForce();
			MarketYard.CleanSession();
		}
	}
}
