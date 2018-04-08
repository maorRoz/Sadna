using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BlackBox
{

	[TestClass]
	public class UseCase1_6
	{
		private IUserBridge _bridge;
		private string userToCheck = "Big Smoke";
		private string userToCheckPassword = "";

		[TestInitialize]
		public void MarketBuilder()
		{
			_bridge = new RealBridge();
			//TODO: open a store
			//TODO: add products to the store
			//TODO: add products to cart
		}

		[TestMethod]
		public void SuccessViewCart()
		{
			_bridge.EnterSystem();
			//_bridge.SignIn(userToCheck, 

		}
	}
}
