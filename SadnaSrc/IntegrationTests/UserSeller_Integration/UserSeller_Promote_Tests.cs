using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace IntegrationTests.UserSeller_Integration
{
    [TestClass]
    public class UserSeller_Promote_Tests
    {
        private IUserService userServiceSession;
        private UserSellerHarmony userSellerHarmony;
        private MarketYard marketSession;

        private string store = "The Red Rock";
        private string owner = "Vova";
        private string manager = "Vadim Chernov";
        private string sysadmin = "Arik1";
        private string shopper = "Arik2";
        private string pass = "123";
        private string storeAction = "ViewPurchaseHistory";
        private string storeOwner = "StoreOwner";

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService) marketSession.GetUserService();
            userServiceSession.EnterSystem();
            userSellerHarmony = new UserSellerHarmony(ref userServiceSession, store);
        }

        [TestMethod]
        public void GuestTryPromoteShopper()
        {
            try
            {
                userSellerHarmony.Promote(shopper, storeAction);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void GuestTryPromoteManager()
        {
            try
            {
                userSellerHarmony.Promote(manager, storeAction);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void GuestTryPromoteOwner()
        {
            try
            {
                userSellerHarmony.Promote(owner, storeAction);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void GuestTryPromoteSysAdmin()
        {
            try
            {
                userSellerHarmony.Promote(sysadmin, storeAction);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestCleanup]
        public void StoreOrderTestCleanUp()
        {
            userServiceSession.CleanSession();
            MarketYard.CleanSession();
        }
    }
}
