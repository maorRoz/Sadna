using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace IntegrationTests
{
    [TestClass]
    public class OrderSyncher_IntegrationTest
    {
        private IUserService userServiceSession;
        private OrderService orderServiceSession;
        private ModuleGlobalHandler storeServiceSession;
        private OrderSyncherHarmony orderSyncherHarmony;

        private MarketYard marketSession;
        private string store1 = "The Red Rock";
        private string store2 = "24";
        private string product1 = "Bamba";
        private string product2 = "Coated Peanuts";

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService)marketSession.GetUserService();
            userServiceSession.EnterSystem();
            orderServiceSession = (OrderService)marketSession.GetOrderService(ref userServiceSession);
            storeServiceSession = ModuleGlobalHandler.GetInstance();
            orderSyncherHarmony = new OrderSyncherHarmony();
        }


        [TestMethod]
        public void TestRefund()
        {
            try
            {
                orderSyncherHarmony.CancelLottery("L2");
                OrderPoolDL _orderDL = new OrderPoolDL();
                _orderDL.GetTicketParticipantID("T2");
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)LotteryOrderStatus.InvalidLotteryTicket, e.Status);
            }
        }

        [TestCleanup]
        public void StoreOrderTestCleanUp()
        {
            userServiceSession.CleanGuestSession();
            orderSyncherHarmony.CleanOrderSyncherSession();
            MarketYard.CleanSession();
        }
    }
}