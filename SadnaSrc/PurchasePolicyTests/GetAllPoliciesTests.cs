using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.PolicyComponent;

namespace PurchasePolicyTests
{
    [TestClass]
    public class GetAllPoliciesTests
    {
        private IPolicyDL datalayer;
        private AddressEquals AddressEqualsLeaf;
        private PriceGreaterThan PriceGreaterThanLeaf;
        private PriceLessThan PriceLessThanLeaf;
        private QuantityGreaterThan QuantityGreaterThanLeaf;
        private QuantityLessThan QuantityLessThanLeaf;
        private UsernameEquals UsernameEqualsLeaf;
        private UsernameEquals UsernameEqualsLeaf2;
        [TestInitialize]
        public void BuildSupplyPoint()
        {
            MarketDB.Instance.InsertByForce();
            datalayer = PolicyDL.Instance;
            UsernameEqualsLeaf2 = new UsernameEquals(PolicyType.Global, "bla", "bla_bla", 1);
            UsernameEqualsLeaf = new UsernameEquals(PolicyType.Global, "bla", "bla_bla", 2);
            QuantityLessThanLeaf = new QuantityLessThan(PolicyType.Global, "bla", "bla_bla", 3);
            QuantityGreaterThanLeaf = new QuantityGreaterThan(PolicyType.Global, "bla", "bla_bla", 4);
            PriceLessThanLeaf = new PriceLessThan(PolicyType.Global, "bla", "bla_bla", 5);
            PriceGreaterThanLeaf = new PriceGreaterThan(PolicyType.Global, "bla", "bla_bla", 6);
            AddressEqualsLeaf = new AddressEquals(PolicyType.Global, "bla", "bla_bla", 7);
        }
        [TestMethod]
        public void GetAllHasInDB()
        {
            List<PurchasePolicy> expected = new List<PurchasePolicy>();
            expected.Add(UsernameEqualsLeaf2);
            expected.Add(UsernameEqualsLeaf);
            expected.Add(QuantityLessThanLeaf);
            expected.Add(QuantityGreaterThanLeaf);
            expected.Add(PriceLessThanLeaf);
            expected.Add(PriceGreaterThanLeaf);
            expected.Add(AddressEqualsLeaf);
            UsernameEqualsLeaf2.IsRoot = true;
            UsernameEqualsLeaf.IsRoot = true;
            QuantityLessThanLeaf.IsRoot = true;
            QuantityGreaterThanLeaf.IsRoot = true;
            PriceLessThanLeaf.IsRoot = true;
            PriceGreaterThanLeaf.IsRoot = true;
            AddressEqualsLeaf.IsRoot = true;
            datalayer.SavePolicy(UsernameEqualsLeaf2);
            datalayer.SavePolicy(UsernameEqualsLeaf);
            datalayer.SavePolicy(QuantityLessThanLeaf);
            datalayer.SavePolicy(QuantityGreaterThanLeaf);
            datalayer.SavePolicy(PriceLessThanLeaf);
            datalayer.SavePolicy(PriceGreaterThanLeaf);
            datalayer.SavePolicy(AddressEqualsLeaf);
            List<PurchasePolicy> found = datalayer.GetAllPolicies();
            Assert.AreEqual(expected.Count, found.Count);
            PurchasePolicy[] foundaray = found.ToArray();
            PurchasePolicy[] expectedaray = expected.ToArray();
            for (int i=0; i<expectedaray.Length; i++)
            {
                Assert.AreEqual(expectedaray[i],foundaray[i]);
            }
        }
        [TestMethod]
        public void GetAllHasNone()
        {
            List<PurchasePolicy> expected = new List<PurchasePolicy>();
            List<PurchasePolicy> found = datalayer.GetAllPolicies();
            Assert.AreEqual(expected.Count, found.Count);
            PurchasePolicy[] foundaray = found.ToArray();
            PurchasePolicy[] expectedaray = expected.ToArray();
            for (int i = 0; i < expectedaray.Length; i++)
            {
                Assert.AreEqual(expectedaray[i], foundaray[i]);
            }
        }

        [TestCleanup]
        public void CleanDb()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();

        }

    }
}
