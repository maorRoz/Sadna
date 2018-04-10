using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackBoxUserTests
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
