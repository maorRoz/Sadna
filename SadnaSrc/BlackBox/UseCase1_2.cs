using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace BlackBox
{
	[TestClass]
	public class UseCase1_2
	{
		private UserBridge _bridge;
		private UserBridge _bridge2;

		[TestInitialize]
		public void MarketBuilder()
		{
			_bridge = new RealBridge();
			_bridge2 = null;
		}
		[TestMethod]

		public void RegistrationSucceeded()
		{
			_bridge.EnterSystem();
			Assert.AreEqual((int)SignUpStatus.Success, _bridge.SignUp("PninaBashkanski", "miahol susia 12", "123456").Status);
		}

		[TestMethod]

		public void RegistrationWithoutEnteringTheSystem()
		{
			Assert.AreEqual((int)SignUpStatus.DidntEnterSystem, _bridge.SignUp("Pnina", "miahol susia 12", "123456").Status);
		}

		[TestMethod]
	
		public void RegistrationWithATakenName()
		{
			_bridge.EnterSystem();
			_bridge.SignUp("Pnina", "miahol susia 12", "123456");
			_bridge2 = new RealBridge();
			_bridge2.EnterSystem();
			Assert.AreEqual((int)SignUpStatus.TakenName, _bridge2.SignUp("Pnina", "miahol susia 12", "123456").Status);
		}

		[TestMethod]

		public void SignUpMoreThanOnce()
		{
			_bridge.EnterSystem();
			_bridge.SignUp("PninaBas", "mishol susia 8", "123852");
			Assert.AreEqual((int)SignUpStatus.SignedUpAlready, _bridge.SignUp("PninaBas", "mishol susia 8", "123852").Status);

		}

		[TestMethod]

		public void DataToSignUpIsNull()
		{
			_bridge.EnterSystem();
			Assert.AreEqual((int)SignUpStatus.NullEmptyDataGiven, _bridge.SignUp(null, "miahol susia 12", "123456").Status);
			Assert.AreEqual((int)SignUpStatus.NullEmptyDataGiven, _bridge.SignUp("Pnina", null, "123456").Status);
			Assert.AreEqual((int)SignUpStatus.NullEmptyDataGiven, _bridge.SignUp("Pnina", "mishol susia 12", null).Status);
			Assert.AreEqual((int)SignUpStatus.NullEmptyDataGiven, _bridge.SignUp(null, null, "123456").Status);
			Assert.AreEqual((int)SignUpStatus.NullEmptyDataGiven, _bridge.SignUp(null, "mishol susia 12", null).Status);
			Assert.AreEqual((int)SignUpStatus.NullEmptyDataGiven, _bridge.SignUp("Pnina", null, null).Status);
			Assert.AreEqual((int)SignUpStatus.NullEmptyDataGiven, _bridge.SignUp(null, null, null).Status);

		}


		[TestCleanup]
		public void UserTestCleanUp()
		{
			_bridge2?.CleanSession();
			_bridge.CleanSession();
			MarketLog.RemoveLogs();
			MarketException.RemoveErrors();
			_bridge.ExitMarket();
			_bridge2?.ExitMarket();
		}
	}
}
