using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketFeed;
using SadnaSrc.MarketHarmony;
using SadnaSrc.OrderPool;
using SadnaSrc.PolicyComponent;
using SadnaSrc.SupplyPoint;
using SadnaSrc.Walleter;

namespace PurchasePolicyTests
{
    [TestClass]
    public class PolicyCheckingTest
    {

        private PolicyHandler handler;

        [TestInitialize]
        public void MarketBuilder()
        {
            handler = PolicyHandler.Instance;
        }

        [TestMethod]
        public void TestNoPolicySuccess()
        {
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestCheckPolicySuccess1()
        {
            handler.CreateCategorySimplePolicy("Food", ConditionType.AddressEqual, "Grove Street");
            handler.AddPolicy(0);
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestCheckPolicySuccess2()
        {
            handler.CreateProductSimplePolicy("#9 Large", ConditionType.UsernameEqual, "Big Smoke");
            handler.AddPolicy(0);
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestCheckPolicySuccess3()
        {
            handler.CreateStoreSimplePolicy("Cluckin Bell", ConditionType.PriceGreater, "7.0");
            handler.AddPolicy(0);
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestCheckPolicySuccess4()
        {
            handler.CreateStockItemSimplePolicy("Cluckin Bell", "#9 Large", ConditionType.PriceLesser, "21.0");
            handler.AddPolicy(0);
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestCheckPolicySuccess5()
        {
            handler.CreateGlobalSimplePolicy(ConditionType.QuantityLesser, "5");
            handler.AddPolicy(0);
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestCheckPolicyFail1()
        {
            handler.CreateCategorySimplePolicy("Food", ConditionType.AddressEqual, "Grove Street");
            handler.AddPolicy(0);
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Vinewood", 2, 14.00));
        }

        [TestMethod]
        public void TestCheckPolicyFail2()
        {
            handler.CreateProductSimplePolicy("#9 Large", ConditionType.UsernameEqual, "Big Smoke");
            handler.AddPolicy(0);
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Ryder", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestCheckPolicyFail3()
        {
            handler.CreateStoreSimplePolicy("Cluckin Bell", ConditionType.PriceGreater, "7.0");
            handler.AddPolicy(0);
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 5.00));
        }

        [TestMethod]
        public void TestCheckPolicyFail4()
        {
            handler.CreateStockItemSimplePolicy("Cluckin Bell", "#9 Large", ConditionType.PriceLesser, "21.0");
            handler.AddPolicy(0);
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 25.00));
        }

        [TestMethod]
        public void TestCheckPolicyFail5()
        {
            handler.CreateGlobalSimplePolicy(ConditionType.QuantityLesser, "5");
            handler.AddPolicy(0);
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 6, 14.00));
        }

        [TestMethod]
        public void TestAndPolicySuccess()
        {
            InitComplexPolicy(OperatorType.AND);
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestAndPolicyFail1()
        {
            InitComplexPolicy(OperatorType.AND);
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 1, 14.00));
        }

        [TestMethod]
        public void TestAndPolicyFail2()
        {
            InitComplexPolicy(OperatorType.AND);
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "CJ", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestAndPolicyFail3()
        {
            InitComplexPolicy(OperatorType.AND);
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "CJ", "Grove Street", 5, 14.00));
        }

        [TestMethod]
        public void TestOrPolicySuccess1()
        {
            InitComplexPolicy(OperatorType.OR);
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestOrPolicySuccess2()
        {
            InitComplexPolicy(OperatorType.OR);
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 3, 14.00));
        }

        [TestMethod]
        public void TestOrPolicySuccess3()
        {
            InitComplexPolicy(OperatorType.OR);
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "CJ", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestOrPolicyFail()
        {
            InitComplexPolicy(OperatorType.OR);
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "CJ", "Grove Street", 1, 14.00));
        }

        [TestMethod]
        public void TestNotPolicySuccess()
        {
            InitComplexPolicy(OperatorType.NOT);
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "CJ", "Grove Street", 1, 14.00));
        }

        [TestMethod]
        public void TestNotPolicyFail()
        {
            InitComplexPolicy(OperatorType.NOT);
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "CJ", "Grove Street", 5, 14.00));
        }

        [TestMethod]
        public void TestSeveralPoliciesSuccess1()
        {
            InitTwoPolicies();
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestSeveralPoliciesSuccess2()
        {
            InitTwoPolicies();
            Assert.IsTrue(handler.CheckRelevantPolicies("#9", "Cluckin Bell", "Food", "CJ", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestSeveralPoliciesSuccess3()
        {
            InitTwoPolicies();
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Taco Bell", "Food", "Big Smoke", "Grove Street", 2, 5.00));
        }

        [TestMethod]
        public void TestSeveralPoliciesFail1()
        {
            InitTwoPolicies();
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 5.00));
        }

        [TestMethod]
        public void TestSeveralPoliciesFail2()
        {
            InitTwoPolicies();
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "CJ", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestSeveralPoliciesFail3()
        {
            InitTwoPolicies();
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "CJ", "Grove Street", 2, 5.00));
        }

        [TestMethod]
        public void TestAllPoliciesSuccess()
        {
            InitSimplePolicies();
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestAllPoliciesFail()
        {
            InitSimplePolicies();
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 4, 25.00));
        }

        [TestCleanup]
        public void UserOrderTestCleanUp()
        {
            handler.CleanSession();
        }

        private void CompareArrays(string[] arr1, string[] arr2)
        {
            Assert.AreEqual(arr1.Length, arr2.Length);
            for (int i = 0; i < arr1.Length; i++)
            {
                Assert.AreEqual(arr1[i],arr2[i]);
            }
        }

        private void InitSimplePolicies()
        {
            handler.CreateCategorySimplePolicy("Food", ConditionType.QuantityGreater, "2");
            handler.AddPolicy(0);
            handler.CreateProductSimplePolicy("#9 Large", ConditionType.UsernameEqual, "Big Smoke");
            handler.AddPolicy(0);
            handler.CreateStoreSimplePolicy("Cluckin Bell", ConditionType.PriceGreater, "7.0");
            handler.AddPolicy(0);
            handler.CreateStockItemSimplePolicy("Cluckin Bell", "#9 Large", ConditionType.PriceLesser, "21.0");
            handler.AddPolicy(0);
            handler.CreateGlobalSimplePolicy(ConditionType.QuantityLesser, "5");
            handler.AddPolicy(0);
        }


        private void InitComplexPolicy(OperatorType type)
        {
            handler.CreateCategorySimplePolicy("Food", ConditionType.QuantityGreater, "2");
            handler.CreateCategorySimplePolicy("Food", ConditionType.UsernameEqual, "Big Smoke");
            handler.CreateCategoryPolicy("Food", type, 0, 1);
            handler.AddPolicy(2);
        }

        private void InitTwoPolicies()
        {
            handler.CreateProductSimplePolicy("#9 Large", ConditionType.UsernameEqual, "Big Smoke");
            handler.AddPolicy(0);
            handler.CreateStoreSimplePolicy("Cluckin Bell", ConditionType.PriceGreater, "7.0");
            handler.AddPolicy(0);
        }
    }
}
