using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace IntegrationTests.UserSeller_Integration
{
    [TestClass]
    public class Promote_ManageProducts_IntegrationTests
    {
        private IUserService userServiceSession;
        private IUserService userServiceSession2;
        private StoreManagementService storeServiceSession;
        private MarketYard marketSession;

        private string store = "The Red Rock";
        private string owner = "Vova";
        private string manager = "Vadim Chernov";
        private string manager2 = "Big Smoke";
        private string shopper = "Arik2";
        private string sysadmin = "Arik1";
        private string pass = "123";
        private string storeAction1 = "ViewPurchaseHistory";
        private string storeAction2 = "ManageProducts";
        private string storeAction3 = "PromoteStoreAdmin";
        private string storeAction4 = "DeclareDiscountPolicy";
        private string storeOwner = "StoreOwner";
        private string newProduct = "Coated Peanuts";
        private string existingProduct = "Bamba";

        [TestInitialize]
        public void MarketBuilder()
        {
            marketSession = MarketYard.Instance;
            userServiceSession = (UserService) marketSession.GetUserService();
            userServiceSession2 = (UserService)marketSession.GetUserService();
            userServiceSession.EnterSystem();
            userServiceSession2.EnterSystem();
            storeServiceSession =
                (StoreManagementService) marketSession.GetStoreManagementService(userServiceSession, store);
        }

        /*
         * Promotion tests
         */

        [TestMethod]
        public void GuestTryPromoteShopper()
        {
            try
            {
                SignInAndPromote("guest", shopper, storeAction1);
                Assert.AreEqual(0, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);

            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GuestTryPromoteManager()
        {
            try
            {
                SignInAndPromote("guest", manager, storeAction1);
                Assert.AreEqual(2, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);

            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void GuestTryPromoteOwner()
        {
            try
            {
                SignInAndPromote("guest", owner, storeAction1);
                Assert.AreEqual(1, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);

            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OwnerPromoteShopper()
        {
            try
            {
                SignInAndPromote(owner, shopper, storeAction1);
                Assert.AreEqual(1, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
                Assert.AreEqual(StoreManagerPolicy.StoreAction.ViewPurchaseHistory,
                    ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store)[0].Action);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OwnerPromoteManager()
        {
            try
            {
                SignInAndPromote(owner, manager, storeAction1);
                Assert.AreEqual(1, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
                Assert.AreEqual(StoreManagerPolicy.StoreAction.ViewPurchaseHistory,
                    ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store)[0].Action);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OwnerPromoteHimself()
        {
            try
            {
                SignInAndPromote(owner, owner, storeAction1);
                Assert.AreEqual(1, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerPromoteShopperFail()
        {
            try
            {
                SignInAndPromote(manager, shopper, storeAction1);
                Assert.AreEqual(0, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerPromoteShopper()
        {
            try
            {
                SignInAndPromote(manager, shopper, storeAction2);
                Assert.AreEqual(1, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
                Assert.AreEqual(StoreManagerPolicy.StoreAction.ManageProducts,
                    ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store)[0].Action);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerPromoteHimself()
        {
            try
            {
                SignInAndPromote(manager, manager, storeAction1);
                Assert.AreEqual(2, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerPromoteOwner()
        {
            try
            {
                SignInAndPromote(manager, owner, storeAction1);
                Assert.AreEqual(1, ((UserService)userServiceSession2).MarketUser.GetStoreManagerPolicies(store).Length);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        /*
         * Add Product tests
         */

        [TestMethod]
        public void GuestAddProduct()
        {
            try
            {
                storeServiceSession.AddNewProduct(newProduct, 8, "munch", 50);
                ModuleGlobalHandler.GetInstance().DataLayer.GetProductFromStore(store, newProduct);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void ShopperAddProduct()
        {
            try
            {
                userServiceSession.SignIn(shopper, pass);
                storeServiceSession.AddNewProduct(newProduct, 8, "munch", 50);
                ModuleGlobalHandler.GetInstance().DataLayer.GetProductFromStore(store, newProduct);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void ManagerAddProduct()
        {
            try
            {
                userServiceSession.SignIn(manager, pass);
                storeServiceSession.AddNewProduct(newProduct, 8, "munch", 50);
                Assert.AreEqual(50, ModuleGlobalHandler.GetInstance().DataLayer.GetProductFromStore(store, newProduct).Quantity);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerAddProduct2()
        {
            try
            {
                userServiceSession.SignIn(manager2, pass);
                storeServiceSession.AddNewProduct(newProduct, 8, "munch", 50);
                ModuleGlobalHandler.GetInstance().DataLayer.GetProductFromStore(store, newProduct);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void OwnerAddProduct()
        {
            try
            {
                userServiceSession.SignIn(owner, pass);
                storeServiceSession.AddNewProduct(newProduct, 8, "munch", 50);
                Assert.AreEqual(50, ModuleGlobalHandler.GetInstance().DataLayer.GetProductFromStore(store, newProduct).Quantity);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void SysAdminAddProduct()
        {
            try
            {
                userServiceSession.SignIn(sysadmin, pass);
                storeServiceSession.AddNewProduct(newProduct, 8, "munch", 50);
                Assert.AreEqual(50, ModuleGlobalHandler.GetInstance().DataLayer.GetProductFromStore(store, newProduct).Quantity);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        /*
         * Remove Product tests
         */

        [TestMethod]
        public void GuestRemoveProduct()
        {
            try
            {
                SignInAndRemoveProduct("guest", existingProduct);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ShopperRemoveProduct()
        {
            try
            {
                SignInAndRemoveProduct(shopper, existingProduct);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerRemoveProduct()
        {
            try
            {
                SignInAndRemoveProduct(manager, existingProduct);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void ManagerRemoveProduct2()
        {
            try
            {
                SignInAndRemoveProduct(manager2, existingProduct);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void OwnerRemoveProduct()
        {
            try
            {
                SignInAndRemoveProduct(owner, existingProduct);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        /*
         * Edit Product tests
         */

        [TestMethod]
        public void GuestEditProduct()
        {
            try
            {
                SignInAndEditProduct("guest", existingProduct);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void ShopperEditProduct()
        {
            try
            {
                SignInAndEditProduct(shopper, existingProduct);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void ManagerEditProduct()
        {
            try
            {
                SignInAndEditProduct(manager, existingProduct);
            }
            catch (MarketException)
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void ManagerEditProduct2()
        {
            try
            {
                SignInAndEditProduct(manager2, existingProduct);
                Assert.Fail();
            }
            catch (MarketException)
            {
            }
        }

        [TestMethod]
        public void OwnerEditProduct()
        {
            try
            {
                SignInAndEditProduct(owner, existingProduct);
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
            userServiceSession2.CleanSession();
            storeServiceSession.CleanSession();
            MarketYard.CleanSession();
        }

        private void SignInAndPromote(string promoter, string toPromote, string action)
        {
            SignIn(promoter);
            storeServiceSession.PromoteToStoreManager(toPromote, action);
            userServiceSession2.SignIn(toPromote, pass);
        }

        private void SignInAndRemoveProduct(string user, string product)
        {
            SignIn(user);
            storeServiceSession.RemoveProduct(product);
            ModuleGlobalHandler.GetInstance().DataLayer.GetProductFromStore(store, product);

        }

        private void SignInAndEditProduct(string user, string product)
        {
            SignIn(user);
            storeServiceSession.EditProduct(product, "Name", "Bambaa");
            ModuleGlobalHandler.GetInstance().DataLayer.GetProductFromStore(store, "Bambaa");
        }

        private void SignIn(string user)
        {
            if (user != "guest")
                userServiceSession.SignIn(user, pass);
        }
    }
}
