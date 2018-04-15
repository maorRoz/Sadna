using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBoxUserTests
{
	[TestClass]
	public class UseCase1_1
	{
		private IUserBridge _bridge;

		[TestInitialize]
		public void MarketBuilder()
		{
			_bridge = UserDriver.getBridge();
		}

		[TestMethod]
		public void SuccessGuestEntry()
		{
			Assert.AreEqual((int)EnterSystemStatus.Success, _bridge.EnterSystem().Status);
		}

		[TestCleanup]
		public void UserTestCleanUp()
		{
			_bridge.CleanSession();
			_bridge.CleanMarket();

		}

	}
}
