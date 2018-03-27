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
    public class UseCase2_1_Test
    {
        private UserService userServiceSignInSession;
        private UserService userServiceSignUpSession;
        private MarketYard marketSession;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = new MarketYard();
            userServiceSignInSession = (UserService)marketSession.GetUserService();
            userServiceSignUpSession = null;
        }

        [TestMethod]
        public void TestMethod1()
        {

        }

        private void doSignIn(string name, string password)
        {
            userServiceSignInSession.EnterSystem();
            userServiceSignInSession.SignIn(name, password);
        }
        private void doSignUp(string name,string address , string password)
        {
            userServiceSignUpSession = (UserService)marketSession.GetUserService();
            userServiceSignUpSession.EnterSystem();
            userServiceSignUpSession.SignUp(name, address, password);
            userServiceSignInSession.ReConnect();

        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            userServiceSignInSession.CleanSession();
            MarketLog.RemoveLogs();
            MarketException.RemoveErrors();
            marketSession.Exit();
        }
    }
}
