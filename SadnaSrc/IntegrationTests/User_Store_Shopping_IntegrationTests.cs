using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace IntegrationTests
{
    [TestClass]
    public class User_Store_Shopping_IntegrationTests
    {
        private IUserService userServiceSession;
        private StoreShoppingService storeServiceSession;
        private UserShopperHarmony userShopperHarmony;
        private MarketYard marketSession;

        private string store1 = "The Red Rock";
        private string store2 = "24";
        private string user = "Vadim Chernov";
        private string pass = "123";

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService)marketSession.GetUserService();
            userShopperHarmony = new UserShopperHarmony(ref userServiceSession);
            storeServiceSession = (StoreShoppingService)marketSession.GetStoreShoppingService(ref userServiceSession);
        }

        [TestMethod]
        public void NotEnteredBrowseMarketTest()
        {
            try
            {
                userShopperHarmony.ValidateCanBrowseMarket();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void EnterSystemTest()
        {
            try
            {
                storeServiceSession.MakeGuest();
                Assert.IsNotNull(((UserService)userServiceSession).MarketUser);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void CanBrowseMarketAsGuestTest()
        {
            try
            {
                userShopperHarmony.MakeGuest();
                userShopperHarmony.ValidateCanBrowseMarket();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestCleanup]
        public void StoreOrderTestCleanUp()
        {
            userServiceSession.CleanSession();
            storeServiceSession.CleanSeesion();
            MarketYard.CleanSession();
        }
    }
}
