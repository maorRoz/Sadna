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

		public void RegistrationSucceeded()
		{
			_bridge.EnterSystem();
			string res = _bridge.SignUp("Pnina", "miahol susia 12", "123456");
			Assert.AreEqual(res, "Sign up has been successfull!");

		}

		[TestMethod]

		public void RegistrationWithoutEnteringTheSystem()
		{
			//string res = _bridge.SignUp("Pnina", "miahol susia 12", "123456");
			//Assert.AreEqual(res, "sign up action has been requested by user which hasn't fully entered the system yet!");
		}

		[TestCleanup]
		public void UserTestCleanUp()
		{
			//delete the user that has been added


		}
	}
}
