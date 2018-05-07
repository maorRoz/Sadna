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
    public class PolicyTest
    {

        private PolicyHandler handler;
        private PurchasePolicy policy;

        [TestInitialize]
        public void MarketBuilder()
        {
            handler = PolicyHandler.Instance;
        }

        [TestMethod]
        public void TestCreateCondition1()
        {
            string[] policyData =
                handler.CreateCondition(PolicyType.Category, "Food", ConditionType.AddressEqual, "Grove Street");
            string[] expectedData = {"1", "Category", "Food", "Address", "=", "Grove Street"};
            CompareArrays(expectedData, policyData);
        }

        [TestMethod]
        public void TestCreateCondition2()
        {
            string[] policyData =
                handler.CreateCondition(PolicyType.Store, "Cluckin' Bell", ConditionType.PriceGreater, "7");
            string[] expectedData = { "1", "Store", "Cluckin' Bell", "Price", ">=", "7" };
            CompareArrays(expectedData, policyData);
        }

        [TestMethod]
        public void TestCreateCondition3()
        {
            string[] policyData =
                handler.CreateCondition(PolicyType.Product, "#9", ConditionType.PriceLesser, "7");
            string[] expectedData = { "1", "Product", "#9", "Price", "<=", "7" };
            CompareArrays(expectedData, policyData);
        }

        [TestMethod]
        public void TestCreateCondition4()
        {
            string[] policyData =
                handler.CreateCondition(PolicyType.Category, "Food", ConditionType.QuantityGreater, "2");
            string[] expectedData = { "1", "Category", "Food", "Quantity", ">=", "2" };
            CompareArrays(expectedData, policyData);
        }

        [TestMethod]
        public void TestCreateCondition5()
        {
            string[] policyData =
                handler.CreateCondition(PolicyType.Global, null, ConditionType.QuantityLesser, "2");
            string[] expectedData = { "1", "Global", null, "Quantity", "<=", "2" };
            CompareArrays(expectedData, policyData);
        }

        [TestMethod]
        public void TestCreateCondition6()
        {
            string[] policyData =
                handler.CreateCondition(PolicyType.Category, "Food", ConditionType.UsernameEqual, "Big Smoke");
            string[] expectedData = { "1", "Category", "Food", "Username", "=", "Big Smoke" };
            CompareArrays(expectedData, policyData);
        }

        [TestMethod]
        public void TestCreateStockItemCondition()
        {
            string[] policyData =
                handler.CreateStockItemCondition("Cluckin' Bell", "#9 Large", ConditionType.UsernameEqual, "Big Smoke");
            string[] expectedData = { "1", "StockItem", "Cluckin' Bell.#9 Large", "Username", "=", "Big Smoke" };
            CompareArrays(expectedData, policyData);
        }

        private void CompareArrays(string[] arr1, string[] arr2)
        {
            Assert.AreEqual(arr1.Length, arr2.Length);
            for (int i = 0; i < arr1.Length; i++)
            {
                Assert.AreEqual(arr1[i],arr2[i]);
            }
        }
    }
}
