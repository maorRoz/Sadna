using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackBox
{
	[TestClass]
	public class UseCase1_2
	{
		private UserBridge _bridge;

		[TestInitialize]
		public void MarketBuilder()
		{
			_bridge = new RealBridge();
		}
		[TestMethod]
		public void TestMethod1()
		{

		}

		[TestCleanup]
		public void UserTestCleanUp()
		{

		}
	}
}
