using System;
using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.SupplyPoint;
using SadnaSrc.Walleter;

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
        private IStoreManagementBridge _storeManagementBridge;
        private string storeName = "LotteryStore";

        [TestInitialize]
        public void MarketBuilder()
        {
            MarketYard.SetDateTime(Convert.ToDateTime("14/04/2018"));
            MarketDB.Instance.InsertByForce();
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
            PaymentService.Instance.FixExternal();
            SupplyService.Instance.FixExternal();
        }


        [TestMethod]
        public void LotteryStillGoing()
        {
            MakeRegisteredShoppers();
            Assert.AreEqual((int)OrderStatus.Success, _orderBridge1.BuyLotteryTicket("Fanta", storeName, 1, 4).Status);
            string[] expectedHistoryFirstBuyer =
            {
                "User: Shalom1 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy"),
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
                "User: Shalom1 Product: DELIVERY : Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 1 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy"),
                "User: Shalom1 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy"),
            };
            string[] expectedHistorySecondBuyer =
            {
                "User: Shalom2 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy"),
            };
            string[] expectedHistoryThirdBuyer =
            {
                "User: Shalom3 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy"),
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
                "User: Shalom1 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy")
            };
            string[] expectedHistorySecondBuyer =
            {
                "User: Shalom2 Product: DELIVERY : Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 1 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy"),
                "User: Shalom2 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy")
            };
            string[] expectedHistoryThirdBuyer =
            {
                "User: Shalom3 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy")
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
                "User: Shalom1 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy"),
            };
            string[] expectedHistorySecondBuyer =
            {
                "User: Shalom2 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy"),
            };
            string[] expectedHistoryThirdBuyer =
            {
                "User: Shalom3 Product: DELIVERY : Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 1 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy"),
                "User: Shalom3 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy"),
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
                "User: Shalom1 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy"),
                "User: Shalom1 Product: REFUND: Lottery Ticket Store: --- Sale: Lottery Quantity: 1 Price: -4 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy")
            };
            string[] expectedHistorySecondBuyer =
            {
                "User: Shalom2 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy"),
                "User: Shalom2 Product: REFUND: Lottery Ticket Store: --- Sale: Lottery Quantity: 1 Price: -4 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy")
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
            _buyerGuestBridge = UserDriver.getBridge();
            _buyerGuestBridge.EnterSystem();
            _orderBridge1 = OrderDriver.getBridge();
            _orderBridge1.GetOrderService(_buyerGuestBridge.GetUserSession());
            Assert.AreEqual((int)OrderStatus.InvalidUser, _orderBridge1.BuyLotteryTicket("Fanta", storeName, 1, 4).Status);
        }

        [TestMethod]
        public void FailPurchaseLotterySupplySystemCollapsed()
        {
            MakeRegisteredShopper1();
            _orderBridge1.DisableSupplySystem();
            Assert.AreEqual((int)OrderStatus.Success, _orderBridge1.BuyLotteryTicket("Fanta", storeName, 1, 4).Status);
            string[] expectedHistoryFirstBuyer =
            {
                "User: Shalom1 Product: Fanta Store: LotteryStore Sale: Lottery Quantity: 1 Price: 4 Date: "+DateTime.Now.Date.ToString("dd/MM/yyyy"),
            };
            string[] actualHistoryFirstBuyer = _adminBridge.ViewPurchaseHistoryByUser("Shalom1").ReportList;
            Assert.AreEqual(expectedHistoryFirstBuyer.Length,actualHistoryFirstBuyer.Length);
            for (int i = 0; i < expectedHistoryFirstBuyer.Length; i++)
            {
                Assert.AreEqual(expectedHistoryFirstBuyer[i],actualHistoryFirstBuyer[i]);
            }
        }

        [TestMethod]
        public void FailPurchaseLotteryPaymentSystemCollapsed()
        {
            MakeRegisteredShopper1();
            _orderBridge1.DisablePaymentSystem();
            Assert.AreEqual((int)WalleterStatus.PaymentSystemError, _orderBridge1.BuyLotteryTicket("Fanta", storeName, 1, 4).Status);
            string[] actualHistoryFirstBuyer = _adminBridge.ViewPurchaseHistoryByUser("Shalom1").ReportList;
            Assert.IsNull(actualHistoryFirstBuyer);
        }

        [TestCleanup]
        public void LotteryTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
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
            _storeManagementBridge.AddNewLottery("Fanta", 12, "very nice fanta", Convert.ToDateTime("14/04/2018"), Convert.ToDateTime("28/12/2018"));
            _storeManagementBridge.AddNewLottery("Cola", 24, "very nice cola", Convert.ToDateTime("14/04/2018"), Convert.ToDateTime("28/12/2018"));
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
