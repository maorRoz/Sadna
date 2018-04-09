using System;
using System.Text;
using System.Collections.Generic;
using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBoxUserTests
{

	[TestClass]
	public class UseCase1_6
	{
		private IUserBridge _bridge;
		private string userToCheck = "Big Smoke";
		private string userToCheckPassword = "123";

		[TestInitialize]
		public void MarketBuilder()
		{
			_bridge = Driver.getBridge();
			//TODO: open a store
			//TODO: add products to the store
			//TODO: add products to cart
		}

		[TestMethod]
		public void SuccessViewCartOfRegisteredUser()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(userToCheck, userToCheckPassword);
			MarketAnswer res = _bridge.ViewCart();
			Assert.AreEqual((int)ViewCartStatus.Success,res.Status);
			string[] cartItemsReceived = res.ReportList;
			string[] cartItemsExpected =
			{
				"Name : #45 With Cheese Store Cluckin Bell Quantity: 1 Unit Price : 18 Final Price: 18",
				"Name : #6 Extra Dip Store Cluckin Bell Quantity: 1 Unit Price : 8.5 Final Price: 8.5",
				"Name : #7 Store Cluckin Bell Quantity: 1 Unit Price : 8 Final Price: 8",
				"Name : #9 Large Store Cluckin Bell Quantity: 1 Unit Price : 7 Final Price: 7",
				"Name : Large Soda Store Cluckin Bell Quantity: 1 Unit Price : 5 Final Price: 5",
				"Name : #9 Store Cluckin Bell Quantity: 2 Unit Price : 5 Final Price: 10",
				"Name : Gun Store M Quantity: 3 Unit Price : 25 Final Price: 75"
				
			};

			for (int i = 0; i < cartItemsReceived.Length; i++)
			{
				Assert.AreEqual(cartItemsExpected[i],cartItemsReceived[i]);
			}

		}

		[TestCleanup]
		public void UserTestCleanUp()
		{
			_bridge.CleanSession();
			_bridge.CleanMarket();

		}

	}
}
