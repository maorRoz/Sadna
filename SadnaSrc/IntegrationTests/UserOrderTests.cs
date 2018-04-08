using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace IntegrationTests
{
    [TestClass]
    public class UserOrderTests
    {

        private UserService userServiceSession;
        private OrderService orderServiceSession;
        private StoreService storeServicesession;
        private MarketYard marketSession;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService)marketSession.GetUserService();
            storeServicesession = (StoreService)marketSession.GetStoreService(userServiceSession);
            orderServiceSession = (OrderService)marketSession.GetOrderService(userServiceSession, storeServicesession);
        }

        [TestMethod]
        public void TestMethod1()
        {
        }
    }
}
