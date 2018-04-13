﻿using System;
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

        /*
         * User validation tests
         */

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
        public void CanBrowseMarketAsGuestTest()
        {
            try
            {
                storeServiceSession.MakeGuest();
                userShopperHarmony.ValidateCanBrowseMarket();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void NotRegisteredUserTest()
        {
            try
            {
                storeServiceSession.MakeGuest();
                userShopperHarmony.ValidateRegistered();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void ValidateRegisteredTest()
        {
            try
            {
                storeServiceSession.LoginShoper(user, pass);
                userShopperHarmony.ValidateRegistered();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void NonExistantUserLoginTest()
        {
            try
            {
                storeServiceSession.LoginShoper("A" + user, pass);
                userShopperHarmony.ValidateRegistered();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void GuestIDTest()
        {
            try
            {
                storeServiceSession.MakeGuest();
                Assert.AreEqual(((UserService)userServiceSession).MarketUser.SystemID, userShopperHarmony.GetShopperID());
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void RegUserTest()
        {
            try
            {
                storeServiceSession.LoginShoper(user, pass);
                Assert.AreEqual(((UserService)userServiceSession).MarketUser.SystemID, userShopperHarmony.GetShopperID());
                Assert.AreEqual(((RegisteredUser)((UserService)userServiceSession).MarketUser).Name, 
                    userShopperHarmony.GetShopperName());
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
