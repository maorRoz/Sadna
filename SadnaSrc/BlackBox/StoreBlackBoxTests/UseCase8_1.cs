using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox.StoreBlackBoxTests
{
	[TestClass]
	public class UseCase8_1
	{
		private IUserBridge _storeOwnerUserBridge;
		private IStoreShoppingBridge _storeShoppingBridge;
		private IStoreManagementBridge _storeManagementBridge;
		private IUserBridge _userBuyer;

		[TestInitialize]
		public void MarketBuilder()
		{
			_storeOwnerUserBridge = UserDriver.getBridge();
			_storeOwnerUserBridge.EnterSystem();
			_storeOwnerUserBridge.SignUp("Pnina", "Mishol", "7777", "77777777");
			_storeShoppingBridge = StoreShoppingDriver.getBridge();
			_storeShoppingBridge.GetStoreShoppingService(_storeOwnerUserBridge.GetUserSession());
			_storeShoppingBridge.OpenStore("Toy", "notYour");
			_storeManagementBridge = StoreManagementDriver.getBridge();
			_storeManagementBridge.GetStoreManagementService(_storeOwnerUserBridge.GetUserSession(), "Toy");
			_storeManagementBridge.AddNewProduct("Ouch", 30, "Ouchouch", 6);
		}

		[TestMethod]
		public void AddDiscountAndReceiveItInOrderSuccessfully()
		{
			MarketAnswer res = _storeManagementBridge.AddDiscountToProduct("Ouch", Convert.ToDateTime("15/04/2018"), Convert.ToDateTime("20/04/2018"), 10,"VISIBLE",false);
			Assert.AreEqual((int)DiscountStatus.Success,res.Status);
			//TODO: check the discount was added to the product in the stock


		}

		[TestMethod]
		public void AddDiscountAndDontReceiveItBecauseDatePassed()
		{

		}

		[TestMethod]
		public void AddDiscountFailedNoStore()
		{
			//TODO: check that the product in the stock doesn't have a discout
		}

		[TestMethod]
		public void AddDiscountFailedProductNotFound()
		{
			//TODO: check that the product in the stock doesn't have a discout
		}

		[TestMethod]
		public void AddDiscountFailedDatesAreWrong()
		{
			//TODO: check that the product in the stock doesn't have a discout
		}

		[TestMethod]
		public void AddDiscountFailedPrecentagesTooBig()
		{

		}

		[TestMethod]
		public void AddDiscountFailedAmountIsNegativeOrZero()
		{

		}

		[TestMethod]
		public void AddDiscountFailedDiscountGreaterThanProductPrice()
		{

		}


		[TestMethod]
		public void AddDiscountFailedNotherDiscountAlreadyExists()
		{

		}

		private void SignUp(ref IUserBridge userBridge, string name, string address, string password, string creditCard)
		{
			userBridge = UserDriver.getBridge();
			userBridge.EnterSystem();
			userBridge.SignUp(name, address, password, creditCard);
		}

		/*[TestCleanup]
		public void UserTestCleanUp()
		{
			_storeOwnerUserBridge.CleanSession();
			_storeShoppingBridge.CleanSession();
			_storeManagementBridge.CleanSession();
			_storeOwnerUserBridge.CleanMarket();
		}*/
	}
}
