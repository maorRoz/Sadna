using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.UserSpot;

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

		public void RegistrationSucceeded()
		{
			_bridge.EnterSystem();
			Assert.AreEqual((int)SignUpStatus.Success, _bridge.SignUp("Pnina", "miahol susia 12", "123456"));
		}

		[TestMethod]

		public void RegistrationWithoutEnteringTheSystem()
		{
			Assert.AreEqual((int)SignUpStatus.DidntEnterSystem, _bridge.SignUp("Pnina", "miahol susia 12", "123456"));
		}

		/*[TestMethod]
	
		public void RegistrationWithoutEnteringTheSystem()
		{
			Assert.AreEqual((int)SignUpStatus.DidntEnterSystem, _bridge.SignUp("Pnina", "miahol susia 12", "123456"));
		}
		*/

		[TestCleanup]
		public void UserTestCleanUp()
		{
			//delete the user that has been added


		}
	}
}
