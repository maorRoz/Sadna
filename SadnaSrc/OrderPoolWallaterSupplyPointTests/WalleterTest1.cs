using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using SadnaSrc.Walleter;

namespace OrderPoolWallaterSupplyPointTests
{
    [TestClass]
    public class WalleterTest1
    {
        private MarketYard market;
        private IUserService userService;
        private IStoreService storeService;
        private OrderService orderService;
        private PaymentService paymentService;
        private List<string> creditCard;

        [TestInitialize]
        public void BuildSupplyPoint()
        {
            market = MarketYard.Instance;
            userService = market.GetUserService();
            userService.EnterSystem();
            storeService = market.GetStoreService(userService);
            orderService = (OrderService)market.GetOrderService(ref userService, storeService);
            orderService.setUsername("Big Smoke");
            paymentService = new PaymentService(orderService);
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
