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
        private PaymentService paymentService;

        [TestInitialize]
        public void BuildSupplyPoint()
        {
            market = MarketYard.Instance;       
            paymentService = (PaymentService)market.GetPaymentService();

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
                Order order = new Order(123456, "Grove Street");
                
                paymentService.ProccesPayment(order, "12345678");
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
          
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {

            MarketYard.CleanSession();
        }
    }
}
