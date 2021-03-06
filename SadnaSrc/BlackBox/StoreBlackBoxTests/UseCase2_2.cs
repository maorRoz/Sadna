﻿using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace BlackBox.StoreBlackBoxTests
{
    [TestClass]
    public class UseCase2_2
    {
        private IUserBridge _bridgeSignUp;
        private IStoreShoppingBridge _storeBridge;

        [TestInitialize]
        public void MarketBuilder()
        {
            _storeBridge = StoreShoppingDriver.getBridge();
        }

		[TestMethod]
		public void SuccessInOpeningAStore()
		{
		    MarketDB.Instance.InsertByForce();
            SignUp("Pnina","mishol","7894","12345678");
			_storeBridge.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			Assert.AreEqual((int)OpenStoreStatus.Success, _storeBridge.OpenStore("PninaStore", "Ben-Gurion").Status);
			MarketAnswer storeDetails = _storeBridge.ViewStoreInfo("PninaStore");
			string expectedAnswer = "_storeName: PninaStore StoreAddress: Ben-Gurion";
			string receivedAnswer = "_storeName: " + storeDetails.ReportList[0] + " StoreAddress: " + storeDetails.ReportList[1];
			Assert.AreEqual(expectedAnswer, receivedAnswer);
		}

		[TestMethod]
		public void StoreAlreadyExists()
		{
			SignUp("Pnina", "mishol", "7894", "12345678");
			_storeBridge.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			_storeBridge.OpenStore("PninaStore", "ben-gurion");
			Assert.AreEqual((int)OpenStoreStatus.AlreadyExist, _storeBridge.OpenStore("PninaStore", "ben-gurion").Status);
		}

		[TestMethod]
		public void UserDidntSignUpInvalidCreditCart()
		{
			SignUp("Pnina", "mishol", "7894", "12345");
			_storeBridge.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			Assert.AreEqual((int)OpenStoreStatus.InvalidUser, _storeBridge.OpenStore("PninaStore", "ben-gurion").Status);
		}

		[TestMethod]
		public void UserDidntSignUpInvalidUserName()
		{
			SignUp("", "mishol", "7894", "12345678");
			_storeBridge.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			Assert.AreEqual((int) OpenStoreStatus.InvalidUser, _storeBridge.OpenStore("PninaStore", "ben-gurion").Status);
		}

		[TestMethod]
		public void UserDidntSignUpInvalidAddress()
		{
			SignUp("Pnina", null, "7894", "12345678");
			_storeBridge.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			Assert.AreEqual((int)OpenStoreStatus.InvalidUser, _storeBridge.OpenStore("PninaStore", "ben-gurion").Status);
		}

		[TestMethod]
		public void UserDidntSignUpInvalidPassword()
		{
			SignUp("Pnina", "mishol", "", "12345678");
			_storeBridge.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			Assert.AreEqual((int)OpenStoreStatus.InvalidUser, _storeBridge.OpenStore("PninaStore", "ben-gurion").Status);
		}

        private void SignUp(string name, string address, string password, string creditCard)
        {
            _bridgeSignUp = UserDriver.getBridge();
            _bridgeSignUp.EnterSystem();
            _bridgeSignUp.SignUp(name, address, password, creditCard);
        }
        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }


    }
}