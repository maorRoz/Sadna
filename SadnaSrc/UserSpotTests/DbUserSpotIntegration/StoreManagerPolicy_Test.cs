using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.UserSpot;

namespace UserSpotTests.DbUserSpotIntegration
{
    [TestClass]
    public class StoreManagerPolicy_Test
    {
        private UserService userServiceSignInSession;
        private UserService userServiceSignInSession2;
        private UserService userServiceSignUpSession;
        private MarketYard marketSession;
        private string promoteName = "MaorStorePolicy";
        private string promotePassword = "123";
        private string store1 = "X";
        private string store2 = "Y";
        private string store3 = "M";

        [TestInitialize]
        public void MarketBuilder()
        {
            MarketDB.Instance.InsertByForce();
            marketSession = MarketYard.Instance;
            DoSignUp(promoteName, "no-where", promotePassword, "12345678");
            userServiceSignInSession = null;
            userServiceSignInSession2 = null;
        }

        [TestMethod]
        public void AddPromoteToStoreAdminPolicyTest()
        {
            AddOnePolicy(StoreManagerPolicy.StoreAction.PromoteStoreAdmin);

        }

        [TestMethod]
        public void AddManageProductsPolicyTest()
        {
            AddOnePolicy(StoreManagerPolicy.StoreAction.ManageProducts);
        }

        [TestMethod]
        public void AddDeclarePurchasePolicyTest()
        {
            AddOnePolicy(StoreManagerPolicy.StoreAction.DeclarePurchasePolicy);
        }

        [TestMethod]
        public void AddDeclareDiscountPolicyTest()
        {
            AddOnePolicy(StoreManagerPolicy.StoreAction.DeclareDiscountPolicy);
        }

        [TestMethod]
        public void AddViewPurchasePolicyTest()
        {
            AddOnePolicy(StoreManagerPolicy.StoreAction.ViewPurchaseHistory);
        }

        [TestMethod]
        public void AddStoreOwnerPolicyTest()
        {
            AddOnePolicy(StoreManagerPolicy.StoreAction.StoreOwner);
        }

        [TestMethod]
        public void AddNoPolicyTest()
        {
            Assert.AreEqual(0, userServiceSignUpSession.MarketUser.GetStoreManagerPolicies(store1).Length);
            UserPolicyService.PromoteStorePolicies(promoteName, store1, new StoreManagerPolicy.StoreAction[0]);
            DoSignIn(promoteName, promotePassword);
            StoreManagerPolicy[] actions = userServiceSignInSession.MarketUser.GetStoreManagerPolicies(store1);
            Assert.AreEqual(0, actions.Length);
        }
        [TestMethod]
        public void AddMoreThenOnePolicyTest()
        {
            StoreManagerPolicy.StoreAction[] givenPermissions =
            {
                StoreManagerPolicy.StoreAction.ManageProducts,
                StoreManagerPolicy.StoreAction.PromoteStoreAdmin,
                StoreManagerPolicy.StoreAction.DeclareDiscountPolicy,
                StoreManagerPolicy.StoreAction.DeclarePurchasePolicy,
                StoreManagerPolicy.StoreAction.ViewPurchaseHistory

            };
            StoreManagerPolicy[] expected =
            {
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.PromoteStoreAdmin),
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.ManageProducts),
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.DeclarePurchasePolicy),
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.DeclareDiscountPolicy),
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.ViewPurchaseHistory),
            };
            AddMoreThenOnePolicy(expected,givenPermissions);
        }

        [TestMethod]
        public void AddStoreOwnerAndMorePolicyTest()
        {
            StoreManagerPolicy.StoreAction[] givenPermissions =
            {
                StoreManagerPolicy.StoreAction.ManageProducts,
                StoreManagerPolicy.StoreAction.PromoteStoreAdmin,
                StoreManagerPolicy.StoreAction.StoreOwner,

            };
            StoreManagerPolicy[] expected =
            {
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.StoreOwner),
            };
            AddMoreThenOnePolicy(expected, givenPermissions);
        }

        [TestMethod]
        public void AddMoreThenOnePolicyFromDifferentStoresTest()
        {
            StoreManagerPolicy.StoreAction[] givenPermissions1 =
            {
                StoreManagerPolicy.StoreAction.ManageProducts,
                StoreManagerPolicy.StoreAction.ViewPurchaseHistory

            };
            StoreManagerPolicy.StoreAction[] givenPermissions2 =
            {
                StoreManagerPolicy.StoreAction.ManageProducts,
                StoreManagerPolicy.StoreAction.PromoteStoreAdmin,
                StoreManagerPolicy.StoreAction.StoreOwner,

            };
            StoreManagerPolicy.StoreAction[] givenPermissions3 =
            {
                StoreManagerPolicy.StoreAction.PromoteStoreAdmin,
                StoreManagerPolicy.StoreAction.ViewPurchaseHistory,
                StoreManagerPolicy.StoreAction.ManageProducts
            };
            StoreManagerPolicy[] expected1 =
            {
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.ManageProducts),
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.ViewPurchaseHistory)
            };
            StoreManagerPolicy[] expected2 =
            {
                new StoreManagerPolicy(store2,StoreManagerPolicy.StoreAction.StoreOwner)
            };
            StoreManagerPolicy[] expected3 =
            {
                new StoreManagerPolicy(store3,StoreManagerPolicy.StoreAction.PromoteStoreAdmin),
                new StoreManagerPolicy(store3,StoreManagerPolicy.StoreAction.ManageProducts),
                new StoreManagerPolicy(store3,StoreManagerPolicy.StoreAction.ViewPurchaseHistory)
            };
            AddMoreThenOneStorePolicyTest(expected1, expected2, expected3, givenPermissions1, givenPermissions2,
                givenPermissions3);
        }

        [TestMethod]
        public void AddStoreOwnerForDifferenetStoresTest()
        {
            StoreManagerPolicy.StoreAction[] givenPermissions1 =
            {
                StoreManagerPolicy.StoreAction.ViewPurchaseHistory,
                StoreManagerPolicy.StoreAction.StoreOwner

            };
            StoreManagerPolicy.StoreAction[] givenPermissions2 =
            {
                StoreManagerPolicy.StoreAction.ManageProducts,
                StoreManagerPolicy.StoreAction.PromoteStoreAdmin,
                StoreManagerPolicy.StoreAction.StoreOwner

            };
            StoreManagerPolicy.StoreAction[] givenPermissions3 =
            {
                StoreManagerPolicy.StoreAction.PromoteStoreAdmin,
                StoreManagerPolicy.StoreAction.ViewPurchaseHistory,
                StoreManagerPolicy.StoreAction.ManageProducts
            };
            StoreManagerPolicy[] expected1 =
            {
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.StoreOwner)
            };
            StoreManagerPolicy[] expected2 =
            {
                new StoreManagerPolicy(store2,StoreManagerPolicy.StoreAction.StoreOwner)
            };
            StoreManagerPolicy[] expected3 =
            {
                new StoreManagerPolicy(store3,StoreManagerPolicy.StoreAction.PromoteStoreAdmin),
                new StoreManagerPolicy(store3,StoreManagerPolicy.StoreAction.ManageProducts),
                new StoreManagerPolicy(store3,StoreManagerPolicy.StoreAction.ViewPurchaseHistory)
            };
            AddMoreThenOneStorePolicyTest(expected1, expected2, expected3, givenPermissions1, givenPermissions2,
                givenPermissions3);
        }

        [TestMethod]
        public void AddMorePoliciesLaterTest()
        {
            StoreManagerPolicy.StoreAction[] givenPermissions1 =
            {
                StoreManagerPolicy.StoreAction.ViewPurchaseHistory

            };
            StoreManagerPolicy.StoreAction[] givenPermissions2 =
            {
                StoreManagerPolicy.StoreAction.ManageProducts,
                StoreManagerPolicy.StoreAction.PromoteStoreAdmin

            };
            StoreManagerPolicy[] expected1 =
            {
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.ViewPurchaseHistory)
            };
            StoreManagerPolicy[] expected2 =
            {
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.PromoteStoreAdmin),
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.ManageProducts)
            };
            AddMorePoliciesLater(expected1, expected2, givenPermissions1, givenPermissions2);

        }

        [TestMethod]
        public void AddMorePoliciesLaterToStoreOwnerTest()
        {
            StoreManagerPolicy.StoreAction[] givenPermissions1 =
            {
                StoreManagerPolicy.StoreAction.StoreOwner,
                StoreManagerPolicy.StoreAction.ViewPurchaseHistory

            };
            StoreManagerPolicy.StoreAction[] givenPermissions2 =
            {
                StoreManagerPolicy.StoreAction.ManageProducts,
                StoreManagerPolicy.StoreAction.PromoteStoreAdmin

            };
            StoreManagerPolicy[] expected1 =
            {
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.StoreOwner)
            };
            StoreManagerPolicy[] expected2 =
            {
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.PromoteStoreAdmin),
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.ManageProducts)
            };
            AddMorePoliciesLater(expected1, expected2, givenPermissions1, givenPermissions2);
        }
        [TestMethod]
        public void AddStoreOwnerLaterTest()
        {
            StoreManagerPolicy.StoreAction[] givenPermissions1 =
            {
                StoreManagerPolicy.StoreAction.ManageProducts,
                StoreManagerPolicy.StoreAction.PromoteStoreAdmin

            };
            StoreManagerPolicy.StoreAction[] givenPermissions2 =
            {
                StoreManagerPolicy.StoreAction.StoreOwner,
                StoreManagerPolicy.StoreAction.ViewPurchaseHistory

            };
            StoreManagerPolicy[] expected1 =
            {
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.PromoteStoreAdmin),
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.ManageProducts)
            };
            StoreManagerPolicy[] expected2 =
            {
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.StoreOwner)
            };
            AddMorePoliciesLater(expected1, expected2, givenPermissions1, givenPermissions2);
        }

        [TestMethod]
        public void RemoveAllPoliciesTest()
        {
            StoreManagerPolicy.StoreAction[] givenPermissions1 =
            {
                StoreManagerPolicy.StoreAction.ManageProducts,
                StoreManagerPolicy.StoreAction.PromoteStoreAdmin,
                StoreManagerPolicy.StoreAction.ViewPurchaseHistory,
                StoreManagerPolicy.StoreAction.DeclarePurchasePolicy

            };
            StoreManagerPolicy.StoreAction[] givenPermissions2 = new StoreManagerPolicy.StoreAction[0];
            StoreManagerPolicy[] expected1 =
            {
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.PromoteStoreAdmin),
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.ManageProducts),
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.DeclarePurchasePolicy),
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.ViewPurchaseHistory)
            };
            StoreManagerPolicy[] expected2 = new StoreManagerPolicy[0];
            AddMorePoliciesLater(expected1, expected2, givenPermissions1, givenPermissions2);
        }

        [TestMethod]
        public void AddStoreOwnerFromDifferentStoreLaterTest()
        {
            StoreManagerPolicy.StoreAction[] givenPermissions1 =
            {
                StoreManagerPolicy.StoreAction.DeclarePurchasePolicy,

            };
            StoreManagerPolicy.StoreAction[] givenPermissions2 =
            {
                StoreManagerPolicy.StoreAction.ManageProducts,
                StoreManagerPolicy.StoreAction.PromoteStoreAdmin,

            };
            StoreManagerPolicy.StoreAction[] givenPermissions3 =
            {
                StoreManagerPolicy.StoreAction.PromoteStoreAdmin,
                StoreManagerPolicy.StoreAction.ViewPurchaseHistory,
                StoreManagerPolicy.StoreAction.StoreOwner
            };
            StoreManagerPolicy[] expected1 =
            {
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.DeclarePurchasePolicy)
            };
            StoreManagerPolicy[] expected2 =
            {
                new StoreManagerPolicy(store2,StoreManagerPolicy.StoreAction.PromoteStoreAdmin),
                new StoreManagerPolicy(store2,StoreManagerPolicy.StoreAction.ManageProducts)

            };
            StoreManagerPolicy[] expected3 =
            {
                new StoreManagerPolicy(store3,StoreManagerPolicy.StoreAction.StoreOwner),

            };
            AddMorePoliciesFromMoreThenOneStoreLater(expected1, expected2,expected3, givenPermissions1, givenPermissions2,givenPermissions3);
        }


        [TestMethod]
        public void AddStoreOwnerFromDifferentStoreToStoreOwnerLaterTest()
        {
            StoreManagerPolicy.StoreAction[] givenPermissions1 =
            {
                StoreManagerPolicy.StoreAction.DeclarePurchasePolicy,
                StoreManagerPolicy.StoreAction.ManageProducts,
                StoreManagerPolicy.StoreAction.StoreOwner,

            };
            StoreManagerPolicy.StoreAction[] givenPermissions2 =
            {
                StoreManagerPolicy.StoreAction.ManageProducts,
                StoreManagerPolicy.StoreAction.StoreOwner,

            };
            StoreManagerPolicy.StoreAction[] givenPermissions3 =
            {
                StoreManagerPolicy.StoreAction.PromoteStoreAdmin,
                StoreManagerPolicy.StoreAction.ViewPurchaseHistory,
                StoreManagerPolicy.StoreAction.StoreOwner
            };
            StoreManagerPolicy[] expected1 =
            {
                new StoreManagerPolicy(store1,StoreManagerPolicy.StoreAction.StoreOwner),
            };
            StoreManagerPolicy[] expected2 =
            {
                new StoreManagerPolicy(store2,StoreManagerPolicy.StoreAction.StoreOwner),

            };
            StoreManagerPolicy[] expected3 =
            {
                new StoreManagerPolicy(store3,StoreManagerPolicy.StoreAction.StoreOwner),

            };
            AddMorePoliciesFromMoreThenOneStoreLater(expected1, expected2, expected3, givenPermissions1, givenPermissions2, givenPermissions3);
        }

        [TestMethod]
        public void AddPromotionForNonExistUserTest1()
        {
            try
            {
                UserPolicyService.PromoteStorePolicies("asdasdasdasdssafggfdsg", store1,
                    new[] {StoreManagerPolicy.StoreAction.StoreOwner});
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)PromoteStoreStatus.NoUserFound,e.Status);
            }
        }

        [TestMethod]
        public void AddPromotionForNonExistUserTest2()
        {
            try
            {
                UserPolicyService.PromoteStorePolicies("asdasdasdasdssafggfdsg", store2,
                    new[] { StoreManagerPolicy.StoreAction.StoreOwner,
                        StoreManagerPolicy.StoreAction.DeclareDiscountPolicy, StoreManagerPolicy.StoreAction.ManageProducts });
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)PromoteStoreStatus.NoUserFound, e.Status);
            }
        }

        [TestMethod]
        public void AddInvalidPromotionTest1()
        {
            try
            {
                UserPolicyService.PromoteStorePolicies(promoteName, store1,
                    new[] { StoreManagerPolicy.GetActionFromString("All your bases belong to us") });
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)PromoteStoreStatus.InvalidPromotion, e.Status);
                Assert.AreEqual(0, userServiceSignUpSession.MarketUser.GetStoreManagerPolicies(store1).Length);
                DoSignIn(promoteName, promotePassword);
                Assert.AreEqual(0, userServiceSignInSession.MarketUser.GetStoreManagerPolicies(store1).Length);
            }
        }

        [TestMethod]
        public void AddInvalidPromotionTest2()
        {
            try
            {
                UserPolicyService.PromoteStorePolicies(promoteName, store2,
                    new[] { StoreManagerPolicy.StoreAction.StoreOwner, StoreManagerPolicy.StoreAction.ManageProducts,
                        StoreManagerPolicy.GetActionFromString("THE CAKE IS A LIE") });
                Assert.Fail();
            }
            catch (MarketException e)
            {
                Assert.AreEqual((int)PromoteStoreStatus.InvalidPromotion, e.Status);
                Assert.AreEqual(0, userServiceSignUpSession.MarketUser.GetStoreManagerPolicies(store1).Length);
                DoSignIn(promoteName, promotePassword);
                Assert.AreEqual(0,userServiceSignInSession.MarketUser.GetStoreManagerPolicies(store2).Length);
            }
        }


        [TestCleanup]
        public void UserTestCleanUp()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }

        private void AddOnePolicy(StoreManagerPolicy.StoreAction action)
        {
            StoreManagerPolicy[] expected =
            {
                new StoreManagerPolicy(store1,action),
            };
            Assert.AreEqual(0, userServiceSignUpSession.MarketUser.GetStoreManagerPolicies(store1).Length);
            UserPolicyService.PromoteStorePolicies(promoteName, store1, new[] { action });
            DoSignIn(promoteName, promotePassword);
            StoreManagerPolicy[] actions = userServiceSignInSession.MarketUser.GetStoreManagerPolicies(store1);
            Assert.AreEqual(expected.Length, actions.Length);
            Assert.IsTrue(expected.SequenceEqual(actions));
        }

        private void AddMoreThenOnePolicy(StoreManagerPolicy[] expected, StoreManagerPolicy.StoreAction[] givenPermissions)
        {
            Assert.AreEqual(0, userServiceSignUpSession.MarketUser.GetStoreManagerPolicies(store1).Length);
            UserPolicyService.PromoteStorePolicies(promoteName, store1, givenPermissions);
            DoSignIn(promoteName, promotePassword);
            StoreManagerPolicy[] actions = userServiceSignInSession.MarketUser.GetStoreManagerPolicies(store1);
            Assert.AreEqual(expected.Length, actions.Length);
            Assert.IsTrue(expected.SequenceEqual(actions));
        }

        private void AddMoreThenOneStorePolicyTest(StoreManagerPolicy[] expected1, StoreManagerPolicy[] expected2, StoreManagerPolicy[] expected3,
            StoreManagerPolicy.StoreAction[] givenPermissions1, StoreManagerPolicy.StoreAction[] givenPermissions2,
            StoreManagerPolicy.StoreAction[] givenPermissions3)
        {
            Assert.AreEqual(0, userServiceSignUpSession.MarketUser.GetStoreManagerPolicies(store1).Length);
            Assert.AreEqual(0, userServiceSignUpSession.MarketUser.GetStoreManagerPolicies(store2).Length);
            Assert.AreEqual(0, userServiceSignUpSession.MarketUser.GetStoreManagerPolicies(store3).Length);
            UserPolicyService.PromoteStorePolicies(promoteName, store1, givenPermissions1);
            UserPolicyService.PromoteStorePolicies(promoteName, store2, givenPermissions2);
            UserPolicyService.PromoteStorePolicies(promoteName, store3, givenPermissions3);
            DoSignIn(promoteName, promotePassword);
            StoreManagerPolicy[] actions1 = userServiceSignInSession.MarketUser.GetStoreManagerPolicies(store1);
            StoreManagerPolicy[] actions2 = userServiceSignInSession.MarketUser.GetStoreManagerPolicies(store2);
            StoreManagerPolicy[] actions3 = userServiceSignInSession.MarketUser.GetStoreManagerPolicies(store3);
            Assert.AreEqual(expected1.Length, actions1.Length);
            Assert.IsTrue(expected1.SequenceEqual(actions1));
            Assert.AreEqual(expected2.Length, actions2.Length);
            Assert.IsTrue(expected2.SequenceEqual(actions2));
            Assert.AreEqual(expected3.Length, actions3.Length);
            Assert.IsTrue(expected3.SequenceEqual(actions3));
        }

        private void AddMorePoliciesLater(StoreManagerPolicy[] expected1, StoreManagerPolicy[] expected2,
            StoreManagerPolicy.StoreAction[] givenPermissions1, StoreManagerPolicy.StoreAction[] givenPermissions2)
        {
            Assert.AreEqual(0, userServiceSignUpSession.MarketUser.GetStoreManagerPolicies(store1).Length);
            UserPolicyService.PromoteStorePolicies(promoteName, store1, givenPermissions1);
            DoSignIn(promoteName, promotePassword);
            StoreManagerPolicy[] actions1 = userServiceSignInSession.MarketUser.GetStoreManagerPolicies(store1);
            Assert.AreEqual(expected1.Length, actions1.Length);
            Assert.IsTrue(expected1.SequenceEqual(actions1));
            UserPolicyService.PromoteStorePolicies(promoteName, store1, givenPermissions2);
            DoSignIn2(promoteName, promotePassword);
            StoreManagerPolicy[] actions2 = userServiceSignInSession2.MarketUser.GetStoreManagerPolicies(store1);
            Assert.AreEqual(expected2.Length, actions2.Length);
            Assert.IsTrue(expected2.SequenceEqual(actions2));
        }

        private void AddMorePoliciesFromMoreThenOneStoreLater(StoreManagerPolicy[] expected1,StoreManagerPolicy[] expected2,
            StoreManagerPolicy[] expected3,StoreManagerPolicy.StoreAction[] givenPermissions1,StoreManagerPolicy.StoreAction[] givenPermissions2,
            StoreManagerPolicy.StoreAction[] givenPermissions3)
        {
            Assert.AreEqual(0, userServiceSignUpSession.MarketUser.GetStoreManagerPolicies(store1).Length);
            Assert.AreEqual(0, userServiceSignUpSession.MarketUser.GetStoreManagerPolicies(store2).Length);
            Assert.AreEqual(0, userServiceSignUpSession.MarketUser.GetStoreManagerPolicies(store3).Length);
            UserPolicyService.PromoteStorePolicies(promoteName, store1, givenPermissions1);
            UserPolicyService.PromoteStorePolicies(promoteName, store2, givenPermissions2);
            DoSignIn(promoteName, promotePassword);
            StoreManagerPolicy[] actions1 = userServiceSignInSession.MarketUser.GetStoreManagerPolicies(store1);
            StoreManagerPolicy[] actions2 = userServiceSignInSession.MarketUser.GetStoreManagerPolicies(store2);
            StoreManagerPolicy[] actions3 = userServiceSignInSession.MarketUser.GetStoreManagerPolicies(store3);
            Assert.AreEqual(expected1.Length, actions1.Length);
            Assert.IsTrue(expected1.SequenceEqual(actions1));
            Assert.AreEqual(expected2.Length, actions2.Length);
            Assert.IsTrue(expected2.SequenceEqual(actions2));
            Assert.AreEqual(0, actions3.Length);
            UserPolicyService.PromoteStorePolicies(promoteName, store3, givenPermissions3);
            DoSignIn2(promoteName, promotePassword);
            actions1 = userServiceSignInSession2.MarketUser.GetStoreManagerPolicies(store1);
            actions2 = userServiceSignInSession2.MarketUser.GetStoreManagerPolicies(store2);
            actions3 = userServiceSignInSession2.MarketUser.GetStoreManagerPolicies(store3);
            Assert.AreEqual(expected1.Length, actions1.Length);
            Assert.IsTrue(expected1.SequenceEqual(actions1));
            Assert.AreEqual(expected2.Length, actions2.Length);
            Assert.IsTrue(expected2.SequenceEqual(actions2));
            Assert.AreEqual(expected3.Length, actions3.Length);
            Assert.IsTrue(expected3.SequenceEqual(actions3));
        }

        private void DoSignIn(string name, string password)
        {
            userServiceSignInSession = (UserService)marketSession.GetUserService();
            userServiceSignInSession.EnterSystem();
            Assert.AreEqual((int)SignInStatus.Success, userServiceSignInSession.SignIn(name, password).Status);
        }

        private void DoSignIn2(string name, string password)
        {
            userServiceSignInSession2 = (UserService)marketSession.GetUserService();
            userServiceSignInSession2.EnterSystem();
            Assert.AreEqual((int)SignInStatus.Success, userServiceSignInSession2.SignIn(name, password).Status);
        }
        private void DoSignUp(string name, string address, string password,string creditCard)
        {
            userServiceSignUpSession = (UserService)marketSession.GetUserService();
            userServiceSignUpSession.EnterSystem();
            Assert.AreEqual((int)SignUpStatus.Success, userServiceSignUpSession.SignUp(name, address, password, creditCard).Status);

        }
    }
}
