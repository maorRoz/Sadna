using System;
using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox.StoreBlackBoxTests
{
    [TestClass]
    public class UseCase1_5
    {
        private IUserBridge _signInBridge;
        private IUserBridge _bridgeSignUp;
        private IStoreShoppingBridge _storeBridge;
        private IStoreShoppingBridge _storeBridge2;
        private IStoreShoppingBridge _storeBridge3;
        private IStoreManagementBridge _storeManage1;
        private IStoreManagementBridge _storeManage2;


        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            SignIn();
            CreateStore1AndProducts();
            CreateStore2AndProducts();
            _bridgeSignUp = UserDriver.getBridge();
            _storeBridge3 = null;
        }


        private void SignIn()
        {
            _signInBridge = UserDriver.getBridge();
            _signInBridge.EnterSystem();
            _signInBridge.SignUp("Pninaaa", "aaaaa", "777777", "44444444");
        }
		private void CreateStore1AndProducts()
		{
			_storeBridge = StoreShoppingDriver.getBridge();
			_storeBridge.GetStoreShoppingService(_signInBridge.GetUserSession());
			_storeBridge.OpenStore("BlahStore", "BlahStreet");
			_storeManage1 = StoreManagementDriver.getBridge();
			_storeManage1.GetStoreManagementService(_signInBridge.GetUserSession(), "BlahStore");
			_storeManage1.AddNewProduct("bisli", 200, "yammy!", 5);
			_storeManage1.AddNewProduct("cheaps", 20, "yammy2!", 80);
		}

		private void CreateStore2AndProducts()
		{
			_storeBridge2 = StoreShoppingDriver.getBridge();
			_storeBridge2.GetStoreShoppingService(_signInBridge.GetUserSession());
			_storeBridge2.OpenStore("BlahStore2", "BlahStreet2");
			_storeManage2 = StoreManagementDriver.getBridge();
			_storeManage2.GetStoreManagementService(_signInBridge.GetUserSession(), "BlahStore2");
			_storeManage2.AddNewProduct("doritos", 30, "yammy3!", 30);
		}

		[TestMethod]
		public void SuccessAddingProductToCartGuest()
		{
			_bridgeSignUp.EnterSystem();
			_storeBridge3 = StoreShoppingDriver.getBridge();
			_storeBridge3.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			MarketAnswer res1 = _storeBridge3.AddProductToCart("BlahStore", "bisli", 1);
			Assert.AreEqual((int)StoreEnum.Success,res1.Status);
			MarketAnswer res2 = _storeBridge3.AddProductToCart("BlahStore", "cheaps", 2);
			Assert.AreEqual((int)StoreEnum.Success, res2.Status);
			MarketAnswer res3 = _storeBridge3.AddProductToCart("BlahStore2", "doritos", 3);
			Assert.AreEqual((int)StoreEnum.Success, res3.Status);
			//Lets view to cart to see the products were indeed added.
			MarketAnswer cartDetails = _bridgeSignUp.ViewCart();
			string[] cartItemsExpected =
			{
				"Name : bisli Store : BlahStore Quantity : 1 Unit Price : 200 Final Price : 200",
				"Name : cheaps Store : BlahStore Quantity : 2 Unit Price : 20 Final Price : 40",
				"Name : doritos Store : BlahStore2 Quantity : 3 Unit Price : 30 Final Price : 90"
			};

            string[] cartItemsReceived = cartDetails.ReportList;
            Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
            for (int i = 0; i < cartItemsReceived.Length; i++)
            {
                Assert.AreEqual(cartItemsExpected[i], cartItemsReceived[i]);
            }

        }

        [TestMethod]
        public void SuccessAddingProductToCartGRegisteredUser()
        {
            MarketAnswer res1 = _storeBridge.AddProductToCart("BlahStore", "bisli", 1);
            Assert.AreEqual((int)StoreEnum.Success, res1.Status);
            MarketAnswer res2 = _storeBridge.AddProductToCart("BlahStore", "cheaps", 2);
            Assert.AreEqual((int)StoreEnum.Success, res2.Status);
            MarketAnswer res3 = _storeBridge.AddProductToCart("BlahStore2", "doritos", 3);
            Assert.AreEqual((int)StoreEnum.Success, res3.Status);
            //Lets view to cart to see the products were indeed added.
            MarketAnswer cartDetails = _signInBridge.ViewCart();
            string[] cartItemsExpected =
            {
                "Name : bisli Store : BlahStore Quantity : 1 Unit Price : 200 Final Price : 200",
                "Name : cheaps Store : BlahStore Quantity : 2 Unit Price : 20 Final Price : 40",
                "Name : doritos Store : BlahStore2 Quantity : 3 Unit Price : 30 Final Price : 90"

            };

            string[] cartItemsReceived = cartDetails.ReportList;
            Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
            for (int i = 0; i < cartItemsReceived.Length; i++)
            {
                Assert.AreEqual(cartItemsExpected[i], cartItemsReceived[i]);
            }

        }

		[TestMethod]
		public void StoreDoesntExistGuest()
		{
			_bridgeSignUp.EnterSystem();
			_storeBridge3 = StoreShoppingDriver.getBridge();
			_storeBridge3.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			MarketAnswer res1 = _storeBridge3.AddProductToCart("BlahStoreIhsa", "bisli", 1);
			Assert.AreEqual((int)StoreEnum.StoreNotExists, res1.Status);
			//the cart should remain empty
			MarketAnswer cartDetails = _bridgeSignUp.ViewCart();
			string[] cartItemsReceived = cartDetails.ReportList;
			string[] cartItemsExpected = { };
			Assert.AreEqual(cartItemsExpected.Length,cartItemsReceived.Length);
		}

        [TestMethod]
        public void StoreDoesntExistRegisteredUser()
        {
            MarketAnswer res1 = _storeBridge.AddProductToCart("BlahStoreIhsa", "bisli", 1);
            Assert.AreEqual((int)StoreEnum.StoreNotExists, res1.Status);
            //the cart should remain empty
            MarketAnswer cartDetails = _signInBridge.ViewCart();
            string[] cartItemsReceived = cartDetails.ReportList;
            string[] cartItemsExpected = { };
            Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
        }

		[TestMethod]
		public void ProductWantedWasntFoundGuest()
		{
			_bridgeSignUp.EnterSystem();
			_storeBridge3 = StoreShoppingDriver.getBridge();
			_storeBridge3.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			MarketAnswer res1 = _storeBridge3.AddProductToCart("BlahStore", "bisli852", 1);
			Assert.AreEqual((int)StoreEnum.ProductNotFound, res1.Status);
			//the cart should remain empty
			MarketAnswer cartDetails = _bridgeSignUp.ViewCart();
			string[] cartItemsReceived = cartDetails.ReportList;
			string[] cartItemsExpected = { };
			Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
		}

        [TestMethod]
        public void ProductWantedWasntFoundRegisteredUser()
        {
            MarketAnswer res1 = _storeBridge.AddProductToCart("BlahStore", "bisli852", 1);
            Assert.AreEqual((int)StoreEnum.ProductNotFound, res1.Status);
            //the cart should remain empty
            MarketAnswer cartDetails = _signInBridge.ViewCart();
            string[] cartItemsReceived = cartDetails.ReportList;
            string[] cartItemsExpected = { };
            Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
        }

		[TestMethod]
		public void ProductQuantityTooBigGuest()
		{
			_bridgeSignUp.EnterSystem();
			_storeBridge3 = StoreShoppingDriver.getBridge();
			_storeBridge3.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			MarketAnswer res1 = _storeBridge3.AddProductToCart("BlahStore", "bisli", 10);
			Assert.AreEqual((int)StoreEnum.QuantityIsTooBig, res1.Status);
			//the cart should remain empty
			MarketAnswer cartDetails = _bridgeSignUp.ViewCart();
			string[] cartItemsReceived = cartDetails.ReportList;
			string[] cartItemsExpected = { };
			Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
		}

        [TestMethod]
        public void ProductQuantityTooBigRegisteredUser()
        {
            MarketAnswer res1 = _storeBridge.AddProductToCart("BlahStore", "bisli", 10);
            Assert.AreEqual((int)StoreEnum.QuantityIsTooBig, res1.Status);
            //the cart should remain empty
            MarketAnswer cartDetails = _signInBridge.ViewCart();
            string[] cartItemsReceived = cartDetails.ReportList;
            string[] cartItemsExpected = { };
            Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
        }

		[TestMethod]
		public void ProductQuantityNegativeGuest()
		{
			_bridgeSignUp.EnterSystem();
			_storeBridge3 = StoreShoppingDriver.getBridge();
			_storeBridge3.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			MarketAnswer res1 = _storeBridge3.AddProductToCart("BlahStore", "bisli", -5);
			Assert.AreEqual((int)StoreEnum.QuantityIsNegative, res1.Status);
			//the cart should remain empty
			MarketAnswer cartDetails = _bridgeSignUp.ViewCart();
			string[] cartItemsReceived = cartDetails.ReportList;
			string[] cartItemsExpected = { };
			Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
		}

        [TestMethod]
        public void ProductQuantityNegativeRegisteredUser()
        {
            MarketAnswer res1 = _storeBridge.AddProductToCart("BlahStore", "bisli", 0);
            Assert.AreEqual((int)StoreEnum.QuantityIsNegative, res1.Status);
            //the cart should remain empty
            MarketAnswer cartDetails = _signInBridge.ViewCart();
            string[] cartItemsReceived = cartDetails.ReportList;
            string[] cartItemsExpected = { };
            Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
        }

		[TestMethod]
		public void AddToCartInvalidUser()
		{
			//User didn't enter the system
			_storeBridge3 = StoreShoppingDriver.getBridge();
			_storeBridge3.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			MarketAnswer res1 = _storeBridge3.AddProductToCart("BlahStore", "bisli", 1);
			//TODO: when lior fixes the states - this should be invalid user
			Assert.AreEqual((int)StoreEnum.NoPremmision, res1.Status);
			//the user can't even watch his cart until entering the system
			MarketAnswer cartDetails = _bridgeSignUp.ViewCart();
			Assert.AreEqual((int)ViewCartStatus.DidntEnterSystem ,cartDetails.Status);
		}

		[TestMethod]
		public void AddToCartGuestTurnsIntoRegisteredUser()
		{
			MarketAnswer res1 = _storeBridge.AddProductToCart("BlahStore", "bisli", 1);
			Assert.AreEqual((int)StoreEnum.Success, res1.Status);
			MarketAnswer res2 = _storeBridge.AddProductToCart("BlahStore", "cheaps", 2);
			Assert.AreEqual((int)StoreEnum.Success, res2.Status);
			_bridgeSignUp.EnterSystem();
			_storeBridge3 = StoreShoppingDriver.getBridge();
			_storeBridge3.GetStoreShoppingService(_bridgeSignUp.GetUserSession());
			MarketAnswer res3 = _storeBridge3.AddProductToCart("BlahStore2", "doritos", 3);
			Assert.AreEqual((int)StoreEnum.Success, res3.Status);

            MarketAnswer res4 = _bridgeSignUp.SignIn("Pninaaa", "777777");
            Assert.AreEqual((int)SignInStatus.Success, res4.Status);
            //Lets view to cart to see the products were indeed added.
            MarketAnswer cartDetails = _bridgeSignUp.ViewCart();
            string[] cartItemsExpected =
            {
                "Name : bisli Store : BlahStore Quantity : 1 Unit Price : 200 Final Price : 200",
                "Name : cheaps Store : BlahStore Quantity : 2 Unit Price : 20 Final Price : 40",
                "Name : doritos Store : BlahStore2 Quantity : 3 Unit Price : 30 Final Price : 90"

            };

            string[] cartItemsReceived = cartDetails.ReportList;
            //Assert.AreEqual(cartItemsExpected.Length, cartItemsReceived.Length);
            for (int i = 0; i < cartItemsReceived.Length; i++)
            {
                Assert.AreEqual(cartItemsExpected[i], cartItemsReceived[i]);
            }
        }


        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }

    }
}