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

namespace IntegrationTests.OrderSyncher_Integration
{
    [TestClass]
    public class OrderSyncher_IntegrationTest
    {
        private IUserService userServiceSession;
        private OrderSyncherHarmony orderSyncherHarmony;

        private MarketYard marketSession;

        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService)marketSession.GetUserService();
            userServiceSession.EnterSystem();
            orderSyncherHarmony = new OrderSyncherHarmony();
        }


        [TestMethod]
        public void TestRefund()
        {
            try
            {
                orderSyncherHarmony.CancelLottery("L2");
                OrderDL _orderDL = OrderDL.Instance;
                int userID = _orderDL.GetTicketParticipantID("T2");
                Assert.AreEqual(-1, userID);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestCleanup]
        public void StoreOrderTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}