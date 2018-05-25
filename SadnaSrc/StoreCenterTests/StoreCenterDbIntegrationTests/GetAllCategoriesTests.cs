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
		public Store toDeleteStore;
		private IStoreDL handler;
		IUserService userService;
		[TestInitialize]
		public void BuildInitialize()
		{
			MarketDB.Instance.InsertByForce();
			market = MarketYard.Instance;
			handler = StoreDL.Instance;
			userService = market.GetUserService();
		}
	}
}
