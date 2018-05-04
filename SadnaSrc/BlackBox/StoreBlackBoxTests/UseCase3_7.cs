using System;
using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox.StoreBlackBoxTests
{
    [TestClass]
    public class UseCase3_7
    {
        private IUserBridge _userBridge;
        private IUserBridge _userBridge2;
        private IStoreShoppingBridge _storeShopping;
        private IStoreShoppingBridge _storeShopping2;
        private IStoreManagementBridge _managerBridge;
        private IStoreManagementBridge _ownerStoreBridge;
        private IOrderBridge _orderBridge;
        private IOrderBridge _orderBridge2;
        private string storeToCheck1 = "blahblah";
        private string storeToCheck2 = "blahblah2";

        [TestInitialize]

        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            _ownerStoreBridge = StoreManagementDriver.getBridge();

            CreateUser1();
            CreateUser2();
            CreateStoreBlahblah();
            CreateStoreBlahblah2();
            AddProductsToBlahblah();
            User1AddToCart();
            User2AddToCart();
            User1MakeOrder();
            User2MakeOrder();
        }

        private void User2MakeOrder()
        {
            _orderBridge2 = OrderDriver.getBridge();
            _orderBridge2.GetOrderService(_userBridge2.GetUserSession());
            _orderBridge2.BuyItemFromImmediate("hello2", "blahblah", 2, 20, null);
            _orderBridge2.BuyItemFromImmediate("Goodbye2", "blahblah2", 2, 20, null);
        }

        private void User1MakeOrder()
        {
            _orderBridge = OrderDriver.getBridge();
            _orderBridge.GetOrderService(_userBridge.GetUserSession());
            _orderBridge.BuyItemFromImmediate("hello", "blahblah", 2, 10, null);
            _orderBridge.BuyItemFromImmediate("Goodbye", "blahblah2", 2, 10, null);
        }

        private void User2AddToCart()
        {
            _storeShopping2.AddProductToCart("blahblah", "hello2", 5);
        }

        private void User1AddToCart()
        {
            _storeShopping.AddProductToCart("blahblah", "hello", 5);
        }

        private void AddProductsToBlahblah()
        {
            _managerBridge = StoreManagementDriver.getBridge();
            _managerBridge.GetStoreManagementService(_userBridge.GetUserSession(), "blahblah");
            _managerBridge.AddNewProduct("hello", 10, "nice product", 8);
            _managerBridge.AddNewProduct("hello2", 20, "nice product2", 20);
        }

        private void CreateStoreBlahblah2()
        {
            _storeShopping2 = StoreShoppingDriver.getBridge();
            _storeShopping2.GetStoreShoppingService(_userBridge2.GetUserSession());
            MarketAnswer res20 = _storeShopping2.OpenStore("blahblah2", "blah");
            Assert.AreEqual((int)OpenStoreStatus.Success, res20.Status);
        }

        private void CreateStoreBlahblah()
        {
            _storeShopping = StoreShoppingDriver.getBridge();
            _storeShopping.GetStoreShoppingService(_userBridge.GetUserSession());
            _storeShopping.OpenStore("blahblah", "blah");
        }

        private void CreateUser2()
        {
            _userBridge2 = UserDriver.getBridge();
            _userBridge2.EnterSystem();
            _userBridge2.SignUp("Maor", "vinget", "9999", "99999999");
        }

        private void CreateUser1()
        {
            _userBridge = UserDriver.getBridge();
            _userBridge.EnterSystem();
            _userBridge.SignUp("Pnina", "misholSusia", "852852", "77777777");
        }

        [TestMethod]
        public void SuccessHistoryPurchase()
        {
            _ownerStoreBridge.GetStoreManagementService(_userBridge.GetUserSession(), storeToCheck1);
            MarketAnswer res = _ownerStoreBridge.ViewStoreHistory();
            string[] purchaseUserHistory = res.ReportList;
            string[] expectedHistory =
            {
                "User: Pnina Product: hello Store: blahblah Sale: Immediate Quantity: 2 Price: 50 Date: "+DateTime.Now.Date.ToString("yyyy-MM-dd"),
                "User: Maor Product: hello2 Store: blahblah Sale: Immediate Quantity: 2 Price: 100 Date: "+DateTime.Now.Date.ToString("yyyy-MM-dd")
            };

            Assert.AreEqual((int)ViewPurchaseHistoryStatus.Success, res.Status);
            for (int i = 0; i < purchaseUserHistory.Length; i++)
            {
                Assert.AreEqual(expectedHistory[i], purchaseUserHistory[i]);
            }

        }

        [TestMethod]
        public void FailHistoryPurchaseNotOwner()
        {
            _ownerStoreBridge.GetStoreManagementService(_userBridge2.GetUserSession(), storeToCheck1);
            MarketAnswer res = _ownerStoreBridge.ViewStoreHistory();
            Assert.AreEqual((int)ManageStoreStatus.InvalidManager, res.Status);
            Assert.IsNull(res.ReportList);

        }

        [TestMethod]
        public void SuccessStoreWithNoHistory()
        {
            _ownerStoreBridge.GetStoreManagementService(_userBridge2.GetUserSession(), storeToCheck2);
            MarketAnswer res = _ownerStoreBridge.ViewStoreHistory();
            Assert.AreEqual((int)ViewPurchaseHistoryStatus.Success, res.Status);
            Assert.AreEqual(0, res.ReportList.Length);

        }


        [TestCleanup]

        public void UserTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();

        }


    }
}