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
            InitSimplePolicy(PolicyType.Category, "Food", ConditionType.AddressEqual, "Grove Street");
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestCheckPolicySuccess2()
        {
            InitSimplePolicy(PolicyType.Product, "#9 Large", ConditionType.UsernameEqual, "Big Smoke");
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestCheckPolicySuccess3()
        {
            InitSimplePolicy(PolicyType.Store, "Cluckin Bell", ConditionType.PriceGreater, "7.0");
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestCheckPolicySuccess4()
        {
            InitSimplePolicy(PolicyType.StockItem, "Cluckin Bell.#9 Large", ConditionType.PriceLesser, "21.0");
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestCheckPolicySuccess5()
        {
            InitSimplePolicy(PolicyType.Global, null, ConditionType.QuantityLesser, "5");
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestCheckPolicyFail1()
        {
            InitSimplePolicy(PolicyType.Category, "Food", ConditionType.AddressEqual, "Grove Street");
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Vinewood", 2, 14.00));
        }

        [TestMethod]
        public void TestCheckPolicyFail2()
        {
            InitSimplePolicy(PolicyType.Product, "#9 Large", ConditionType.UsernameEqual, "Big Smoke");
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Ryder", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestCheckPolicyFail3()
        {
            InitSimplePolicy(PolicyType.Store, "Cluckin Bell", ConditionType.PriceGreater, "7.0");
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 5.00));
        }

        [TestMethod]
        public void TestCheckPolicyFail4()
        {
            InitSimplePolicy(PolicyType.StockItem, "Cluckin Bell.#9 Large", ConditionType.PriceLesser, "21.0");
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 25.00));
        }

        [TestMethod]
        public void TestCheckPolicyFail5()
        {
            InitSimplePolicy(PolicyType.Global, null, ConditionType.QuantityLesser, "5");
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
            InitSimplePolicy(PolicyType.Product, "#9 Large", ConditionType.UsernameEqual, "Big Smoke");
            InitSimplePolicy(PolicyType.Store, "Cluckin Bell", ConditionType.PriceGreater, "7.0");
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestSeveralPoliciesSuccess2()
        {
            InitSimplePolicy(PolicyType.Product, "#9 Large", ConditionType.UsernameEqual, "Big Smoke");
            InitSimplePolicy(PolicyType.Store, "Cluckin Bell", ConditionType.PriceGreater, "7.0");
            Assert.IsTrue(handler.CheckRelevantPolicies("#9", "Cluckin Bell", "Food", "CJ", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestSeveralPoliciesSuccess3()
        {
            InitSimplePolicy(PolicyType.Product, "#9 Large", ConditionType.UsernameEqual, "Big Smoke");
            InitSimplePolicy(PolicyType.Store, "Cluckin Bell", ConditionType.PriceGreater, "7.0");
            Assert.IsTrue(handler.CheckRelevantPolicies("#9 Large", "Taco Bell", "Food", "Big Smoke", "Grove Street", 2, 5.00));
        }

        [TestMethod]
        public void TestSeveralPoliciesFail1()
        {
            InitSimplePolicy(PolicyType.Product, "#9 Large", ConditionType.UsernameEqual, "Big Smoke");
            InitSimplePolicy(PolicyType.Store, "Cluckin Bell", ConditionType.PriceGreater, "7.0");
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "Big Smoke", "Grove Street", 2, 5.00));
        }

        [TestMethod]
        public void TestSeveralPoliciesFail2()
        {
            InitSimplePolicy(PolicyType.Product, "#9 Large", ConditionType.UsernameEqual, "Big Smoke");
            InitSimplePolicy(PolicyType.Store, "Cluckin Bell", ConditionType.PriceGreater, "7.0");
            Assert.IsFalse(handler.CheckRelevantPolicies("#9 Large", "Cluckin Bell", "Food", "CJ", "Grove Street", 2, 14.00));
        }

        [TestMethod]
        public void TestSeveralPoliciesFail3()
        {
            InitSimplePolicy(PolicyType.Product, "#9 Large", ConditionType.UsernameEqual, "Big Smoke");
            InitSimplePolicy(PolicyType.Store, "Cluckin Bell", ConditionType.PriceGreater, "7.0");
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
            InitSimplePolicy(PolicyType.Category, "Food", ConditionType.QuantityGreater, "2");
            InitSimplePolicy(PolicyType.Product, "#9 Large", ConditionType.UsernameEqual, "Big Smoke");
            InitSimplePolicy(PolicyType.Store, "Cluckin Bell", ConditionType.PriceGreater, "7.0");
            InitSimplePolicy(PolicyType.StockItem, "Cluckin Bell.#9 Large", ConditionType.PriceLesser, "21.0");
            InitSimplePolicy(PolicyType.Global, null, ConditionType.QuantityLesser, "5");
        }

        private void InitSimplePolicy(PolicyType type, string subject, ConditionType cond, string value)
        {
            handler.StartSession(type, subject);
            handler.CreateCondition(cond, value);
            handler.AddPolicy(0);
        }

        private void InitComplexPolicy(OperatorType type)
        {
            handler.StartSession(PolicyType.Category, "Food");
            handler.CreateCondition(ConditionType.QuantityGreater, "2");
            handler.CreateCondition(ConditionType.UsernameEqual, "Big Smoke");
            handler.CreatePolicy(type, 0, 1);
            handler.AddPolicy(2);
        }
    }
}
