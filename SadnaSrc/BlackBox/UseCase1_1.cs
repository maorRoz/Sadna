using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackBoxUserTests
{
	[TestClass]
	public class UseCase1_1
	{
		private UserBridge _bridge;

		[TestInitialize]
		public void MarketBuilder()
		{
			_bridge = Driver.getBridge();
		}

		[TestMethod]
		public void successGuestEntry()
		{

			Assert.AreEqual(_bridge.EnterSystem(), "You've been entered the system successfully!");
		}

	}
}
