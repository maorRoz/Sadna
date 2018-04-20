using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;
namespace UserSpotTests.UserSpotApiTests
{
    [TestClass]
    class SignInTests
    {
        private SignInSlave slave;
        private int registeredUserID = 5000;
        private User guestUser;
        private string registeredUserName = "MaorLogin";
        private string registeredUserAddress = "Here 3";
        private string registeredUserPassword = "123";
        private string encryptedUserPassword = UserSecurityService.GetSecuredPassword("123");
        private string registeredUserCreditCard = "12345678";
        private Mock<IMarketDB> marketDbMocker;
        private Mock<IUserDL> userDbMocker;
        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            userDbMocker = new Mock<IUserDL>();
            userDbMocker.Setup(x => x.RegisterUser(It.IsAny<int>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CartItem[]>()))
                .Returns(new RegisteredUser(userDbMocker.Object, registeredUserID, registeredUserName, registeredUserAddress,
                    encryptedUserPassword, registeredUserCreditCard, new CartItem[0],
                    new[] { new StatePolicy(StatePolicy.State.RegisteredUser) }, new StoreManagerPolicy[0]));
            userDbMocker.Setup(x => x.IsUserNameExist(It.IsAny<string>())).Returns(false);
            guestUser = new User(userDbMocker.Object, registeredUserID);
            slave = new SignInSlave(guestUser, userDbMocker.Object);
        }
    }
}
