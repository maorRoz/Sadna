using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace UserSpotTests
{
    [TestClass]
    public class UserTest
    {
        private UserService userServiceSession;
        private User guest;
        private int guestID = 10000;
        private int registeredID = 10001;
        private RegisteredUser registeredUser;
        [TestInitialize]
        public void MarketBuilder()
        {
            var marketSession = new MarketYard();
            userServiceSession = (UserService)marketSession.GetUserService();
            userServiceSession.EnterSystem();
            guest = new User(guestID);
            registeredUser = new RegisteredUser(registeredID,"Maor", "Here 3","123");
        }

        [TestMethod]
        public void GuestHasNoPolicyTest()
        {
            Assert.AreEqual(0, guest.GetPolicies().Length);
            User generatedGuest = userServiceSession.GetUser();
            Assert.AreEqual(0, generatedGuest.GetPolicies().Length);

        }

        [TestMethod]
        public void GuestDataTest()
        {
            object[] expectedData = {guestID, null, null, null};
            Assert.IsTrue(expectedData.SequenceEqual(guest.ToData()));
            User generatedGuest = userServiceSession.GetUser();
            expectedData[0] = generatedGuest.SystemID;
            Assert.IsTrue(expectedData.SequenceEqual(generatedGuest.ToData()));
        }

        [TestMethod]
        public void RegisteredDataTest()
        {
            object[] expectedData = {registeredID, "Maor", "Here 3", "123"};
            Assert.IsTrue(expectedData.SequenceEqual(registeredUser.ToData()));
            registeredUser.PromoteToAdmin();
            Assert.IsTrue(expectedData.SequenceEqual(registeredUser.ToData()));
        }

        [TestMethod]
        public void PromoteToAdminTest()
        {
            registeredUser.PromoteToAdmin();
            Assert.AreEqual(2, registeredUser.GetPolicies().Length);

        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            userServiceSession.ExitSystem();
        }
    }
}
