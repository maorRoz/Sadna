using System;
using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBoxStoreTests
{
    [TestClass]
    public class UseCase1_3
    {
        private IUserBridge _bridgeSignUp;
        private IStoreShoppingBridge _storeBridge;
        private IStoreShoppingBridge _storeBridgeGuest;
        private IUserBridge _userWatchStore;

        [TestInitialize]
        public void MarketBuilder()
        {
            _storeBridge = StoreShoppingDriver.getBridge();
            SignUp("Pnina", "mishol", "7894", "12345678");
            _storeBridge.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
            Assert.AreEqual((int)OpenStoreStatus.Success, _storeBridge.OpenStore("OOF", "BASA").Status);
            _userWatchStore = null;
            _storeBridgeGuest = null;
        }

        [TestMethod]
        public void GuestViewStore()
        {
            _userWatchStore = UserDriver.getBridge();
            _userWatchStore.EnterSystem();
            _storeBridgeGuest = StoreShoppingDriver.getBridge();
            _storeBridgeGuest.GetStoreShoppingService(_userWatchStore.GetUserSession());
            MarketAnswer storeDetails = _storeBridgeGuest.ViewStoreInfo("OOF");
            Assert.AreEqual((int)ViewStoreStatus.Success, storeDetails.Status);
            string expectedAnswer = "StoreName: OOF StoreAddress: BASA";
            string receivedAnswer = "StoreName: " + storeDetails.ReportList[0] + " StoreAddress: " + storeDetails.ReportList[1];
            Assert.AreEqual(expectedAnswer, receivedAnswer);

        }

        [TestMethod]
        public void RegisteredUserViewStore()
        {
            MarketAnswer storeDetails = _storeBridge.ViewStoreInfo("OOF");
            Assert.AreEqual((int)ViewStoreStatus.Success, storeDetails.Status);
            string expectedAnswer = "StoreName: OOF StoreAddress: BASA";
            string receivedAnswer = "StoreName: " + storeDetails.ReportList[0] + " StoreAddress: " + storeDetails.ReportList[1];
            Assert.AreEqual(expectedAnswer, receivedAnswer);
        }

        [TestMethod]
        public void NoStoreExistsGuestViewStore()
        {
            _userWatchStore = UserDriver.getBridge();
            _userWatchStore.EnterSystem();
            _storeBridgeGuest = StoreShoppingDriver.getBridge();
            _storeBridgeGuest.GetStoreShoppingService(_userWatchStore.GetUserSession());
            MarketAnswer storeDetails = _storeBridgeGuest.ViewStoreInfo("OOFA");
            Assert.AreEqual((int)ViewStoreStatus.NoStore, storeDetails.Status);
            Assert.AreEqual(null, storeDetails.ReportList);
        }

        [TestMethod]
        public void NoStoreExistsRegisteredUserViewStore()
        {
            MarketAnswer storeDetails = _storeBridge.ViewStoreInfo("OOFA");
            Assert.AreEqual((int)ViewStoreStatus.NoStore, storeDetails.Status);
            Assert.AreEqual(null, storeDetails.ReportList);
        }

        [TestMethod]
        public void InvalidUserDidntEnterSystem()
        {
            _userWatchStore = UserDriver.getBridge();
            _storeBridgeGuest = StoreShoppingDriver.getBridge();
            _storeBridgeGuest.GetStoreShoppingService(_userWatchStore.GetUserSession());
            MarketAnswer storeDetails = _storeBridgeGuest.ViewStoreInfo("OOF");
            Assert.AreEqual((int)ViewStoreStatus.InvalidUser, storeDetails.Status);
            Assert.AreEqual(null, storeDetails.ReportList);
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
            _bridgeSignUp.CleanSession();
            _storeBridge.CleanSession();
            _userWatchStore?.CleanSession();
            _storeBridgeGuest?.CleanSession();
            _bridgeSignUp.CleanMarket();
        }

    }
}