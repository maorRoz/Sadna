﻿using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox.UserBlackBoxTests
{
    [TestClass]
    public class UseCase1_1
    {
        private IUserBridge _bridge;

        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            _bridge = UserDriver.getBridge();
        }

        [TestMethod]
        public void SuccessGuestEntry()
        {
            Assert.AreEqual((int)EnterSystemStatus.Success, _bridge.EnterSystem().Status);
        }

        [TestCleanup]
        public void UserTestCleanUp()
        {
            _bridge.CleanSession();
            _bridge.CleanMarket();

        }

    }
}