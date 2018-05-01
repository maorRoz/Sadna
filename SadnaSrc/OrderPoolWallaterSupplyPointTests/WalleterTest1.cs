﻿using System;
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
        private PaymentService paymentService;

        [TestInitialize]
        public void BuildWalleter()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;       
            paymentService = (PaymentService)market.GetPaymentService();
            paymentService.FixExternal();
        }

       

        [TestMethod]
        public void TestCreditCardCheck1()
        {
            paymentService.CheckCreditCard("12345678");
        }

        [TestMethod]
        public void TestCreditCardCheck2()
        {
            try
            {
                paymentService.CheckCreditCard("12345645678");

                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidCreditCardSyntax, e.Status);
            }
        }

        [TestMethod]
        public void TestCreditCardCheck3()
        {
            try
            {
                paymentService.CheckCreditCard("5678");

                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidCreditCardSyntax, e.Status);
            }
        }

        [TestMethod]
        public void TestCreditCardCheck4()
        {
            try
            {
                paymentService.CheckCreditCard("sdfsvx46");

                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidCreditCardSyntax, e.Status);
            }
        }


        [TestMethod]
        public void TestCheckRefundDetails1()
        {
            
                paymentService.CheckRefundDetails(3.2,"Big Smoke");

        }

        [TestMethod]
        public void TestCheckRefundDetails2()
        {
            try
            {
                paymentService.CheckRefundDetails(0, "Big Smoke");
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidData, e.Status);
            }
        }

        [TestMethod]
        public void TestCheckRefundDetails3()
        {
            try
            {
                paymentService.CheckRefundDetails(4.2, null);
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidData, e.Status);
            }
        }

        [TestMethod]
        public void TestProccessPayment()
        {
            Order order = new Order(123456, "Big Smoke","Grove Street");
            paymentService.ProccesPayment(order, "12345678");
        }

        [TestMethod]
        public void TestProccessPaymentBadData1()
        {
            Order order = new Order(123456, "Big Smoke", "Grove Street");
            try
            {
                paymentService.ProccesPayment(order, "1234ss5678");
                Assert.Fail();

            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidCreditCardSyntax, e.Status);
            }
        }

        [TestMethod]
        public void TestProccessPaymentBadData2()
        {
            Order order = new Order(123456, "Big Smoke", "Grove Street");

            try
            {
                paymentService.ProccesPayment(order, "45");
                Assert.Fail();

            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidCreditCardSyntax, e.Status);
            }
        }

        public void TestProccessPaymentBadData3()
        {
            Order order = new Order(123456, "Big Smoke", "Grove Street");

            try
            {
                paymentService.ProccesPayment(order, "45");
                Assert.Fail();

            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidCreditCardSyntax, e.Status);
            }
        }

        [TestMethod]
        public void TestBrokenExternalOnPayment()
        {
            Order order = new Order(123456, "Big Smoke", "Grove Street");
            paymentService.BreakExternal();
            try
            {
                paymentService.ProccesPayment(order, "12345678");
                Assert.Fail();

            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.PaymentSystemError, e.Status);
            }
        }

        [TestMethod]
        public void TestRefund()
        {
            paymentService.FixExternal();
            paymentService.Refund(3.0,"12345678","Big Smoke");
        }

        [TestMethod]
        public void TestBadRefund1()
        {

            try
            {
                paymentService.Refund(3.0, "12345678", null);
                Assert.Fail();

            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidData, e.Status);
            }
        }

        [TestMethod]
        public void TestBadRefund2()
        {
            try
            {
                paymentService.Refund(3.0, "123dd45678", "Big Smoke");
                Assert.Fail();

            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidCreditCardSyntax, e.Status);
            }
        }

        [TestMethod]
        public void TestBadRefund3()
        {
            try
            {
                paymentService.Refund(3.0, "8", "Big Smoke");
                Assert.Fail();

            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidCreditCardSyntax, e.Status);
            }
        }

        [TestMethod]
        public void TestBadRefund4()
        {

            try
            {
                paymentService.Refund(0, "12345678", "Big Smoke");
                Assert.Fail();

            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.InvalidData, e.Status);
            }
        }

        [TestMethod]
        public void TestBrokenExternalOnRefund()
        {
            Order order = new Order(123456, "Big Smoke", "Grove Street");
            paymentService.BreakExternal();
            try
            {
                paymentService.Refund(3.0, "12345678", "Big Smoke");
                Assert.Fail();

            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)WalleterStatus.PaymentSystemError, e.Status);
            }
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketYard.CleanSession();
        }
    }
}
