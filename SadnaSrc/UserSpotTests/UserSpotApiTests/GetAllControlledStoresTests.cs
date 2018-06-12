﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketRecovery;
using SadnaSrc.UserSpot;

namespace UserSpotTests.UserSpotApiTests
{
    [TestClass]
    public class GetAllControlledStoresTests
    {
        private GetControlledStoreNamesSlave slave;
        private User guest;
        private RegisteredUser registered;
        private Mock<IMarketBackUpDB> marketDbMocker;
        private Mock<IUserDL> userDbMocker;
        private readonly int userID = 5000;

        [TestInitialize]
        public void MarketBuilder()
        {
            marketDbMocker = new Mock<IMarketBackUpDB>();
            MarketException.SetDB(marketDbMocker.Object);
            MarketLog.SetDB(marketDbMocker.Object);
            userDbMocker = new Mock<IUserDL>();
            guest = new User(userDbMocker.Object, userID);
            registered = new RegisteredUser(userDbMocker.Object,userID,"Moshe","Here 3","123","12345678",new CartItem[0],
                new StatePolicy[0],
                new []
                {
                    new StoreManagerPolicy("Store1",StoreManagerPolicy.StoreAction.StoreOwner),
                    new StoreManagerPolicy("Store2",StoreManagerPolicy.StoreAction.StoreOwner), 
                } );
        }

        [TestMethod]
        public void NoControlledStoresTest()
        {
	        
			slave = new GetControlledStoreNamesSlave(guest, userDbMocker.Object);
            slave.GetControlledStoreNames();
            var actual = slave.Answer.ReportList;
            Assert.AreEqual(0, actual.Length);
        }

        [TestMethod]
        public void GetControlledStoresTest()
        {
	        userDbMocker.Setup(x => x.GetAllActiveStoreNames()).Returns(new[] { "Store1", "Store2" });
			slave = new GetControlledStoreNamesSlave(registered, userDbMocker.Object);
            slave.GetControlledStoreNames();
            var expectedStores = new[] { "Store1", "Store2"};
            var actual = slave.Answer.ReportList;
            Assert.AreEqual(expectedStores.Length, actual.Length);
            for (int i = 0; i < expectedStores.Length; i++)
            {
                Assert.AreEqual(expectedStores[i], actual[i]);
            }

        }

        [TestMethod]
        public void DidntEnterTest()
        {
            slave = new GetControlledStoreNamesSlave(null, userDbMocker.Object);
            slave.GetControlledStoreNames();
            Assert.IsNull(slave.Answer.ReportList);
        }
    }
}
