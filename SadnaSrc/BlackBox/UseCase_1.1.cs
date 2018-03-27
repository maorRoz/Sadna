using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackBox
{
	[TestClass]
	class UseCase_1
	{
		private UserBridge bridge;

		[TestInitialize]
		public void MarketBuilder()
		{
			bridge = Driver.getBridge();
		}

		[TestMethod]
		public void successGuestEntry()
		{
			Assert.Equals(bridge.EnterSystem(), "You've been entered the system successfully!");
		}

	}
}
