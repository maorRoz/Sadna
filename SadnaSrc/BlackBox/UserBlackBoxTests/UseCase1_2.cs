using System;
using BlackBox;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;

namespace BlackBox.UserBlackBoxTests
{
    [TestClass]
    public class UseCase1_2
    {
        private IUserBridge _bridge;
        private IUserBridge _bridge2;

        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            _bridge = UserDriver.getBridge();
            _bridge2 = null;
        }
        [TestMethod]

        public void RegistrationSucceeded()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.Success, _bridge.SignUp("PninaBashkanski", "miahol susia 12", "123456", "12345678").Status);
        }

        [TestMethod]

        public void RegistrationWithoutEnteringTheSystem()
        {
            Assert.AreEqual((int)SignUpStatus.DidntEnterSystem, _bridge.SignUp("Pnina", "miahol susia 12", "123456", "12345678").Status);
        }

        [TestMethod]

        public void RegistrationWithATakenName()
        {
            _bridge.EnterSystem();
            _bridge.SignUp("Pnina", "miahol susia 12", "123456", "12345678");
            _bridge2 = UserDriver.getBridge();
            _bridge2.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.TakenName, _bridge2.SignUp("Pnina", "miahol susia 12", "123456", "12345678").Status);
        }

        [TestMethod]

        public void SignUpMoreThanOnce()
        {
            _bridge.EnterSystem();
            _bridge.SignUp("PninaBas", "mishol susia 8", "123852", "12345678");
            Assert.AreEqual((int)SignUpStatus.SignedUpAlready, _bridge.SignUp("PninaBas", "mishol susia 8", "123852", "12345678").Status);

        }

        [TestMethod]

        public void UserNameToSignUpIsNull()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp(null, "miahol susia 12", "123456", "12345678").Status);
        }

        [TestMethod]

        public void AddressToSignUpIsNull()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp("Pnina", null, "123456", "12345678").Status);
        }

        [TestMethod]

        public void PasswordToSignUpIsNull()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp("Pnina", "mishol susia 12", null, "12345678").Status);
        }

        [TestMethod]

        public void UserNameAndAddressToSignUpAreNull()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp(null, null, "123456", "12345678").Status);
        }

        [TestMethod]

        public void UserNameAndPasswordToSignUpAreNull()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp(null, "mishol susia 12", null, "12345678").Status);
        }

        [TestMethod]

        public void AddressAndPasswordToSignUpAreNull()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp("Pnina", null, null, "12345678").Status);
        }

        [TestMethod]

        public void UserNameAddressAndPasswordAreNull()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp(null, null, null, "12345678").Status);
        }


        [TestMethod]

        public void UserNameToSignUpIsEmpty()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp("", "miahol susia 12", "123456", "12345678").Status);
        }

        [TestMethod]

        public void AddressToSignUpIsEmpty()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp("Pnina", "", "123456", "12345678").Status);
        }

        [TestMethod]

        public void PasswordToSignUpIsEmpty()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp("Pnina", "mishol susia 12", "", "12345678").Status);
        }

        [TestMethod]

        public void UserNameAndAddressToSignUpAreEmpty()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp("", "", "123456", "12345678").Status);
        }

        [TestMethod]

        public void UserNameAndPasswordToSignUpAreEmpty()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp("", "mishol susia 12", "", "12345678").Status);
        }

        [TestMethod]
        public void AddressAndPasswordToSignUpAreEmpty()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp("Pnina", "", "", "12345678").Status);
        }

        [TestMethod]
        public void CreditCardIsTooShort()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp("Pnina", "mishol susia 12", "123", "123456").Status);
        }

        [TestMethod]
        public void CreditCardIsTooLong()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp("Pnina", "mishol susia 12", "123", "123456789").Status);
        }

        [TestMethod]
        public void CreditCardHasInvalidChars()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp("Pnina", "mishol susia 12", "123", "123%&*(8").Status);
        }

        [TestMethod]
        public void UserNameAddressAndPasswordAreEmpty()
        {
            _bridge.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.NullEmptyFewDataGiven, _bridge.SignUp("", "", "", "12345678").Status);
        }


        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}