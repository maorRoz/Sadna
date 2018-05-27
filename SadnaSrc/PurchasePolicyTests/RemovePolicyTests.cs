using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.PolicyComponent;

namespace PurchasePolicyTests
{
    [TestClass]
    public class RemovePolicyTests
    {
        private IPolicyDL datalayer;
        private AndOperator ROOT_AndHas2opSuns;
        private OrOperator OrHas2opSuns;
        private NotOperator NotHasopSun;
        private NotOperator NotHasCondSun;
        private OrOperator OrHas1opSun;
        private OrOperator OrHas0opSun;
        private AndOperator AndHas1opSun;
        private AndOperator AndHas0opSun;
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
            AndHas0opSun = new AndOperator(PolicyType.Global, "bla", PriceGreaterThanLeaf, PriceLessThanLeaf, 8);
            AndHas1opSun = new AndOperator(PolicyType.Global, "bla", AndHas0opSun, UsernameEqualsLeaf2, 9);
            OrHas0opSun = new OrOperator(PolicyType.Global, "bla", QuantityLessThanLeaf, QuantityGreaterThanLeaf, 10);
            OrHas1opSun = new OrOperator(PolicyType.Global, "bla", OrHas0opSun, UsernameEqualsLeaf, 11);
            NotHasCondSun = new NotOperator(PolicyType.Global, "bla", AddressEqualsLeaf, null, 12);
            NotHasopSun = new NotOperator(PolicyType.Global, "bla", NotHasCondSun, null, 13);
            OrHas2opSuns = new OrOperator(PolicyType.Global, "bla", AndHas1opSun, OrHas1opSun, 14);
            ROOT_AndHas2opSuns = new AndOperator(PolicyType.Global, "bla", OrHas2opSuns, NotHasopSun, 15);
        }
        [TestMethod]
        public void RemoveUsernameEqualsLeaf2()
        {
            UsernameEqualsLeaf2.IsRoot = true;
            datalayer.SavePolicy(UsernameEqualsLeaf2);
            datalayer.RemovePolicy(UsernameEqualsLeaf2);
            Assert.IsNull(datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void RemoveUsernameEqualsLeaf()
        {
            UsernameEqualsLeaf.IsRoot = true;
            datalayer.SavePolicy(UsernameEqualsLeaf);
            datalayer.RemovePolicy(UsernameEqualsLeaf);
            Assert.IsNull(datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void RemoveQuantityLessThanLeaf()
        {
            QuantityLessThanLeaf.IsRoot = true;
            datalayer.SavePolicy(QuantityLessThanLeaf);
            datalayer.RemovePolicy(QuantityLessThanLeaf);
            Assert.IsNull(datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void RemoveQuantityGreaterThanLeaf()
        {
            QuantityGreaterThanLeaf.IsRoot = true;
            datalayer.SavePolicy(QuantityGreaterThanLeaf);
            datalayer.RemovePolicy(QuantityGreaterThanLeaf);
            Assert.IsNull(datalayer.GetPolicy(PolicyType.Global, "bla"));
        }

        [TestMethod]
        public void RemovePriceLessThanLeaf()
        {
            PriceLessThanLeaf.IsRoot = true;
            datalayer.SavePolicy(PriceLessThanLeaf);
            datalayer.RemovePolicy(PriceLessThanLeaf);
            Assert.IsNull(datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void RemovePriceGreaterThanLeaf()
        {
            PriceGreaterThanLeaf.IsRoot = true;
            datalayer.SavePolicy(PriceGreaterThanLeaf);
            datalayer.RemovePolicy(PriceGreaterThanLeaf);
            Assert.IsNull(datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void RemoveAddressEqualsLeaf()
        {
            AddressEqualsLeaf.IsRoot = true;
            datalayer.SavePolicy(AddressEqualsLeaf);
            datalayer.RemovePolicy(AddressEqualsLeaf);
            Assert.IsNull(datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void RemoveAndHas0opSun()
        {
            AndHas0opSun.IsRoot = true;
            datalayer.SavePolicy(AndHas0opSun);
            datalayer.RemovePolicy(AndHas0opSun);
            Assert.IsNull(datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void RemoveAndHas1opSun()
        {
            AndHas1opSun.IsRoot = true;
            datalayer.SavePolicy(AndHas1opSun);
            datalayer.RemovePolicy(AndHas1opSun);
            Assert.IsNull(datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void RemoveOrHas0opSun()
        {
            OrHas0opSun.IsRoot = true;
            datalayer.SavePolicy(OrHas0opSun);
            datalayer.RemovePolicy(OrHas0opSun);
            Assert.IsNull(datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void RemoveOrHas1opSun()
        {
            OrHas1opSun.IsRoot = true;
            datalayer.SavePolicy(OrHas1opSun);
            datalayer.RemovePolicy(OrHas1opSun);
            Assert.IsNull(datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void RemoveNotHasCondSun()
        {
            NotHasCondSun.IsRoot = true;
            datalayer.SavePolicy(NotHasCondSun);
            datalayer.RemovePolicy(NotHasCondSun);
            Assert.IsNull(datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void RemoveNotHasopSun()
        {
            NotHasopSun.IsRoot = true;
            datalayer.SavePolicy(NotHasopSun);
            datalayer.RemovePolicy(NotHasopSun);
            Assert.IsNull(datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void RemoveOrHas2opSuns()
        {
            OrHas2opSuns.IsRoot = true;
            datalayer.SavePolicy(OrHas2opSuns);
            datalayer.RemovePolicy(OrHas2opSuns);
            Assert.IsNull(datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void RemoveROOT_AndHas2opSuns()
        {
            ROOT_AndHas2opSuns.IsRoot = true;
            datalayer.SavePolicy(ROOT_AndHas2opSuns);
            datalayer.RemovePolicy(ROOT_AndHas2opSuns);
            Assert.IsNull(datalayer.GetPolicy(PolicyType.Global, "bla"));
        }

        [TestCleanup]
        public void CleanDb()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
            PolicyHandler.Instance.CleanSession();
        }

    }
}
