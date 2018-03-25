using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace UserSpotTests
{

    [TestClass]
    public class UseCase1_2_Test
    {
        private UserService userServiceSession;
        private RegisteredUser registeredUser;
        [TestInitialize]
        public void MarketBuilder()
        {
            var marketSession = new MarketYard();
            userServiceSession = (UserService) marketSession.GetUserService();
            userServiceSession.EnterSystem();
            userServiceSession.SignUp("Maor","Here 3","123");
            registeredUser = (RegisteredUser) userServiceSession.GetUser();
        }
        [TestMethod]
        public void RegisteredUserDataTest()
        {
            object[] expectedData = { registeredUser.SystemID, "Maor", "Here 3", userServiceSession.GetSecuredPassword("123")};
            Assert.IsTrue(expectedData.SequenceEqual(registeredUser.ToData()));
          //  registeredUser.PromoteToAdmin();
          //  Assert.IsTrue(expectedData.SequenceEqual(registeredUser.ToData()));
        }

        [TestMethod]
        public void PromoteToAdminTest()
        {
            //    registeredUser.PromoteToAdmin();
            //   Assert.AreEqual(2, registeredUser.GetPolicies().Length);
        }

        [TestMethod]
        public void AqcuireStoreControlTest()
        {
            //   List<StoreAdminPolicy.StoreAction> newPolicies = new List<StoreAdminPolicy.StoreAction>();
            //   newPolicies.Add(StoreAdminPolicy.StoreAction.StoreOwner);
            //   newPolicies.Add(StoreAdminPolicy.StoreAction.DeclareDiscountPolicy);
            //   registeredUser.AddUserPolicy("UserSpotTest",newPolicies);
            //   registeredUser.PromoteToAdmin();
            //    registeredUser.
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            userServiceSession.CleanSession();
            MarketLog.RemoveLogs();
            MarketException.RemoveErrors();
        }
    }
}
