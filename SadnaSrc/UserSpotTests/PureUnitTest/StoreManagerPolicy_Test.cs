using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace UserSpotTests.PureUnitTest
{
    [TestClass]
    public class StoreManagerPolicy_Test
    {
        private UserService userServiceSignInSession;
        private UserService userServiceSignUpSession;
        private UserService userServiceSignInSystemAdminSession;
        private UserService userServiceSignUpSystemAdminSessionSession;
        private MarketYard marketSession;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSignInSession = null;
            userServiceSignUpSession = (UserService)marketSession.GetUserService();
            userServiceSignUpSystemAdminSessionSession = (UserService)marketSession.GetUserService();
            userServiceSignInSystemAdminSession = null;
        }

        [TestMethod]
        public void AddPromoteToStoreAdminPolicyTest()
        {

        }

        [TestMethod]
        public void AddManageProductsPolicyTest()
        {

        }

        [TestMethod]
        public void AddDeclarePurchasePolicyTest()
        {

        }

        [TestMethod]
        public void AddViewPurchasePolicyTest()
        {

        }

        [TestMethod]
        public void AddStoreOwnerPolicyTest()
        {

        }

        [TestMethod]
        public void AddNoPolicyTest()
        {

        }
        [TestMethod]
        public void AddMoreThenOnePolicyTest()
        {

        }

        [TestMethod]
        public void AddStoreOwnerAndMorePolicyTest()
        {

        }

        [TestMethod]
        public void AddMoreThenOnePolicyFromDifferentStoresTest()
        {

        }

        [TestMethod]
        public void AddStoreOwnerAndMorePolicyFromDifferentStoresTest()
        {

        }

        [TestMethod]
        public void AddStoreOwnerForDifferenetStoresTest()
        {

        }

        [TestMethod]
        public void AddMorePoliciesLaterTest()
        {

        }

        [TestMethod]
        public void AddMorePoliciesLaterToStoreOwnerTest()
        {

        }
        [TestMethod]
        public void AddStoreOwnerLaterTest()
        {

        }

        [TestMethod]
        public void AddStoreOwnerFromDifferentStoreLaterTest()
        {

        }

        [TestMethod]
        public void AddStoreOwnerFromDifferentStoreToStoreOwnerLaterTest()
        {

        }


        [TestCleanup]
        public void UserTestCleanUp()
        {
            userServiceSignUpSystemAdminSessionSession.CleanSession();
            userServiceSignInSystemAdminSession?.CleanSession();
            userServiceSignUpSession.CleanSession();
            userServiceSignInSession?.CleanSession();
            MarketYard.CleanSession();
        }

        private void DoSignUpSignIn(string name, string address, string password)
        {
            DoSignUp(name, address, password);
            Assert.IsFalse(MarketException.hasErrorRaised());
            DoSignIn(name, password);
        }
        private void DoSignIn(string name, string password)
        {
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.Success, userServiceSignInSession.SignIn(name, password).Status);
        }
        private void DoSignUp(string name, string address, string password)
        {
            userServiceSignUpSession = (UserService)marketSession.GetUserService();
            userServiceSignUpSession.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.Success, userServiceSignUpSession.SignUp(name, address, password).Status);
            userServiceSignInSession.Synch();

        }
    }
}
