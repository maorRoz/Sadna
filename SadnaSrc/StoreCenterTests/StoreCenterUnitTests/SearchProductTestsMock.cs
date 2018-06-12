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
using SadnaSrc.MarketRecovery;
using SadnaSrc.StoreCenter;

namespace StoreCenterTests.StoreCenterUnitTests
{
	[TestClass]
	public class SearchProductTestsMock
	{
		private Mock<IStoreDL> _handler;
		private Mock<IUserShopper> _userShopper;
		private Mock<IMarketBackUpDB> _marketDbMocker;
		private SearchProductSlave _slave;
	    private string p1;
	    private string p2;

        [TestInitialize]
		public void BuildStore()
		{
			_marketDbMocker = new Mock<IMarketBackUpDB>();
			MarketException.SetDB(_marketDbMocker.Object);
			MarketLog.SetDB(_marketDbMocker.Object);
			_handler = new Mock<IStoreDL>();
			_userShopper = new Mock<IUserShopper>();
			_slave = new SearchProductSlave(_userShopper.Object, _handler.Object);
		    p1 = " name: BOX base price: 100 description: this is a plastic box Discount: {none} Purchase Way: Immediate Quantity: 5 Store: X";
		    p2 = " name: Fraid Egg base price: 10 description: yami Discount: {none} Purchase Way: Immediate Quantity: 10 Store: T";
            Product[] allProducts =
			{
				new Product("P1", "BOX", 100, "this is a plastic box"),
				new Product("P2", "Fraid Egg", 10, "yami")
			};
			_handler.Setup(x => x.GetAllProducts()).Returns(allProducts);
		    Product pr1 = new Product("P1", "BOX", 100, "this is a plastic box");
		    Product pr2 = new Product("P2", "Fraid Egg", 10, "yami");
            Product[] product1 = { pr1 };
		    _handler.Setup(x => x.GetProductsByName("BOX")).Returns(product1);
		    _handler.Setup(x => x.GetStoreByProductId("P1")).Returns("S1");
		    _handler.Setup(x => x.GetStorebyID("S1")).Returns(new Store("X", "somewhere"));
		    _handler.Setup(x => x.GetStockListItembyProductID("P1")).
		        Returns(new StockListItem(5,pr1,null,PurchaseEnum.Immediate,"1"));
            Product[] product2 = { pr2 };
		    _handler.Setup(x => x.GetProductsByName("Fraid Egg")).Returns(product2);
		    _handler.Setup(x => x.GetStoreByProductId("P2")).Returns("S7");
		    _handler.Setup(x => x.GetStorebyID("S7")).Returns(new Store("T", "somewhere"));
		    _handler.Setup(x => x.GetStockListItembyProductID("P2")).
		        Returns(new StockListItem(10, pr2, null, PurchaseEnum.Immediate, "2"));
		    _handler.Setup(x => x.GetCategoryByName("WanderlandItems")).Returns(new Category("C1", "WanderlandItems"));
            _handler.Setup(x => x.GetCategoryByName("Books")).Returns(new Category("C2","Books"));
            LinkedList<Product> l1 = new LinkedList<Product>();
		    LinkedList<Product> l2 = new LinkedList<Product>();
		    l1.AddLast(pr2);
            _handler.Setup(x => x.GetAllCategoryProducts("C1")).Returns(l1);
		    _handler.Setup(x => x.GetAllCategoryProducts("C2")).Returns(l2);
		    _handler.Setup(x => x.GetAllCategorysNames()).Returns(new[] {"WanderlandItems", "Books"});

		}

		[TestMethod]
		public void SearchByNameNoFilteringSuccessTest()
		{
			_slave.SearchProduct("Name", "BOX", 0,0,"None");
            ProductFound(p1, _slave.Answer);
		}

        [TestMethod]
        public void SearchByNameSimilarResultTest()
        {
            _slave.SearchProduct("Name", "BUX", 0, 0, "None");
            Assert.AreEqual((int)SearchProductStatus.MistakeTipGiven, _slave.Answer.Status);
        }

        [TestMethod]
        public void SearchByNameNotExistTest()
        {
            _slave.SearchProduct("Name", "aerhaer", 0, 0, "None");
            NoneFound(_slave.Answer);
        }

        [TestMethod]
        public void SearchByCategoryNoFilteringSuccessTest()
        {
            _slave.SearchProduct("Category", "WanderlandItems", 0, 0, "None");
            ProductFound(p2, _slave.Answer);
        }

        [TestMethod]
        public void SearchByCategorySimilarResultTest()
        {
            _slave.SearchProduct("Category", "WanderlondItems", 0, 0, "None");
            Assert.AreEqual((int)SearchProductStatus.MistakeTipGiven, _slave.Answer.Status);
        }

        [TestMethod]
        public void SearchByCategoryNotExistTest()
        {
            _slave.SearchProduct("Category", "ABOX", 0, 0, "None");
            Assert.AreEqual((int)SearchProductStatus.CategoryNotFound, _slave.Answer.Status);
        }

        [TestMethod]
        public void SearchByCategoryEmptyTest()
        {
            _slave.SearchProduct("Category", "Books", 0, 0, "None");
            NoneFound(_slave.Answer);
        }

        [TestMethod]
        public void SearchByKeywordNoFilteringSuccessTest()
        {
            _slave.SearchProduct("KeyWord", "plastic", 0, 0, "None");
            ProductFound(p1, _slave.Answer);
        }

        [TestMethod]
        public void SearchByKeywordNotExistTest()
        {
            _slave.SearchProduct("KeyWord", "dfdfbzdb", 0, 0, "None");
            NoneFound(_slave.Answer);
        }

        [TestMethod]
        public void SearchByNameMinPriceFoundTest()
        {
            _slave.SearchProduct("Name", "BOX", 10, 0, "None");
            ProductFound(p1, _slave.Answer);
        }

        [TestMethod]
        public void SearchByNameMinPriceNotFoundTest()
        {
            _slave.SearchProduct("Name", "BOX", 1000, 0, "None");
            NoneFound(_slave.Answer);
        }

        [TestMethod]
        public void SearchByNameMaxPriceFoundTest()
        {
            _slave.SearchProduct("Name", "BOX", 0, 1000, "None");
            ProductFound(p1, _slave.Answer);
        }

        [TestMethod]
        public void SearchByNameMaxPriceNotFoundTest()
        {
            _slave.SearchProduct("Name", "BOX", 0, 10, "None");
            NoneFound(_slave.Answer);
        }

        [TestMethod]
        public void SearchByNamePriceRangeFoundTest()
        {
            _slave.SearchProduct("Name", "BOX", 10, 1000, "None");
            ProductFound(p1, _slave.Answer);
        }

        [TestMethod]
        public void SearchByNamePriceRangeNotFoundTest()
        {
            _slave.SearchProduct("Name", "BOX", 1000, 50000, "None");
            NoneFound(_slave.Answer);
        }

        [TestMethod]
        public void SearchByNameMinPriceWrongTest()
        {
            _slave.SearchProduct("Name", "BOX", -1, 0, "None");
            Assert.AreEqual((int)SearchProductStatus.PricesInvalid, _slave.Answer.Status);
        }

        [TestMethod]
        public void SearchByNameMaxPriceWrongTest()
        {
            _slave.SearchProduct("Name", "BOX", 0, -1, "None");
            Assert.AreEqual((int)SearchProductStatus.PricesInvalid, _slave.Answer.Status);
        }

        [TestMethod]
        public void SearchByNamePriceRangeWrongTest()
        {
            _slave.SearchProduct("Name", "BOX", 1000, 500, "None");
            Assert.AreEqual((int)SearchProductStatus.PricesInvalid, _slave.Answer.Status);
        }

        [TestMethod]
        public void SearchByNameCategoryFoundTest()
        {
            _slave.SearchProduct("Name", "Fraid Egg", 0, 0, "WanderlandItems");
            ProductFound(p2, _slave.Answer);
        }

        [TestMethod]
        public void SearchByNameCategoryNotFoundTest()
        {
            _slave.SearchProduct("Name", "Fraid Egg", 0, 0, "Books");
            NoneFound(_slave.Answer);
        }

        [TestMethod]
        public void SearchByCategoryConflictTest()
        {
            _slave.SearchProduct("Category", "WanderlandItems", 0, 0, "Books");
            NoneFound(_slave.Answer);
        }

        [TestMethod]
        public void SearchByNameMultipleConstraintsTest1()
        {
            _slave.SearchProduct("Name", "Fraid Egg", 10, 0, "WanderlandItems");
            ProductFound(p2, _slave.Answer);
        }

        [TestMethod]
        public void SearchByNameMultipleConstraintsTest2()
        {
            _slave.SearchProduct("Name", "Fraid Egg", 0, 1000, "WanderlandItems");
            ProductFound(p2, _slave.Answer);
        }

        [TestMethod]
        public void SearchByNameMultipleConstraintsTest3()
        {
            _slave.SearchProduct("Name", "Fraid Egg", 5, 1000, "WanderlandItems");
            ProductFound(p2, _slave.Answer);
        }

        [TestMethod]
        public void SearchByNameMultipleConstraintsTest4()
        {
            _slave.SearchProduct("Name", "Fraid Egg", 100, 1000, "WanderlandItems");
            NoneFound(_slave.Answer);
        }

	    [TestMethod]
	    public void NullDataGiven()
	    {
	        _slave.SearchProduct("Name", "", 0, 0, "None");
	        Assert.AreEqual((int)SearchProductStatus.NullValue, _slave.Answer.Status);
	    }

        [TestCleanup]
        public void CleanUpOpenStoreTest()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();
        }

        private void ProductFound(string p, MarketAnswer ans)
        {
            string[] expected = { p };
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

