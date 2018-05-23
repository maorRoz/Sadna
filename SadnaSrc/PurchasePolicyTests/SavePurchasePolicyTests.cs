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
    public class SavePurchasePolicyTests
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
            AndHas0opSun = new AndOperator(PolicyType.Global, "bla", PriceGreaterThanLeaf, PriceLessThanLeaf,8);
            AndHas1opSun = new AndOperator(PolicyType.Global, "bla", AndHas0opSun, UsernameEqualsLeaf2, 9);
            OrHas0opSun = new OrOperator(PolicyType.Global, "bla", QuantityLessThanLeaf, QuantityGreaterThanLeaf, 10);
            OrHas1opSun = new OrOperator(PolicyType.Global, "bla", OrHas0opSun, UsernameEqualsLeaf, 11);
            NotHasCondSun = new NotOperator(PolicyType.Global, "bla", AddressEqualsLeaf, null, 12);
            NotHasopSun = new NotOperator(PolicyType.Global, "bla", NotHasCondSun, null, 13);
            OrHas2opSuns =new OrOperator(PolicyType.Global, "bla", AndHas1opSun, OrHas1opSun, 14);
            ROOT_AndHas2opSuns = new AndOperator(PolicyType.Global, "bla", OrHas2opSuns, NotHasopSun, 15);
        }
        [TestMethod]
        public void SaveUsernameEqualsLeaf2()
        {
                UsernameEqualsLeaf2.IsRoot = true;
                datalayer.SavePolicy(UsernameEqualsLeaf2);
                Assert.AreEqual(UsernameEqualsLeaf2, datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void SaveUsernameEqualsLeaf()
        {
            UsernameEqualsLeaf.IsRoot = true;
            datalayer.SavePolicy(UsernameEqualsLeaf);
            Assert.AreEqual(UsernameEqualsLeaf, datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void SaveQuantityLessThanLeaf()
        {
            QuantityLessThanLeaf.IsRoot = true;
            datalayer.SavePolicy(QuantityLessThanLeaf);
            Assert.AreEqual(QuantityLessThanLeaf, datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void SaveQuantityGreaterThanLeaf()
        {
            QuantityGreaterThanLeaf.IsRoot = true;
            datalayer.SavePolicy(QuantityGreaterThanLeaf);
            Assert.AreEqual(QuantityGreaterThanLeaf, datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        
        [TestMethod]
        public void SavePriceLessThanLeaf()
        {
            PriceLessThanLeaf.IsRoot = true;
            datalayer.SavePolicy(PriceLessThanLeaf);
            Assert.AreEqual(PriceLessThanLeaf, datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void SavePriceGreaterThanLeaf()
        {
            PriceGreaterThanLeaf.IsRoot = true;
            datalayer.SavePolicy(PriceGreaterThanLeaf);
            Assert.AreEqual(PriceGreaterThanLeaf, datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void SaveAddressEqualsLeaf()
        {
            AddressEqualsLeaf.IsRoot = true;
            datalayer.SavePolicy(AddressEqualsLeaf);
            Assert.AreEqual(AddressEqualsLeaf, datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void SaveAndHas0opSun()
        {
            AndHas0opSun.IsRoot = true;
            datalayer.SavePolicy(AndHas0opSun);
            Assert.AreEqual(AndHas0opSun, datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void SaveAndHas1opSun()
        {
            AndHas1opSun.IsRoot = true;
            datalayer.SavePolicy(AndHas1opSun);
            Assert.AreEqual(AndHas1opSun, datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void SaveOrHas0opSun()
        {
            OrHas0opSun.IsRoot = true;
            datalayer.SavePolicy(OrHas0opSun);
            Assert.AreEqual(OrHas0opSun, datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void SaveOrHas1opSun()
        {
            OrHas1opSun.IsRoot = true;
            datalayer.SavePolicy(OrHas1opSun);
            Assert.AreEqual(OrHas1opSun, datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void SaveNotHasCondSun()
        {
            NotHasCondSun.IsRoot = true;
            datalayer.SavePolicy(NotHasCondSun);
            Assert.AreEqual(NotHasCondSun, datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void SaveNotHasopSun()
        {
            NotHasopSun.IsRoot = true;
            datalayer.SavePolicy(NotHasopSun);
            Assert.AreEqual(NotHasopSun, datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void SaveOrHas2opSuns()
        {
            OrHas2opSuns.IsRoot = true;
            datalayer.SavePolicy(OrHas2opSuns);
            Assert.AreEqual(OrHas2opSuns, datalayer.GetPolicy(PolicyType.Global, "bla"));
        }
        [TestMethod]
        public void SaveROOT_AndHas2opSuns()
        {
            ROOT_AndHas2opSuns.IsRoot = true;
            datalayer.SavePolicy(ROOT_AndHas2opSuns);
            Assert.AreEqual(ROOT_AndHas2opSuns, datalayer.GetPolicy(PolicyType.Global, "bla"));
        }

        [TestCleanup]
        public void CleanDb()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();

        }

        }
}
