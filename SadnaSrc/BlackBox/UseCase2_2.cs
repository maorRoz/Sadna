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
			//TODO: don't forget to delete the store
			SignUp("Pnina","mishol","7894","12345678");
			_bridgeSignUp.GetStoreShoppingService();
			//Assert.AreEqual((int)OpenStoreStatus.Success,_bridgeSignUp.OpenStore("PninaStore", "ben-gurion").Status);
			_bridgeSignUp.GetStoreShoppingService();
			MarketAnswer storeDetails = _bridgeSignUp.ViewStoreInfo("X");
			//Assert.AreEqual("hello",storeDetails.ReportList[0]);
		}

		[TestMethod]
		public void CheckViewStore()
		{
			
			SignUp("Pnina", "mishol", "7894", "12345678");
			_bridgeSignUp.GetStoreShoppingService();
			MarketAnswer storeDetails = _bridgeSignUp.ViewStoreInfo("X");
			//Assert.AreEqual("hello",storeDetails.ReportList[0]);
		}

		[TestMethod]
		public void StoreAlreadyExists()
		{
			SignUp("Pnina", "mishol", "7894", "12345678");
			_bridgeSignUp.GetStoreShoppingService();
			_bridgeSignUp.OpenStore("PninaStore", "ben-gurion");
			Assert.AreEqual((int)OpenStoreStatus.AlreadyExist, _bridgeSignUp.OpenStore("PninaStore", "ben-gurion").Status);
		}

		[TestMethod]
		public void UserDidntSignUpInvalidCreditCart()
		{
			SignUp("Pnina", "mishol", "7894", "12345");
			_bridgeSignUp.GetStoreShoppingService();
			Assert.AreEqual((int)OpenStoreStatus.InvalidUser,_bridgeSignUp.OpenStore("PninaStore", "ben-gurion").Status);
		}

		[TestMethod]
		public void UserDidntSignUpInvalidUserName()
		{
			SignUp("", "mishol", "7894", "12345678");
			_bridgeSignUp.GetStoreShoppingService();
			Assert.AreEqual((int) OpenStoreStatus.InvalidUser, _bridgeSignUp.OpenStore("PninaStore", "ben-gurion").Status);
		}

		[TestMethod]
		public void UserDidntSignUpInvalidAddress()
		{
			SignUp("Pnina", null, "7894", "12345678");
			_bridgeSignUp.GetStoreShoppingService();
			Assert.AreEqual((int)OpenStoreStatus.InvalidUser, _bridgeSignUp.OpenStore("PninaStore", "ben-gurion").Status);
		}

		[TestMethod]
		public void UserDidntSignUpInvalidPassword()
		{
			SignUp("Pnina", "mishol", "", "12345678");
			_bridgeSignUp.GetStoreShoppingService();
			Assert.AreEqual((int)OpenStoreStatus.InvalidUser, _bridgeSignUp.OpenStore("PninaStore", "ben-gurion").Status);
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
			_bridgeSignUp.CleanSession();
			_bridgeSignIn?.CleanSession();
			_bridgeSignUp.CleanMarket();
		}


	}
}
