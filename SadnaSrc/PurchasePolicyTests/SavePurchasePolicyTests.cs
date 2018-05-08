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
        private List<PurchasePolicy> allOptions;
        [TestInitialize]
        public void BuildSupplyPoint()
        {
            MarketDB.Instance.InsertByForce();
            datalayer = PolicyDL.Instance;
            UsernameEqualsLeaf2 = new UsernameEquals(PolicyType.Global, "bla", "bla_bla", 1);
            /**allOptions = new List<PurchasePolicy>();
            UsernameEqualsLeaf2 = new UsernameEquals(PolicyType.Global, "bla", "bla_bla", 1);
            allOptions.Add(UsernameEqualsLeaf2);
            UsernameEqualsLeaf = new UsernameEquals(PolicyType.Global, "bla", "bla_bla", 2);
            allOptions.Add(UsernameEqualsLeaf);
            QuantityLessThanLeaf = new QuantityLessThan(PolicyType.Global, "bla", "bla_bla", 3);
            allOptions.Add(QuantityLessThanLeaf);
            QuantityGreaterThanLeaf = new QuantityGreaterThan(PolicyType.Global, "bla", "bla_bla", 4);
            allOptions.Add(QuantityGreaterThanLeaf);
            PriceLessThanLeaf = new PriceLessThan(PolicyType.Global, "bla", "bla_bla", 5);
            allOptions.Add(PriceLessThanLeaf);
            PriceGreaterThanLeaf = new PriceGreaterThan(PolicyType.Global, "bla", "bla_bla", 6);
            allOptions.Add(PriceGreaterThanLeaf);
            AddressEqualsLeaf = new AddressEquals(PolicyType.Global, "bla", "bla_bla", 7);
            allOptions.Add(AddressEqualsLeaf);
            AndHas0opSun = new AndOperator(PolicyType.Global, "bla", PriceGreaterThanLeaf, PriceLessThanLeaf,8);
            allOptions.Add(AndHas0opSun);
            AndHas1opSun = new AndOperator(PolicyType.Global, "bla", AndHas0opSun, UsernameEqualsLeaf2, 9);
            allOptions.Add(AndHas1opSun);
            OrHas0opSun = new OrOperator(PolicyType.Global, "bla", QuantityLessThanLeaf, QuantityGreaterThanLeaf, 10);
            allOptions.Add(OrHas0opSun);
            OrHas1opSun = new OrOperator(PolicyType.Global, "bla", OrHas0opSun, UsernameEqualsLeaf, 11);
            allOptions.Add(OrHas1opSun);
            NotHasCondSun = new NotOperator(PolicyType.Global, "bla", AddressEqualsLeaf, null, 12);
            allOptions.Add(NotHasCondSun);
            NotHasopSun = new NotOperator(PolicyType.Global, "bla", NotHasCondSun, null, 13);
            allOptions.Add(NotHasopSun);
            OrHas2opSuns =new OrOperator(PolicyType.Global, "bla", AndHas1opSun, OrHas1opSun, 14);
            allOptions.Add(OrHas2opSuns);
            ROOT_AndHas2opSuns = new AndOperator(PolicyType.Global, "bla", OrHas2opSuns, NotHasopSun, 15);
            allOptions.Add(ROOT_AndHas2opSuns);
            **/
        }
        [TestMethod]
        public void SaveUsernameEqualsLeaf2()
        {
                UsernameEqualsLeaf2.IsRoot = true;
                datalayer.SavePolicy(UsernameEqualsLeaf2);
                Assert.AreEqual(UsernameEqualsLeaf2, datalayer.GetPolicy(PolicyType.Global, "bla"));
        }

        [TestCleanup]
        public void CleanDb()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();

        }

        }
}
