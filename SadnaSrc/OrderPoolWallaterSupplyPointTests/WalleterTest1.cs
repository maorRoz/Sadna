using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.Walleter;
using SadnaSrc.UserSpot;

namespace OrderPoolWallaterSupplyPointTests
{
    [TestClass]
    public class WalleterTest1
    {
        private MarketYard market;
        private OrderItem item1;
        private OrderItem item2;
        private OrderItem item3;
        private UserService userService;
        private StoreService storeService;
        private OrderService orderService;
        private PaymentService paymentService;
        private List<string> creditCard;

        [TestInitialize]
        public void BuildSupplyPoint()
        {
            market = MarketYard.Instance;
            userService = new UserService();
            storeService = new StoreService(userService);
            orderService = (OrderService)market.GetOrderService(userService, storeService);
            orderService.setUsername("Big Smoke");
            item1 = new OrderItem("Cluckin Bell", "#9", 5.00, 2);
            item2 = new OrderItem("Cluckin Bell", "#9 Large", 7.00, 1);
            item3 = new OrderItem("Cluckin Bell", "#6 Extra Dip", 8.50, 1);
            paymentService = new PaymentService(userService, orderService);
            creditCard = new List<string>();

        }

        [TestMethod]
        public void TestExternalSystemAttachment()
        {
            MarketAnswer ans = paymentService.AttachExternalSystem();
            Assert.AreEqual((int)WalleterStatus.Success, ans.Status);
        }

        [TestMethod]
        public void TestNoExternalSystem()
        {
            try
            {
                int orderId;
                orderService.CreateOrder(out orderId);
                paymentService.ProccesPayment(orderId, "Grove Street",creditCard);
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.NoPaymentSystem, e.Status);
            }
        }

        [TestMethod]
        public void TestProccessPayment()
        {
            paymentService.AttachExternalSystem();
            int orderId;
            orderService.CreateOrder(out orderId);
            creditCard.Add("1234");
            creditCard.Add("5678");
            creditCard.Add("9012");
            creditCard.Add("3456");
            creditCard.Add("05");
            creditCard.Add("19");
            creditCard.Add("123");
            MarketAnswer ans = paymentService.ProccesPayment(orderId, "Grove Street", creditCard);
            Assert.AreEqual((int)WalleterStatus.Success, ans.Status);
        }

        [TestMethod]
        public void TestBadCreditCard1()
        {
            try
            {
                paymentService.AttachExternalSystem();
                int orderId;
                orderService.CreateOrder(out orderId);
                creditCard.Add("12g5");
                creditCard.Add("5678");
                creditCard.Add("9012");
                creditCard.Add("3456");
                creditCard.Add("05");
                creditCard.Add("19");
                creditCard.Add("123");
                MarketAnswer ans = paymentService.ProccesPayment(orderId, "Grove Street", creditCard);
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidCreditCardSyntax, e.Status);
            }
        }

        [TestMethod]
        public void TestBadCreditCard2()
        {
            try
            {
                paymentService.AttachExternalSystem();
                int orderId;
                orderService.CreateOrder(out orderId);
                creditCard.Add("1245");
                creditCard.Add("5678");
                creditCard.Add("90172");
                creditCard.Add("3456");
                creditCard.Add("05");
                creditCard.Add("19");
                creditCard.Add("123");
                MarketAnswer ans = paymentService.ProccesPayment(orderId, "Grove Street", creditCard);
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidCreditCardSyntax, e.Status);
            }
        }

        [TestMethod]
        public void TestBadCreditCard3()
        {
            try
            {
                paymentService.AttachExternalSystem();
                int orderId;
                orderService.CreateOrder(out orderId);
                creditCard.Add("1245");
                creditCard.Add("5678");
                creditCard.Add("9012");
                creditCard.Add("3456");
                creditCard.Add("054");
                creditCard.Add("19");
                creditCard.Add("123");
                MarketAnswer ans = paymentService.ProccesPayment(orderId, "Grove Street", creditCard);
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidCreditCardSyntax, e.Status);
            }
        }

        [TestMethod]
        public void TestBadCreditCard4()
        {
            try
            {
                paymentService.AttachExternalSystem();
                int orderId;
                orderService.CreateOrder(out orderId);
                creditCard.Add("1245");
                creditCard.Add("5678");
                creditCard.Add("9012");
                creditCard.Add("3456");
                creditCard.Add("54");
                creditCard.Add("19");
                creditCard.Add("123");
                MarketAnswer ans = paymentService.ProccesPayment(orderId, "Grove Street", creditCard);
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidCreditCardSyntax, e.Status);
            }
        }

        [TestMethod]
        public void TestBadCreditCard5()
        {
            try
            {
                paymentService.AttachExternalSystem();
                int orderId;
                orderService.CreateOrder(out orderId);
                creditCard.Add("1245");
                creditCard.Add("5678");
                creditCard.Add("9012");
                creditCard.Add("3456");
                creditCard.Add("05");
                creditCard.Add("19");
                creditCard.Add("aaa");
                MarketAnswer ans = paymentService.ProccesPayment(orderId, "Grove Street", creditCard);
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidCreditCardSyntax, e.Status);
            }
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {

            userService.CleanSession();
            MarketYard.CleanSession();
        }
    }
}
