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
    public class PolicyManagementTest
    {

        private PolicyHandler handler;

        [TestInitialize]
        public void MarketBuilder()
        {
            handler = PolicyHandler.Instance;
        }

        [TestMethod]
        public void TestStartSession()
        {
            handler.StartSession(PolicyType.Category, "Food");
            Assert.AreEqual(PolicyType.Category, handler.Type);
            Assert.AreEqual("Food", handler.Subject);
        }

        [TestMethod]
        public void TestCreateCondition1()
        {
            handler.StartSession(PolicyType.Category, "Food");
            string[] policyData =
                handler.CreateCondition(ConditionType.AddressEqual, "Grove Street");
            string[] expectedData = {"0", "Address", "=", "Grove Street"};
            CompareArrays(expectedData, policyData);
            Assert.AreEqual(1, handler.GetSessionPolicies().Length);
        }

        [TestMethod]
        public void TestCreateCondition2()
        {
            handler.StartSession(PolicyType.Store, "Cluckin' Bell");
            string[] policyData =
                handler.CreateCondition(ConditionType.PriceGreater, "7");
            string[] expectedData = { "0", "Price", ">=", "7" };
            CompareArrays(expectedData, policyData);
            Assert.AreEqual(1, handler.GetSessionPolicies().Length);
        }

        [TestMethod]
        public void TestCreateCondition3()
        {
            handler.StartSession(PolicyType.Product, "#9");
            string[] policyData =
                handler.CreateCondition(ConditionType.PriceLesser, "7");
            string[] expectedData = { "0", "Price", "<=", "7" };
            CompareArrays(expectedData, policyData);
            Assert.AreEqual(1, handler.GetSessionPolicies().Length);
        }

        [TestMethod]
        public void TestCreateCondition4()
        {
            handler.StartSession(PolicyType.Category, "Food");
            string[] policyData =
                handler.CreateCondition(ConditionType.QuantityGreater, "2");
            string[] expectedData = { "0", "Quantity", ">=", "2" };
            CompareArrays(expectedData, policyData);
            Assert.AreEqual(1, handler.GetSessionPolicies().Length);
        }

        [TestMethod]
        public void TestCreateCondition5()
        {
            handler.StartSession(PolicyType.Global, null);
            string[] policyData =
                handler.CreateCondition(ConditionType.QuantityLesser, "2");
            string[] expectedData = { "0", "Quantity", "<=", "2" };
            CompareArrays(expectedData, policyData);
            Assert.AreEqual(1, handler.GetSessionPolicies().Length);
        }

        [TestMethod]
        public void TestCreateCondition6()
        {
            handler.StartSession(PolicyType.StockItem, "Cluckin Bell.#9 Large");
            string[] policyData =
                handler.CreateCondition(ConditionType.UsernameEqual, "Big Smoke");
            string[] expectedData = { "0", "Username", "=", "Big Smoke" };
            CompareArrays(expectedData, policyData);
            Assert.AreEqual(1, handler.GetSessionPolicies().Length);
        }

        [TestMethod]
        public void TestCreateAndOperatorPolicy()
        {
            InitPolicies();
            string[] policyData =
                handler.CreatePolicy(OperatorType.AND, 0, 1);
            string[] expectedData = { "2", "AND", "0", "1" };
            CompareArrays(expectedData, policyData);
            Assert.AreEqual(3, handler.GetSessionPolicies().Length);
        }

        [TestMethod]
        public void TestCreateOrOperatorPolicy()
        {
            InitPolicies();
            string[] policyData =
                handler.CreatePolicy(OperatorType.OR, 0, 1);
            string[] expectedData = { "2", "OR", "0", "1" };
            CompareArrays(expectedData, policyData);
            Assert.AreEqual(3, handler.GetSessionPolicies().Length);
        }

        [TestMethod]
        public void TestCreateNotOperatorPolicy()
        {
            InitPolicies();
            string[] policyData =
                handler.CreatePolicy(OperatorType.NOT, 0, 1);
            string[] expectedData = { "2", "NOT", "0"};
            CompareArrays(expectedData, policyData);
            Assert.AreEqual(3, handler.GetSessionPolicies().Length);
        }

        [TestMethod]
        public void TestAddSimplePolicy()
        {
            handler.StartSession(PolicyType.Category, "Food");
            string[] policyData =
                handler.CreateCondition(ConditionType.UsernameEqual, "Big Smoke");
            handler.AddPolicy(Int32.Parse(policyData[0]));
            string[] actualData = handler.GetPolicyData(PolicyType.Category, "Food");
            Assert.IsNotNull(actualData);
            string[] expectedData = { actualData[0], "Username", "=", "Big Smoke" };
            CompareArrays(expectedData, actualData);
            Assert.AreEqual(0, handler.GetSessionPolicies().Length);
        }

        [TestMethod]
        public void TestAddComplexPolicy()
        {
            InitPolicies();
            string[] policyData =
                handler.CreatePolicy(OperatorType.AND, 0, 1);
            handler.AddPolicy(Int32.Parse(policyData[0]));
            string[] actualData = handler.GetPolicyData(PolicyType.Category, "Food");
            Assert.IsNotNull(actualData);
            string[] expectedData = { actualData[0], "AND", "0", "1" };
            CompareArrays(expectedData, actualData);
            Assert.AreEqual(0, handler.GetSessionPolicies().Length);
        }

        [TestMethod]
        public void TestRemovePolicy()
        {
            TestAddSimplePolicy();
            handler.RemovePolicy(PolicyType.Category, "Food");
            Assert.AreEqual(0, handler.Policies.Count);
        }

        [TestMethod]
        public void TestRemoveWrongPolicy1()
        {
            TestAddSimplePolicy();
            handler.RemovePolicy(PolicyType.Store, "Food");
            Assert.AreEqual(1, handler.Policies.Count);
        }

        [TestMethod]
        public void TestRemoveWrongPolicy2()
        {
            TestAddSimplePolicy();
            handler.RemovePolicy(PolicyType.Category, "Big Smoke");
            Assert.AreEqual(1, handler.Policies.Count);
        }

        [TestMethod]
        public void TestRemoveSessionPolicy()
        {
            TestCreateCondition5();
            handler.RemoveSessionPolicy(0);
            Assert.AreEqual(0, handler.GetSessionPolicies().Length);
        }

        [TestMethod]
        public void TestRemoveSessionPolicyWrongId()
        {
            TestCreateCondition5();
            handler.RemoveSessionPolicy(3);
            Assert.AreEqual(1, handler.GetSessionPolicies().Length);
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

        private void InitPolicies()
        {
            handler.StartSession(PolicyType.Category, "Food");
            handler.CreateCondition(ConditionType.AddressEqual, "Grove Street");
            handler.CreateCondition(ConditionType.QuantityGreater, "2");
        }
    }
}
