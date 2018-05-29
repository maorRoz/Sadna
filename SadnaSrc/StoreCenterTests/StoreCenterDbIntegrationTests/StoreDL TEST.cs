using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.StoreCenter;
using SadnaSrc.Main;
using System.Collections.Generic;
using System.Linq;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace StoreCenterTests.StoreCenterDbIntegrationTests
{
    [TestClass]
    public class StoreDLTests
    {

        private StoreDL handler;
        private LotteryTicket _toDeleteTicket;
        [TestInitialize]
        public void BuildSupplyPoint()
        {
            MarketDB.Instance.InsertByForce();
            handler = StoreDL.Instance;
            _toDeleteTicket = null;
        }
        [TestMethod]
        public void GetProductID()
        {
            Product product = new Product("P1", "BOX", 100, "this is a plastic box"); // THIS exists in DB by SQL injection
            Product prod = handler.GetProductID("P1");
            Assert.AreEqual(product, prod);
        }
        [TestMethod]
        public void AddProductToDatabase()
        {
            Product product = new Product("P105", "X", 100, "Exits ForTests Only");
            handler.AddProductToDatabase(product);
            Product find = handler.GetProductID("P105");
            Assert.AreEqual(product, find);
        }
        [TestMethod]
        public void RemoveProductToDatabase()
        {
            Product product = new Product("P105", "X", 100, "Exits ForTests Only");
            handler.AddProductToDatabase(product);
            handler.RemoveProduct(product);
            Product find = handler.GetProductID("P105");
            Assert.IsNull(find);
        }

        [TestMethod]
        public void GetUserIdByUserNameFail()
        {
            int find = handler.GetUserIDFromUserName("mmm");
            Assert.AreEqual(-1, find);
        }
        [TestMethod]
        public void GetUserIdByUserNameSuccess()
        {
            int find = handler.GetUserIDFromUserName("Arik1");
            Assert.AreEqual(1,find);
        }
        [TestMethod]
        public void EditLotteryTicketInDatabase()
        {
            _toDeleteTicket = new LotteryTicket("T99", "L4", 0, 1, 1, 5);
            handler.AddLotteryTicket(_toDeleteTicket);
            _toDeleteTicket.myStatus = LotteryTicketStatus.Cancel;
            handler.EditLotteryTicketInDatabase(_toDeleteTicket);
            LotteryTicket find = handler.GetLotteryTicket("T99");
            Assert.AreEqual(_toDeleteTicket, find);
        }

        [TestMethod]
        public void EditProduct()
        {
            Product product = new Product("P105", "X", 100, "Exits ForTests Only");
            handler.AddProductToDatabase(product);
            product.Name = "lili";
            product.Description = "momo";
            product.BasePrice = 110;
            handler.EditProductInDatabase(product);
            Product find = handler.GetProductID("P105");
            Assert.AreEqual("lili", find.Name);
            Assert.AreEqual("momo", find.Description);
            Assert.AreEqual(110, find.BasePrice);
        }
        [TestMethod]
        public void GetStorebyID()
        {
            Store expected = new Store("S1", "X", "Here 4"); // THIS exists in DB by SQL injection
            Store find = handler.GetStorebyID("S1");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void getStoreByName()
        {
            Store expected = new Store("S1", "X", "Here 4"); // THIS exists in DB by SQL injection
            Store find = handler.GetStorebyName("X");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void AddStore()
        {
            Store expected = new Store("Stest", "X2", "Here 4");
            handler.AddStore(expected);
            Store find = handler.GetStorebyID("Stest");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void EditStore()
        {
            Store expected = new Store("S9", "X3", "Here 4");
            Store find = handler.GetStorebyID("S9");

            handler.AddStore(expected);
            find = handler.GetStorebyID("S9");
            Assert.IsTrue(expected.Equals(find));

            expected.Name = "mojo";
            expected.Address = "NOT HERE";
            Assert.IsFalse(expected.Equals(find));

            handler.EditStore(expected);
            find = handler.GetStorebyID("S9");
            Assert.AreEqual(expected, find);
        }

        [TestMethod]
        public void GetDiscount()
        {
            Discount discount = new Discount("D1", DiscountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true); // THIS exists in DB by SQL injection
            Discount find = handler.GetDiscount("D1");
            Assert.AreEqual(discount, find);
        }
        [TestMethod]
        public void AddDiscount()
        {
            Discount expected = new Discount("D102", DiscountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true); // THIS exists in DB by SQL injection
            handler.AddDiscount(expected);
            Discount find = handler.GetDiscount("D102");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void EditDiscount()
        {
            Discount expected = new Discount("D103", DiscountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true);
            handler.AddDiscount(expected);
            Discount find = handler.GetDiscount("D103");
            Assert.AreEqual(expected, find);
            expected.DiscountAmount = 30;
            handler.EditDiscountInDatabase(expected);
            find = handler.GetDiscount("D103");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void RemoveDiscount()
        {
            Discount expected = new Discount("D104", DiscountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true);
            handler.AddDiscount(expected);
            Discount find = handler.GetDiscount("D104");
            Assert.IsTrue(expected.Equals(find));
            handler.RemoveDiscount(expected);
            find = handler.GetDiscount("D104");
            Assert.IsNull(find);
        }

        [TestMethod]
        public void GetStockListItembyProductID()
        {
            Discount discount = handler.GetDiscount("D1");
            Product product = handler.GetProductID("P1");
            StockListItem expected = new StockListItem(5, product, discount, PurchaseEnum.Immediate, "S1");
            StockListItem find = handler.GetStockListItembyProductID("P1");
            Assert.AreEqual(expected, find);

        }

        [TestMethod]
        public void AddStockListItemToDataBase()
        {
            Discount discount = new Discount("D105", DiscountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true);
            Product product = new Product("P110", "BOX", 100, "this is a plastic box");
            StockListItem expected = new StockListItem(10, product, discount, PurchaseEnum.Immediate, "S1");
            StockListItem find = handler.GetStockListItembyProductID("P110");
            Assert.IsNull(find);
            handler.AddStockListItemToDataBase(expected);
            find = handler.GetStockListItembyProductID("P110");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void RemoveStockListItem()
        {
            Discount discount = new Discount("D106", DiscountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true);
            Product product = new Product("P110", "BOX", 100, "this is a plastic box");
            StockListItem expected = new StockListItem(10, product, discount, PurchaseEnum.Immediate, "S1");
            handler.AddStockListItemToDataBase(expected);
            StockListItem find = handler.GetStockListItembyProductID("P110");
            Assert.AreEqual(expected, find);
            handler.RemoveStockListItem(expected);
            find = handler.GetStockListItembyProductID("P110");
            Assert.IsNull(find);
        }
        [TestMethod]
        public void EditStockInDatabase()
        {
            Discount discount = new Discount("D107", DiscountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true);
            Product product = new Product("P111", "BOX", 100, "this is a plastic box");
            StockListItem expected = new StockListItem(10, product, discount, PurchaseEnum.Immediate, "S1");
            handler.AddStockListItemToDataBase(expected);
            StockListItem find = handler.GetStockListItembyProductID("P111");
            Assert.AreEqual(expected, find);
            expected.Quantity = 3;
            Assert.AreNotEqual(expected, find);
            handler.EditStockInDatabase(expected);
            find = handler.GetStockListItembyProductID("P111");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void GetLotteryByProductID()
        {
            Product P = handler.GetProductID("P1");//exist in DL by SQL injection
            LotterySaleManagmentTicket expected = new LotterySaleManagmentTicket("L1", "X", P, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018")); //exist in DL by SQL injection
            LotterySaleManagmentTicket find = handler.GetLotteryByProductID(P.SystemId);
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void AddLottery()
        {
            Product P = handler.GetProductID("P3");//exist in DL by SQL injection
            LotterySaleManagmentTicket expected = new LotterySaleManagmentTicket("L101", "X", P, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"));
            LotterySaleManagmentTicket find = handler.GetLotteryByProductID(P.SystemId);
            Assert.IsNull(find);
            handler.AddLottery(expected);
            find = handler.GetLotteryByProductID(P.SystemId);
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void removeLottery()
        {
            Product P = handler.GetProductID("P3");//exist in DL by SQL injection
            LotterySaleManagmentTicket expected = new LotterySaleManagmentTicket("L102", "X", P, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"));
            handler.AddLottery(expected);
            LotterySaleManagmentTicket find = handler.GetLotteryByProductID(P.SystemId);
            Assert.AreEqual(expected, find);
            handler.RemoveLottery(expected);
            find = handler.GetLotteryByProductID(P.SystemId);
            Assert.IsNull(find);
        }
        [TestMethod]
        public void EditLotteryInDatabase()
        {
            Product P = handler.GetProductID("P3");//exist in DL by SQL injection
            LotterySaleManagmentTicket expected = new LotterySaleManagmentTicket("L101", "X", P, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"));
            handler.AddLottery(expected);
            LotterySaleManagmentTicket find = handler.GetLotteryByProductID(P.SystemId);
            Assert.AreEqual(expected, find);
            expected.TotalMoneyPayed = 50;
            Assert.AreNotEqual(expected, find);
            handler.EditLotteryInDatabase(expected);
            find = handler.GetLotteryByProductID("P3");
            Assert.AreEqual(expected, find); ;
        }
        [TestMethod]
        public void GetLotteryTicket()
        {
            LotteryTicket expected = new LotteryTicket("T1", "L1", 0, 0, 0, 0); //Exists in DB
            LotteryTicket find = handler.GetLotteryTicket("T1");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void AddLotteryTicket()
        {
            LotteryTicket expected = new LotteryTicket("T15", "L1", 0, 0, 0, 0); ;
            LotteryTicket find = handler.GetLotteryTicket("T15");
            _toDeleteTicket = expected;
            Assert.IsNull(find);
            handler.AddLotteryTicket(expected);
            find = handler.GetLotteryTicket("T15");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void RemoveLotteryTicket()
        {
            LotteryTicket expected = new LotteryTicket("T3", "L1", 0, 0, 0, 0);
            handler.AddLotteryTicket(expected);
            LotteryTicket find = handler.GetLotteryTicket("T3");
            Assert.AreEqual(expected, find);
            handler.RemoveLotteryTicket(expected);
            find = handler.GetLotteryTicket("T3");
            Assert.IsNull(find);
        }
        [TestMethod]
        public void getAllTickets()
        {
            LinkedList<LotteryTicket> expected = new LinkedList<LotteryTicket>();
            LotteryTicket ticket1 = new LotteryTicket("T1", "L1", 0, 0, 0, 0); ; //Exists in DB
            LotteryTicket ticket2 = new LotteryTicket("T3", "L1", 0, 0, 0, 0);
            handler.AddLotteryTicket(ticket2);
            _toDeleteTicket = ticket2;
            expected.AddLast(ticket1);
            expected.AddLast(ticket2);
            LinkedList<LotteryTicket> find = handler.GetAllTickets("L1");
            Assert.AreEqual(expected.Count, find.Count);
            LotteryTicket[] findResults = new LotteryTicket[find.Count];
            find.CopyTo(findResults, 0);
            LotteryTicket[] expectedResults = new LotteryTicket[expected.Count];
            expected.CopyTo(expectedResults, 0);
            for (int i = 0; i < findResults.Length; i++)
            {
                Assert.AreEqual(findResults[i], expectedResults[i]);
            }
        }
        [TestMethod]
        public void GetAllStoreProductsID()
        {
            LinkedList<string> expected = new LinkedList<string>();
            expected.AddLast("P1");
            expected.AddLast("P2");
            var find = handler.GetAllStoreProductsID("S1");
            Assert.AreEqual(expected.Count, find.Length);
            string[] findResults = new string[find.Length];
            find.CopyTo(findResults, 0);
            string[] expectedResults = new string[expected.Count];
            expected.CopyTo(expectedResults, 0);
            for (int i = 0; i < findResults.Length; i++)
            {
                Assert.AreEqual(findResults[i], expectedResults[i]);
            }
        }

        [TestMethod]
        public void GetCategoryByName()
        {
            var expected = new Category("C1", "WanderlandItems"); // THIS exists in DB by SQL injection
            var find = handler.GetCategoryByName("WanderlandItems");
            Assert.AreEqual(expected, find);
        }
        
        [TestMethod]
        public void GetAllCategoryProducts()
        {
            LinkedList<Product> expected = new LinkedList<Product>();
            expected.AddLast(new Product("P21", "Fraid Egg", 10, "yami"));
            var find = handler.GetAllCategoryProducts("C1");
            foreach (Product product in find)
            {
                Assert.AreEqual(product, expected.First.Value);
            }
        }
        [TestMethod]
        public void AddProductToCategory()
        {
            LinkedList<Product> expected = new LinkedList<Product>();
            expected.AddLast(new Product("P21", "Fraid Egg", 10, "yami"));
            var newProd = new Product("P10023","bla",10,"da");
            handler.AddProductToDatabase(newProd);
            expected.AddFirst(newProd);
            handler.AddProductToCategory("C1", "P10023");
            var find = handler.GetAllCategoryProducts("C1");
            Product[] expectedresults = new Product[expected.Count];
            Product[] findresults = new Product[find.Count];
            expected.CopyTo(expectedresults,0);
            find.CopyTo(findresults,0);
            for (int i = 0; i < expectedresults.Length; i++)
            {
                Assert.AreEqual(expectedresults[i],findresults[i]);
            }
        }
        [TestMethod]
        public void RemoveProductFromCategory()
        {
            LinkedList<Product> expected = new LinkedList<Product>();
            expected.AddLast(new Product("P21", "Fraid Egg", 10, "yami"));
            var find = handler.GetAllCategoryProducts("C1");
            foreach (Product product in find)
            {
                Assert.AreEqual(product, expected.First.Value);
            }

            expected.RemoveLast();
            handler.RemoveProductFromCategory("C1", "P21");
            find = handler.GetAllCategoryProducts("C1");
            foreach (Product product in find)
            {
                Assert.AreEqual(product, expected.First.Value);
            }
        }
        [TestMethod]
        public void GetCategoryDiscount()
        {
            CategoryDiscount expected = new CategoryDiscount("d1", "MTG_Cards", "T", DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"),50);
            var find = handler.GetCategoryDiscount("MTG_Cards", "T");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void AddCategoryDiscount()
        {
            CategoryDiscount expected = new CategoryDiscount("d2", "WanderlandItems", "T", DateTime.Parse("1/1/2018"), DateTime.Parse("31/12/2018"), 50);
            handler.AddCategoryDiscount(expected);
            var find = handler.GetCategoryDiscount("WanderlandItems", "T");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void EditCategoryDiscount()
        {
            CategoryDiscount expected = new CategoryDiscount("d2", "WanderlandItems", "T", DateTime.Parse("1/1/2018"), DateTime.Parse("31/12/2018"), 50);
            handler.AddCategoryDiscount(expected);
            var find = handler.GetCategoryDiscount("WanderlandItems", "T");
            Assert.AreEqual(expected, find);
            expected.StartDate = DateTime.Parse("15/07/2018");
            handler.EditCategoryDiscount(expected);
            find = handler.GetCategoryDiscount("WanderlandItems", "T");
            Assert.AreEqual(expected, find);

        }
        [TestMethod]
        public void RemoveCategoryDiscount()
        {
            CategoryDiscount expected = new CategoryDiscount("d2", "WanderlandItems", "T", DateTime.Parse("1/1/2018"), DateTime.Parse("31/12/2018"), 50);
            handler.AddCategoryDiscount(expected);
            var find = handler.GetCategoryDiscount("WanderlandItems", "T");
            Assert.AreEqual(expected, find);
            handler.RemoveCategoryDiscount(expected);
            find = handler.GetCategoryDiscount("WanderlandItems", "T");
            Assert.IsNull(find);

        }

        [TestCleanup]
        public void CleanDb()
        {
            MarketDB.Instance.CleanByForce();
            MarketYard.CleanSession();

        }
    }
}