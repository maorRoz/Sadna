using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace IntegrationTests.UserSeller_Integration
{
    [TestClass]
    public class UserSeller_Promote_Tests
    {
        private IUserService userServiceSession;
        private IUserService userServiceSession2;
        private UserSellerHarmony userSellerHarmony;
        private MarketYard marketSession;

        private string store = "The Red Rock";
        private string owner = "Vova";
        private string manager = "Vadim Chernov";
        private string manager2 = "Big Smoke";
        private string shopper = "Arik2";
        private string pass = "123";
        private string storeAction1 = "ViewPurchaseHistory";
        private string storeAction2 = "ManageProducts";

        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService) marketSession.GetUserService();
            userServiceSession2 = (UserService)marketSession.GetUserService();
            userServiceSession.EnterSystem();
            userServiceSession2.EnterSystem();
            userSellerHarmony = new UserSellerHarmony(ref userServiceSession, store);
        }

        [TestMethod]
        public void GuestTryPromoteShopper()
        {
            try
            {
                SignInAndPromote("guset", shopper, storeAction1);
                Assert.Fail();
            }
            catch (MarketException)
            {
                userServiceSession2.SignIn(shopper, pass);
                Assert.AreEqual(0, ((UserService) userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
            }
        }

        [TestMethod]
        public void GuestTryPromoteManager()
        {
            try
            {
                SignInAndPromote("guset", manager, storeAction1);
                Assert.Fail();
            }
            catch (MarketException)
            {
                userServiceSession2.SignIn(manager, pass);
                Assert.AreEqual(2, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
            }
        }

        [TestMethod]
        public void GuestTryPromoteOwner()
        {
            try
            {
                SignInAndPromote("guset", owner, storeAction1);
                Assert.Fail();
            }
            catch (MarketException)
            {
                userServiceSession2.SignIn(owner, pass);
                Assert.AreEqual(1, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
            }
        }

        [TestMethod]
        public void OwnerPromoteShopper()
        {
            try
            {
                SignInAndPromote(owner, shopper, storeAction1);
                Assert.AreEqual(1, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
                Assert.AreEqual(StoreManagerPolicy.StoreAction.ViewPurchaseHistory,
                    ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store)[0].Action);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OwnerPromoteManager()
        {
            try
            {
                SignInAndPromote(owner, manager, storeAction1);
                Assert.AreEqual(1, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
                Assert.AreEqual(StoreManagerPolicy.StoreAction.ViewPurchaseHistory,
                    ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store)[0].Action);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OwnerPromoteHimself()
        {
            try
            {
                SignInAndPromote(owner, owner, storeAction1);
                Assert.Fail();
            }
            catch (MarketException)
            {
                userServiceSession2.SignIn(owner, pass);
                Assert.AreEqual(1, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
            }
        }

        [TestMethod]
        public void ManagerPromoteShopperFail()
        {
            try
            {
                SignInAndPromote(manager, shopper, storeAction1);
                Assert.Fail();
            }
            catch (MarketException)
            {
                userServiceSession2.SignIn(shopper, pass);
                Assert.AreEqual(0, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
            }
        }

        [TestMethod]
        public void ManagerPromoteOtherManagerFail()
        {
            try
            {
                SignInAndPromote(manager, manager2, storeAction1);
                Assert.Fail();
            }
            catch (MarketException)
            {
                userServiceSession2.SignIn(manager2, pass);
                Assert.AreEqual(1, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
            }
        }

        [TestMethod]
        public void ManagerPromoteShopper()
        {
            try
            {
                SignInAndPromote(manager, shopper, storeAction2);
                Assert.AreEqual(1, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
                Assert.AreEqual(StoreManagerPolicy.StoreAction.ManageProducts,
                    ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store)[0].Action);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerPromoteOtherManager()
        {
            try
            {
                SignInAndPromote(manager, manager2, storeAction2);
                Assert.AreEqual(1, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
                Assert.AreEqual(StoreManagerPolicy.StoreAction.ManageProducts,
                    ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store)[0].Action);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerPromoteHimself()
        {
            try
            {
                SignInAndPromote(manager, manager, storeAction1);
                Assert.Fail();
            }
            catch (MarketException)
            {
                userServiceSession2.SignIn(manager, pass);
                Assert.AreEqual(2, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
            }
        }

        [TestMethod]
        public void ManagerPromoteOwner()
        {
            try
            {
                SignInAndPromote(manager, owner, storeAction1);
                Assert.Fail();
            }
            catch (MarketException)
            {
                userServiceSession2.SignIn(owner, pass);
                Assert.AreEqual(1, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
            }
        }

        [TestCleanup]
        public void StoreOrderTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }

        private void SignInAndPromote(string promoter, string toPromote, string action)
        {
            if(promoter!="guest")
                userServiceSession.SignIn(promoter, pass);
            userSellerHarmony.Promote(toPromote, action);
            userServiceSession2.SignIn(toPromote, pass);

        }
    }
}
