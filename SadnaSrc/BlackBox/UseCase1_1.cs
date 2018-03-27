using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackBox
{
	[TestClass]
	public class UseCase1_1
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

			Assert.AreEqual(bridge.EnterSystem(), "You've been entered the system successfully!");
		}

	}
}
