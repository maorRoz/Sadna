using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.UserSpot;


namespace BlackBoxUserTests
{
	[TestClass]
	public class UseCase1_1
	{
		private UserBridge _bridge;

		[TestInitialize]
		public void MarketBuilder()
		{
			_bridge = new RealBridge();
		}

		[TestMethod]
		public void SuccessGuestEntry()
		{
			Assert.AreEqual((int)EnterSystemStatus.Success, _bridge.EnterSystem().Status);
		}

	}
}
