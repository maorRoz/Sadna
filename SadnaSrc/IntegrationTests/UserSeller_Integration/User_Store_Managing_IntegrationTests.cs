using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace IntegrationTests.UserSeller_Integration
{
    [TestClass]
    public class User_Store_Managing_IntegrationTests
    {
        private IUserService userServiceSession;
        private StoreManagementService storeServiceSession;
        private UserSellerHarmony userSellerHarmony;
        private MarketYard marketSession;

        private string store1 = "The Red Rock";
        private string store2 = "24";
        private string product1 = "Bamba";
        private string product2 = "Coated Peanuts";
        private string user = "Vova";
        private string pass = "123";

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService) marketSession.GetUserService();
            //userSellerHarmony = new UserSellerHarmony(ref userServiceSession);
            //storeServiceSession = (StoreShoppingService)marketSession.GetStoreShoppingService(ref userServiceSession);
        }

        [TestCleanup]
        public void StoreOrderTestCleanUp()
        {
            userServiceSession.CleanSession();
            storeServiceSession.CleanSession();
            MarketYard.CleanSession();
        }
    }
}
