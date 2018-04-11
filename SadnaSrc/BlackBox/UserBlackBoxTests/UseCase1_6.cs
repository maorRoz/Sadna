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
			_bridge = UserDriver.getBridge();
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
			    "Name : #45 Store Cluckin Bell Quantity: 1 Unit Price : 16 Final Price: 16",
                "Name : #45 With Cheese Store Cluckin Bell Quantity: 1 Unit Price : 18 Final Price: 18",
				"Name : #6 Extra Dip Store Cluckin Bell Quantity: 1 Unit Price : 8.5 Final Price: 8.5",
				"Name : #7 Store Cluckin Bell Quantity: 1 Unit Price : 8 Final Price: 8",
				"Name : #9 Large Store Cluckin Bell Quantity: 1 Unit Price : 7 Final Price: 7",
				"Name : Large Soda Store Cluckin Bell Quantity: 1 Unit Price : 5 Final Price: 5",
				"Name : #9 Store Cluckin Bell Quantity: 2 Unit Price : 5 Final Price: 10",
				"Name : Gun Store M Quantity: 3 Unit Price : 25 Final Price: 75"
				
			};
            Assert.AreEqual(cartItemsExpected.Length,cartItemsReceived.Length);
			for (int i = 0; i < cartItemsReceived.Length; i++)
			{
				Assert.AreEqual(cartItemsExpected[i],cartItemsReceived[i]);
			}

		}

		[TestMethod]
		public void DidntEnterSystemViewCart()
		{
			Assert.AreEqual((int) ViewCartStatus.DidntEnterSystem, _bridge.ViewCart().Status);
		}

		//TODO: add another test for a guest
		[TestMethod]
		public void SuccessChangeQuantityOfProduct()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(userToCheck, userToCheckPassword);
			//TODO: change a quantity of an item
			//TODO: call viewCart and compare between expected and received products
			//TODO: check that the status is success
			//TODO: change quantity back to what it was before
			//TODO: print to see it worked
			//TODO: the last two should probably be a function

		}

		//TODO: add another test for a guest
		[TestMethod]
		public void DidntEnterSystemChangeQuantity()
		{
			Assert.AreEqual((int)EditCartItemStatus.DidntEnterSystem, _bridge.EditCartItem("M", "Gun", 25.0, 5).Status);
		}

		//TODO: add another test for a guest
		[TestMethod]
		public void NoItemFoundChangeQuantityStoreName()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(userToCheck, userToCheckPassword);
			Assert.AreEqual((int)EditCartItemStatus.NoItemFound, _bridge.EditCartItem("Moo", "Gun", 25.0, 5).Status);
		}

		//TODO: add another test for a guest
		[TestMethod]
		public void NoItemFoundChangeQuantityProductName()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(userToCheck, userToCheckPassword);
			Assert.AreEqual((int)EditCartItemStatus.NoItemFound, _bridge.EditCartItem("M", "Gunoooo", 25.0, 5).Status);
		}

		//TODO: add another test for a guest
		[TestMethod]
		public void NoItemFoundChangeQuantityUnitPrice()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(userToCheck, userToCheckPassword);
			Assert.AreEqual((int)EditCartItemStatus.NoItemFound, _bridge.EditCartItem("M", "Gun", 50, 5).Status);
		}



		
		[TestMethod]
		public void NegativeQuantity()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(userToCheck, userToCheckPassword);
			Assert.AreEqual((int)EditCartItemStatus.ZeroNegativeQuantity, _bridge.EditCartItem("M", "Gun", 25.0, -200).Status);
		}
		

		[TestMethod]
		public void ZeroQuantity()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(userToCheck, userToCheckPassword);
			Assert.AreEqual((int)EditCartItemStatus.ZeroNegativeQuantity, _bridge.EditCartItem("M", "Gun", 25.0, -3).Status);
		}

		//TODO: add another test for a guest
		[TestMethod]
		public void RemoveProductFromCartSuccess()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(userToCheck, userToCheckPassword);
			//TODO: remove an item
			//TODO: check the removal was successful
			//TODO: compare between the expected items and the received items
			//TODO: add the item back to the cart

		}

		//TODO: add another test for a guest
		[TestMethod]
		public void DidntEnterSystemRemoveItem()
		{
			_bridge.RemoveFromCart("M", "Gun", 25.0);
		}

		//TODO: add another test for a guest
		[TestMethod]
		public void NoItemFoundRemoveItemStore()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(userToCheck, userToCheckPassword);
			Assert.AreEqual((int)RemoveFromCartStatus.NoItemFound,_bridge.RemoveFromCart("Moo","Gun",25.0).Status);
		}

		//TODO: add another test for a guest
		[TestMethod]
		public void NoItemFoundRemoveItemProduct()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(userToCheck, userToCheckPassword);
			Assert.AreEqual((int)RemoveFromCartStatus.NoItemFound, _bridge.RemoveFromCart("M", "Gunoo", 25.0).Status);
		}

		//TODO: add another test for a guest
		[TestMethod]
		public void NoItemFoundRemoveItemPrice()
		{
			_bridge.EnterSystem();
			_bridge.SignIn(userToCheck, userToCheckPassword);
			Assert.AreEqual((int)RemoveFromCartStatus.NoItemFound, _bridge.RemoveFromCart("M", "Gun", 20).Status);
		}

		[TestCleanup]
		public void UserTestCleanUp()
		{
			_bridge.CleanSession();
			_bridge.CleanMarket();
		}

	}
}
