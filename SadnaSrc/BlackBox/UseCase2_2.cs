using System;
using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBoxStoreTests
{
	[TestClass]
	public class UseCase2_2
	{
		private IUserBridge _bridgeSignUp;
		private IUserBridge _bridgeSignIn;

		[TestInitialize]
		public void MarketBuilder()
		{
			
		}

		[TestMethod]
		public void SuccessInOpeningAStore()
		{
			SignUp("Pnina","mishol","7894","1234567");
			//SignIn("Pnina","7894");
			_bridgeSignUp.GetStoreShoppingService();
			Assert.AreEqual((int)OpenStoreStatus.Success,_bridgeSignUp.OpenStore("PninaStore", "ben-gurion").Status);
		}

		private void SignUp(string name, string address, string password, string creditCard)
		{
			_bridgeSignUp = Driver.getBridge();
			_bridgeSignUp.EnterSystem();
			_bridgeSignUp.SignUp(name, address, password, creditCard);
		}

		private void SignIn(string name, string password)
		{
			_bridgeSignIn = Driver.getBridge();
			_bridgeSignIn.EnterSystem();
			_bridgeSignIn.SignIn(name, password);
		}

		[TestCleanup]
		public void UserTestCleanUp()
		{
			_bridgeSignUp?.CleanSession();
			_bridgeSignIn?.CleanSession();
			_bridgeSignIn?.CleanMarket();
			//_bridgeSignUp?.CleanMarket();
		}


	}
}
