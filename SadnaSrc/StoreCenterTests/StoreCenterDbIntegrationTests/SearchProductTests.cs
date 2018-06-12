using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;

namespace StoreCenterTests.StoreCenterDbIntegrationTests
{

    [TestClass]
    public class SearchProductTests
    {
        private MarketYard market;
        private IStoreShoppingService storeService;
        IUserService userService;
        private string p1;
        private string p2;

        [TestInitialize]
        public void BuildInitialize()
        {
            MarketDB.Instance.InsertByForce();
            market = MarketYard.Instance;
            userService = market.GetUserService();
            userService.EnterSystem();
            storeService = market.GetStoreShoppingService(ref userService);
            p1 = " name: BOX base price: 100 description: this is a plastic box Discount: {type is: hidden} Purchase Way: Immediate Quantity: 5 Store: X";
            p2 = " name: Fraid Egg base price: 10 description: yami Discount: {none} Purchase Way: Immediate Quantity: 10 Store: T";
        }

        [TestMethod]
        public void SearchByNameNoFilteringSuccessTest()
        {
            ProductFound(p1, storeService.SearchProduct("BOX", 0, 0, "None"));
        }

        [TestMethod]
        public void SearchByNameSimilarResultTest()
        {
            Assert.AreEqual((int)SearchProductStatus.MistakeTipGiven, 
                storeService.SearchProduct("BUX", 0, 0, "None").Status);
        }

        [TestMethod]
        public void SearchByNameNotExistTest()
        {
            NoneFound(storeService.SearchProduct("aerhaer", 0, 0, "None"));
        }

        [TestMethod]
        public void SearchByCategoryNoFilteringSuccessTest()
        {
            ProductFound(p2, storeService.SearchProduct("WanderlandItems", 0, 0, "None"));
        }

        [TestMethod]
        public void SearchByCategorySimilarResultTest()
        {
            Assert.AreEqual((int)SearchProductStatus.MistakeTipGiven,
                storeService.SearchProduct("WanderlondItems", 0, 0, "None").Status);
        }

        [TestMethod]
        public void SearchByCategoryNotExistTest()
        {
            Assert.AreEqual((int)SearchProductStatus.CategoryNotFound,
                storeService.SearchProduct("ABOX", 0, 0, "None").Status);
        }

        [TestMethod]
        public void SearchByCategoryEmptyTest()
        {
            NoneFound(storeService.SearchProduct("Books", 0, 0, "None"));
        }

        [TestMethod]
        public void SearchByKeywordNoFilteringSuccessTest()
        {
            ProductFound(p1, storeService.SearchProduct("plastic", 0, 0, "None"));
        }

        [TestMethod]
        public void SearchByKeywordNotExistTest()
        {
            NoneFound(storeService.SearchProduct("dfdfbzdb", 0, 0, "None"));
        }

        [TestMethod]
        public void SearchByNameMinPriceFoundTest()
        {
            ProductFound(p1, storeService.SearchProduct("BOX", 10, 0, "None"));
        }

        [TestMethod]
        public void SearchByNameMinPriceNotFoundTest()
        {
            NoneFound(storeService.SearchProduct("BOX", 1000, 0, "None"));
        }

        [TestMethod]
        public void SearchByNameMaxPriceFoundTest()
        {
            ProductFound(p1, storeService.SearchProduct("BOX", 0, 1000, "None"));
        }

        [TestMethod]
        public void SearchByNameMaxPriceNotFoundTest()
        {
            NoneFound(storeService.SearchProduct("BOX", 0, 10, "None"));
        }

        [TestMethod]
        public void SearchByNamePriceRangeFoundTest()
        {
            ProductFound(p1, storeService.SearchProduct("BOX", 10, 1000, "None"));
        }

        [TestMethod]
        public void SearchByNamePriceRangeNotFoundTest()
        {
            NoneFound(storeService.SearchProduct("BOX", 1000, 50000, "None"));
        }

        [TestMethod]
        public void SearchByNameMinPriceWrongTest()
        {
            Assert.AreEqual((int)SearchProductStatus.PricesInvalid,
                storeService.SearchProduct("BOX", -1, 0, "None").Status);
        }

        [TestMethod]
        public void SearchByNameMaxPriceWrongTest()
        {
            Assert.AreEqual((int)SearchProductStatus.PricesInvalid,
                storeService.SearchProduct("BOX", 0, -1, "None").Status);
        }

        [TestMethod]
        public void SearchByNamePriceRangeWrongTest()
        {
            Assert.AreEqual((int)SearchProductStatus.PricesInvalid,
                storeService.SearchProduct("BOX", 1000, 500, "None").Status);
        }

        [TestMethod]
        public void SearchByNameCategoryFoundTest()
        {
            ProductFound(p2, storeService.SearchProduct("Fraid Egg", 0, 0, "WanderlandItems"));
        }

        [TestMethod]
        public void SearchByNameCategoryNotFoundTest()
        {
            NoneFound(storeService.SearchProduct("Fraid Egg", 0, 0, "Books"));
        }

        [TestMethod]
        public void SearchByCategoryConflictTest()
        {
            NoneFound(storeService.SearchProduct("WanderlandItems", 0, 0, "Books"));
        }

        [TestMethod]
        public void SearchByNameMultipleConstraintsTest1()
        {
            ProductFound(p2, storeService.SearchProduct("Fraid Egg", 10, 0, "WanderlandItems"));
        }

        [TestMethod]
        public void SearchByNameMultipleConstraintsTest2()
        {
            ProductFound(p2, storeService.SearchProduct("Fraid Egg", 0, 1000, "WanderlandItems"));
        }

        [TestMethod]
        public void SearchByNameMultipleConstraintsTest3()
        {
            ProductFound(p2, storeService.SearchProduct("Fraid Egg", 5, 1000, "WanderlandItems"));
        }

        [TestMethod]
        public void SearchByNameMultipleConstraintsTest4()
        {
            NoneFound(storeService.SearchProduct("Fraid Egg", 100, 1000, "WanderlandItems"));
        }

        [TestMethod]
        public void NullDataGiven()
        {
            Assert.AreEqual((int)SearchProductStatus.NullValue, storeService.SearchProduct("", 0, 0, "None").Status);
        }

        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }

        private void ProductFound(string p, MarketAnswer ans)
        {
            string[] expected = {p};
            string[] received = ans.ReportList;
            Assert.AreEqual((int)SearchProductStatus.Success, ans.Status);
            Assert.AreEqual(expected.Length, received.Length);
            for (int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], received[i]);
            }
        }

        private void NoneFound(MarketAnswer ans)
        {
            string[] received = ans.ReportList;
            Assert.AreEqual((int)SearchProductStatus.Success, ans.Status);
            Assert.AreEqual(0, received.Length);
        }
    }
}
