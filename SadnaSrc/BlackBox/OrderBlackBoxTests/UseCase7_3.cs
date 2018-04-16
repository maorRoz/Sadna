using System;
using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox.OrderBlackBoxTests
{
    [TestClass]
    public class UseCase7_3
    {
        private IUserBridge _userAdminBridge;
        private IAdminBridge _adminBridge;
        private IUserBridge _buyerRegisteredUserBridge1;
        private IUserBridge _buyerRegisteredUserBridge2;
        private IUserBridge _buyerRegisteredUserBridge3;
        private IOrderBridge _orderBridge1;
        private IOrderBridge _orderBridge2;
        private IOrderBridge _orderBridge3;
        private IUserBridge _buyerGuestBridge;
        private IUserBridge _storeOwnerBridge;
        private IStoreShoppingBridge _shoppingBridge;
        private IStoreShoppingBridge _shoppingBridge2;
        private IStoreManagementBridge _storeManagementBridge;
        private string storeName = "LotteryStore";

        [TestInitialize]
        public void MarketBuilder()
        {
            SignUpStoreOwner("Pnina", "mishol", "666", "66666666");
            OpenStoreAndAddProducts();
            _userAdminBridge = UserDriver.getBridge();
            _userAdminBridge.EnterSystem();
            _userAdminBridge.SignIn("Arik1", "123");
            _adminBridge = AdminDriver.getBridge();
            _adminBridge.GetAdminService(_userAdminBridge.GetUserSession());
            _buyerRegisteredUserBridge1 = null;
            _buyerRegisteredUserBridge2 = null;
            _buyerRegisteredUserBridge3 = null;
            _orderBridge1 = null;
            _orderBridge2 = null;
            _orderBridge3 = null;
            _buyerGuestBridge = null;
        }


        [TestMethod]
        public void LotteryStillGoing()
        {
            MakeRegisteredShoppers();
            Assert.AreEqual((int)OrderStatus.Success, _orderBridge1.BuyLotteryTicket("Fanta", storeName, 1, 4).Status);
            string[] expectedHistoryFirstBuyer =
            {
                "User: Shalom1 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("d"),
            };
            string[] actualHistoryFirstBuyer = _adminBridge.ViewPurchaseHistoryByUser("Shalom1").ReportList;
            Assert.AreEqual(expectedHistoryFirstBuyer.Length, actualHistoryFirstBuyer.Length);
            for (int i = 0; i < expectedHistoryFirstBuyer.Length; i++)
            {
                Assert.AreEqual(expectedHistoryFirstBuyer[i], actualHistoryFirstBuyer[i]);
            }

        }

        [TestMethod]
        public void FirstBuyerWin()
        {
            MakeRegisteredShoppers();
            Assert.AreEqual((int)OrderStatus.Success, _orderBridge1.BuyLotteryTicket("Fanta", storeName, 1, 4).Status);
            Assert.AreEqual((int)OrderStatus.Success, _orderBridge2.BuyLotteryTicket("Fanta", storeName, 1, 4).Status);
            _orderBridge3.Cheat(3);
            Assert.AreEqual((int)OrderStatus.Success, _orderBridge3.BuyLotteryTicket("Fanta", storeName, 1, 4).Status);
            string[] expectedHistoryFirstBuyer =
            {
                "User: Shalom1 Product: DELIVERY : Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 1 Date: "+DateTime.Now.Date.ToString("d"),
                "User: Shalom1 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("d"),
            };
            string[] expectedHistorySecondBuyer =
            {
                "User: Shalom2 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("d"),
            };
            string[] expectedHistoryThirdBuyer =
            {
                "User: Shalom3 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("d"),
            };
            string[] actualHistoryFirstBuyer = _adminBridge.ViewPurchaseHistoryByUser("Shalom1").ReportList;
            string[] actualHistorySecondBuyer = _adminBridge.ViewPurchaseHistoryByUser("Shalom2").ReportList;
            string[] actualHistoryThirdBuyer = _adminBridge.ViewPurchaseHistoryByUser("Shalom3").ReportList;
            lotteryEventReport(expectedHistoryFirstBuyer, expectedHistorySecondBuyer, expectedHistoryThirdBuyer,
                actualHistoryFirstBuyer, actualHistorySecondBuyer, actualHistoryThirdBuyer);
        }

        [TestMethod]
        public void SecondBuyerWin()
        {
            MakeRegisteredShoppers();
            Assert.AreEqual((int)OrderStatus.Success, _orderBridge1.BuyLotteryTicket("Fanta", storeName, 1, 4).Status);
            Assert.AreEqual((int)OrderStatus.Success, _orderBridge2.BuyLotteryTicket("Fanta", storeName, 1, 4).Status);
            _orderBridge3.Cheat(6);
            Assert.AreEqual((int)OrderStatus.Success, _orderBridge3.BuyLotteryTicket("Fanta", storeName, 1, 4).Status);
            string[] expectedHistoryFirstBuyer =
            {
                "User: Shalom1 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("d"),
            };
            string[] expectedHistorySecondBuyer =
            {
                "User: Shalom2 Product: DELIVERY : Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 1 Date: "+DateTime.Now.Date.ToString("d"),
                "User: Shalom2 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("d"),
            };
            string[] expectedHistoryThirdBuyer =
            {
                "User: Shalom3 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("d"),
            };
            string[] actualHistoryFirstBuyer = _adminBridge.ViewPurchaseHistoryByUser("Shalom1").ReportList;
            string[] actualHistorySecondBuyer = _adminBridge.ViewPurchaseHistoryByUser("Shalom2").ReportList;
            string[] actualHistoryThirdBuyer = _adminBridge.ViewPurchaseHistoryByUser("Shalom3").ReportList;
            lotteryEventReport(expectedHistoryFirstBuyer, expectedHistorySecondBuyer, expectedHistoryThirdBuyer,
                actualHistoryFirstBuyer, actualHistorySecondBuyer, actualHistoryThirdBuyer);
        }

        [TestMethod]
        public void ThirdBuyerWin()
        {
            MakeRegisteredShoppers();
            Assert.AreEqual((int)OrderStatus.Success, _orderBridge1.BuyLotteryTicket("Fanta", storeName, 1, 4).Status);
            Assert.AreEqual((int)OrderStatus.Success, _orderBridge2.BuyLotteryTicket("Fanta", storeName, 1, 4).Status);
            _orderBridge3.Cheat(11);
            Assert.AreEqual((int)OrderStatus.Success, _orderBridge3.BuyLotteryTicket("Fanta", storeName, 1, 4).Status);
            string[] expectedHistoryFirstBuyer =
            {
                "User: Shalom1 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("d"),
            };
            string[] expectedHistorySecondBuyer =
            {
                "User: Shalom2 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("d"),
            };
            string[] expectedHistoryThirdBuyer =
            {
                "User: Shalom3 Product: DELIVERY : Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 1 Date: "+DateTime.Now.Date.ToString("d"),
                "User: Shalom3 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("d"),
            };
            string[] actualHistoryFirstBuyer = _adminBridge.ViewPurchaseHistoryByUser("Shalom1").ReportList;
            string[] actualHistorySecondBuyer = _adminBridge.ViewPurchaseHistoryByUser("Shalom2").ReportList;
            string[] actualHistoryThirdBuyer = _adminBridge.ViewPurchaseHistoryByUser("Shalom3").ReportList;
            lotteryEventReport(expectedHistoryFirstBuyer, expectedHistorySecondBuyer, expectedHistoryThirdBuyer,
                actualHistoryFirstBuyer, actualHistorySecondBuyer, actualHistoryThirdBuyer);
        }

        [TestMethod]
        public void LotteryCancelationRefund()
        {
            MakeRegisteredShoppers();
            Assert.AreEqual((int)OrderStatus.Success, _orderBridge1.BuyLotteryTicket("Fanta", storeName, 1, 4).Status);
            Assert.AreEqual((int)OrderStatus.Success, _orderBridge2.BuyLotteryTicket("Fanta", storeName, 1, 4).Status);
            MarketYard.SetDateTime(Convert.ToDateTime("01/01/2019"));
            string[] expectedHistoryFirstBuyer =
            {
                "User: Shalom1 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("d"),
                "User: Shalom1 Product: REFUND: T3 Store: --- Sale: Lottery Quantity: 1 Price: -4 Date: "+DateTime.Now.Date.ToString("d")
            };
            string[] expectedHistorySecondBuyer =
            {
                "User: Shalom2 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("d"),
                "User: Shalom2 Product: REFUND: T4 Store: --- Sale: Lottery Quantity: 1 Price: -4 Date: "+DateTime.Now.Date.ToString("d")
            };
            string[] actualHistoryFirstBuyer = _adminBridge.ViewPurchaseHistoryByUser("Shalom1").ReportList;
            string[] actualHistorySecondBuyer = _adminBridge.ViewPurchaseHistoryByUser("Shalom2").ReportList;
            string[] actualHistoryThirdBuyer = _adminBridge.ViewPurchaseHistoryByUser("Shalom3").ReportList;
            Assert.AreEqual(actualHistoryFirstBuyer.Length, actualHistoryFirstBuyer.Length);
            Assert.AreEqual(expectedHistorySecondBuyer.Length, actualHistorySecondBuyer.Length);
            Assert.IsNull(actualHistoryThirdBuyer);
            for (int i = 0; i < expectedHistoryFirstBuyer.Length; i++)
            {
                Assert.AreEqual(expectedHistoryFirstBuyer[i],actualHistoryFirstBuyer[i]);
                Assert.AreEqual(expectedHistorySecondBuyer[i], actualHistorySecondBuyer[i]);
            }
        }

        [TestMethod]
        public void LotteryFailGuest()
        {

        }

        [TestMethod]
        public void FailPurchaseLotterySupplySystemCollapsed()
        {

        }

        [TestMethod]
        public void FailPurchaseLotteryPaymentSystemCollapsed()
        {

        }

        [TestCleanup]
        public void LotteryTestCleanUp()
        {
            _userAdminBridge.CleanSession();
            _buyerRegisteredUserBridge1?.CleanSession();
            _buyerRegisteredUserBridge2?.CleanSession();
            _buyerRegisteredUserBridge3?.CleanSession();
            _buyerGuestBridge?.CleanSession();
            _storeOwnerBridge.CleanSession();
            _shoppingBridge.CleanSession();
            _shoppingBridge2?.CleanSession();
            _storeManagementBridge.CleanSession();
            _orderBridge1?.CleanSession();
            _orderBridge1?.EnableSupplySystem();
            _orderBridge1?.EnablePaymentSystem();
            _orderBridge2?.CleanSession();
            _orderBridge2?.EnableSupplySystem();
            _orderBridge2?.EnablePaymentSystem();
            _orderBridge3?.CleanSession();
            _orderBridge3?.EnableSupplySystem();
            _orderBridge3?.EnablePaymentSystem();
            _userAdminBridge.CleanMarket();
        }

        private void SignUpStoreOwner(string name, string address, string password, string creditCard)
        {
            _storeOwnerBridge = UserDriver.getBridge();
            _storeOwnerBridge.EnterSystem();
            _storeOwnerBridge.SignUp(name, address, password, creditCard);
        }

        private void MakeRegisteredShoppers()
        {
            MakeRegisteredShopper1();
            MakeRegisteredShopper2();
            MakeRegisteredShopper3();
        }

        private void MakeRegisteredShopper1()
        {
            _buyerRegisteredUserBridge1 = UserDriver.getBridge();
            _buyerRegisteredUserBridge1.EnterSystem();
            _buyerRegisteredUserBridge1.SignUp("Shalom1", "Bye1", "555", "55555555");
            _orderBridge1 = OrderDriver.getBridge();
            _orderBridge1.GetOrderService(_buyerRegisteredUserBridge1.GetUserSession());

        }

        private void MakeRegisteredShopper2()
        {
            _buyerRegisteredUserBridge2 = UserDriver.getBridge();
            _buyerRegisteredUserBridge2.EnterSystem();
            _buyerRegisteredUserBridge2.SignUp("Shalom2", "Bye2", "555", "55555555");
            _orderBridge2 = OrderDriver.getBridge();
            _orderBridge2.GetOrderService(_buyerRegisteredUserBridge2.GetUserSession());
        }

        private void MakeRegisteredShopper3()
        {
            _buyerRegisteredUserBridge3 = UserDriver.getBridge();
            _buyerRegisteredUserBridge3.EnterSystem();
            _buyerRegisteredUserBridge3.SignUp("Shalom3", "Bye3", "555", "55555555");
            _orderBridge3 = OrderDriver.getBridge();
            _orderBridge3.GetOrderService(_buyerRegisteredUserBridge3.GetUserSession());
        }

        private void OpenStoreAndAddProducts()
        {
            _shoppingBridge = StoreShoppingDriver.getBridge();
            _shoppingBridge.GetStoreShoppingService(_storeOwnerBridge.GetUserSession());
            _shoppingBridge.OpenStore(storeName, "Great Place");
            _storeManagementBridge = StoreManagementDriver.getBridge();
            _storeManagementBridge.GetStoreManagementService(_storeOwnerBridge.GetUserSession(), storeName);
            _storeManagementBridge.AddNewLottery("Fanta", 12, "very nice fanta", Convert.ToDateTime("01/03/2018"), Convert.ToDateTime("28/12/2018"));
            _storeManagementBridge.AddNewLottery("Cola", 24, "very nice cola", Convert.ToDateTime("01/03/2018"), Convert.ToDateTime("28/12/2018"));
        }

        private void lotteryEventReport(string[] expected1, string[] expected2, string[] expected3,
            string[] actual1, string[] actual2, string[] actual3)
        {
            Assert.AreEqual(expected1.Length, actual1.Length);
            Assert.AreEqual(expected2.Length, actual2.Length);
            Assert.AreEqual(expected3.Length, actual3.Length);
            for (int i = 0; i < expected1.Length; i++)
            {
                Assert.AreEqual(expected1[i],actual1[i]);
            }
            for (int i = 0; i < expected2.Length; i++)
            {
                Assert.AreEqual(expected2[i], actual2[i]);
            }
            for (int i = 0; i < expected3.Length; i++)
            {
                Assert.AreEqual(expected3[i], actual3[i]);
            }

        }

    }
}
