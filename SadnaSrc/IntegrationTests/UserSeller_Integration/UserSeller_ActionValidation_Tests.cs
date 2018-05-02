using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace IntegrationTests.UserSeller_Integration
{
    [TestClass]
    public class UserSeller_ActionValidation_Tests
    {
        private IUserService userServiceSession;
        private UserSellerHarmony userSellerHarmony;
        private MarketYard marketSession;

        private string store = "The Red Rock";
        private string owner = "Vova";
        private string manager = "Vadim Chernov";
        private string sysadmin = "Arik1";
        private string shopper = "Arik2";
        private string pass = "123";

        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService) marketSession.GetUserService();
            userServiceSession.EnterSystem();
            userSellerHarmony = new UserSellerHarmony(ref userServiceSession, store);
        }

        [TestMethod]
        public void GuestManageStoreTest1()
        {
            try
            {
                userSellerHarmony.CanDeclareDiscountPolicy();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void GuestManageStoreTest2()
        {
            try
            {
                userSellerHarmony.CanDeclarePurchasePolicy();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void GuestManageStoreTest3()
        {
            try
            {
                userSellerHarmony.CanManageProducts();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void GuestManageStoreTest4()
        {
            try
            {
                userSellerHarmony.CanPromoteStoreAdmin();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void GuestManageStoreTest5()
        {
            try
            {
                userSellerHarmony.CanPromoteStoreOwner();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void GuestManageStoreTest6()
        {
            try
            {
                userSellerHarmony.CanViewPurchaseHistory();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void OwnerManageStoreTest1()
        {
            try
            {
                userServiceSession.SignIn(owner, pass);
                userSellerHarmony.CanDeclareDiscountPolicy();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OwnerManageStoreTest2()
        {
            try
            {
                userServiceSession.SignIn(owner, pass);
                userSellerHarmony.CanDeclarePurchasePolicy();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OwnerManageStoreTest3()
        {
            try
            {
                userServiceSession.SignIn(owner, pass);
                userSellerHarmony.CanManageProducts();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OwnerManageStoreTest4()
        {
            try
            {
                userServiceSession.SignIn(owner, pass);
                userSellerHarmony.CanPromoteStoreAdmin();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OwnerManageStoreTest5()
        {
            try
            {
                userServiceSession.SignIn(owner, pass);
                userSellerHarmony.CanPromoteStoreOwner();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OwnerManageStoreTest6()
        {
            try
            {
                userServiceSession.SignIn(owner, pass);
                userSellerHarmony.CanViewPurchaseHistory();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerManageStoreTest1()
        {
            try
            {
                userServiceSession.SignIn(manager, pass);
                userSellerHarmony.CanDeclareDiscountPolicy();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void ManagerManageStoreTest2()
        {
            try
            {
                userServiceSession.SignIn(manager, pass);
                userSellerHarmony.CanPromoteStoreAdmin();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerManageStoreTest3()
        {
            try
            {
                userServiceSession.SignIn(manager, pass);
                userSellerHarmony.CanDeclarePurchasePolicy();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void ManagerManageStoreTest4()
        {
            try
            {
                userServiceSession.SignIn(manager, pass);
                userSellerHarmony.CanManageProducts();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerManageStoreTest5()
        {
            try
            {
                userServiceSession.SignIn(manager, pass);
                userSellerHarmony.CanPromoteStoreOwner();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void ManagerManageStoreTest6()
        {
            try
            {
                userServiceSession.SignIn(manager, pass);
                userSellerHarmony.CanViewPurchaseHistory();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void SystemAdminManageStoreTest1()
        {
            try
            {
                userServiceSession.SignIn(sysadmin, pass);
                userSellerHarmony.CanDeclareDiscountPolicy();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void SystemAdminManageStoreTest2()
        {
            try
            {
                userServiceSession.SignIn(sysadmin, pass);
                userSellerHarmony.CanPromoteStoreAdmin();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void SystemAdminManageStoreTest3()
        {
            try
            {
                userServiceSession.SignIn(sysadmin, pass);
                userSellerHarmony.CanDeclarePurchasePolicy();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void SystemAdminManageStoreTest4()
        {
            try
            {
                userServiceSession.SignIn(sysadmin, pass);
                userSellerHarmony.CanManageProducts();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void SystemAdminManageStoreTest5()
        {
            try
            {
                userServiceSession.SignIn(sysadmin, pass);
                userSellerHarmony.CanPromoteStoreOwner();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void SystemAdminManageStoreTest6()
        {
            try
            {
                userServiceSession.SignIn(sysadmin, pass);
                userSellerHarmony.CanViewPurchaseHistory();
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ShopperManageStoreTest1()
        {
            try
            {
                userServiceSession.SignIn(shopper, pass);
                userSellerHarmony.CanDeclareDiscountPolicy();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void ShopperManageStoreTest2()
        {
            try
            {
                userServiceSession.SignIn(shopper, pass);
                userSellerHarmony.CanPromoteStoreAdmin();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void ShopperManageStoreTest3()
        {
            try
            {
                userServiceSession.SignIn(shopper, pass);
                userSellerHarmony.CanDeclarePurchasePolicy();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void ShopperManageStoreTest4()
        {
            try
            {
                userServiceSession.SignIn(shopper, pass);
                userSellerHarmony.CanManageProducts();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void ShopperManageStoreTest5()
        {
            try
            {
                userServiceSession.SignIn(shopper, pass);
                userSellerHarmony.CanPromoteStoreOwner();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void ShopperManageStoreTest6()
        {
            try
            {
                userServiceSession.SignIn(shopper, pass);
                userSellerHarmony.CanViewPurchaseHistory();
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestCleanup]
        public void StoreOrderTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }
    }
}
