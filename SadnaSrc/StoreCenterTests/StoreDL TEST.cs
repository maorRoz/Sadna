using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.StoreCenter;
using SadnaSrc.Main;
using System.Collections.Generic;
using System.Linq;

namespace StoreCenterTests
{
    [TestClass]
    public class StoreDL
    {

        private MarketYard market;
        private ModuleGlobalHandler handler;
        private Product toDeleteProduct;
        private Discount toDeleteDiscount;
        private Store toDeleteStore;
        private LotterySaleManagmentTicket toDeleteLottery;
        private LotteryTicket toDeleteTicket;
        private StockListItem toDeleteStockItem;
        [TestInitialize]
        public void BuildSupplyPoint()
        {
            market = MarketYard.Instance;
            handler = ModuleGlobalHandler.GetInstance();
            toDeleteProduct = null;
            toDeleteDiscount = null;
            toDeleteStore = null;
            toDeleteLottery = null;
            toDeleteTicket = null;
            toDeleteStockItem = null;
        }

        [TestMethod]
        public void GetProductID()
        {
            Product product = new Product("P1", "BOX", 100, "this is a plastic box"); // THIS exists in DB by SQL injection
            Product prod = handler.DataLayer.GetProductID("P1");
            Assert.AreEqual(product, prod);
        }
        [TestMethod]
        public void AddProductToDatabase()
        {
            Product product = new Product("P105", "X", 100, "Exits ForTests Only");
            handler.DataLayer.AddProductToDatabase(product);
            Product find = handler.DataLayer.GetProductID("P105");
            toDeleteProduct = product;
            Assert.AreEqual(product, find);
        }
        [TestMethod]
        public void RemoveProductToDatabase()
        {
            Product product = new Product("P105", "X", 100, "Exits ForTests Only");
            handler.DataLayer.AddProductToDatabase(product);
            handler.DataLayer.RemoveProduct(product);
            Product find = handler.DataLayer.GetProductID("P105");
            Assert.IsNull(find);
        }
        [TestMethod]
        public void EditProduct()
        {
            Product product = new Product("P105", "X", 100, "Exits ForTests Only");
            handler.DataLayer.AddProductToDatabase(product);
            product.Name = "lili";
            product.Description = "momo";
            product.BasePrice = 110;
            handler.DataLayer.EditProductInDatabase(product);
            Product find = handler.DataLayer.GetProductID("P105");
            toDeleteProduct = product;
            Assert.AreEqual("lili", find.Name);
            Assert.AreEqual("momo", find.Description);
            Assert.AreEqual(110, find.BasePrice);
        }
        [TestMethod]
        public void GetStorebyID()
        {
            Store expected = new Store("S1", "X", "Here 4"); // THIS exists in DB by SQL injection
            Store find = handler.DataLayer.GetStorebyID("S1");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void getStoreByName()
        {
            Store expected = new Store("S1", "X", "Here 4"); // THIS exists in DB by SQL injection
            Store find = handler.DataLayer.getStorebyName("X");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void AddStore()
        {
            Store expected = new Store("Stest", "X2", "Here 4");
            handler.DataLayer.AddStore(expected);
            Store find = handler.DataLayer.GetStorebyID("Stest");
            toDeleteStore = find;
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void EditStore()
        {
            Store expected = new Store("S9", "X3", "Here 4");
            Store find = handler.DataLayer.GetStorebyID("S9");

            handler.DataLayer.AddStore(expected);
            find = handler.DataLayer.GetStorebyID("S9");
            toDeleteStore = expected;
            Assert.IsTrue(expected.Equals(find));

            expected.Name = "mojo";
            expected.Address = "NOT HERE";
            Assert.IsFalse(expected.Equals(find));

            handler.DataLayer.EditStore(expected);
            find = handler.DataLayer.GetStorebyID("S9");
            Assert.AreEqual(expected, find);
        }

        [TestMethod]
        public void GetDiscount()
        {
            Discount discount = new Discount("D1", discountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true); // THIS exists in DB by SQL injection
            Discount find = handler.DataLayer.GetDiscount("D1");
            Assert.AreEqual(discount, find);
        }
        [TestMethod]
        public void AddDiscount()
        {
            Discount expected = new Discount("D102", discountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true); // THIS exists in DB by SQL injection
            handler.DataLayer.AddDiscount(expected);
            Discount find = handler.DataLayer.GetDiscount("D102");
            toDeleteDiscount = find;
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void EditDiscount()
        {
            Discount expected = new Discount("D103", discountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true);
            handler.DataLayer.AddDiscount(expected);
            Discount find = handler.DataLayer.GetDiscount("D103");
            toDeleteDiscount = expected;
            Assert.AreEqual(expected, find);
            expected.DiscountAmount = 30;
            handler.DataLayer.EditDiscountInDatabase(expected);
            find = handler.DataLayer.GetDiscount("D103");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void RemoveDiscount()
        {
            Discount expected = new Discount("D104", discountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true);
            handler.DataLayer.AddDiscount(expected);
            Discount find = handler.DataLayer.GetDiscount("D104");
            Assert.IsTrue(expected.Equals(find));
            handler.DataLayer.RemoveDiscount(expected);
            find = handler.DataLayer.GetDiscount("D104");
            Assert.IsNull(find);
        }

        [TestMethod]
        public void GetStockListItembyProductID()
        {
            Discount D = handler.DataLayer.GetDiscount("D1");//exist in DL by SQL injection
            Product P = handler.DataLayer.GetProductID("P1");//exist in DL by SQL injection
            StockListItem expected = new StockListItem(5, P, D, PurchaseEnum.Immediate, "S1"); //exist in DL by SQL injection
            StockListItem find = handler.DataLayer.GetStockListItembyProductID("P1");
            Assert.AreEqual(expected, find);

        }

        [TestMethod]
        public void AddStockListItemToDataBase()
        {
            Discount discount = new Discount("D105", discountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true);
            Product product = new Product("P110", "BOX", 100, "this is a plastic box");
            StockListItem expected = new StockListItem(10, product, discount, PurchaseEnum.Immediate, "S1");
            StockListItem find = handler.DataLayer.GetStockListItembyProductID("P110");
            Assert.IsNull(find);
            handler.DataLayer.AddStockListItemToDataBase(expected);
            find = handler.DataLayer.GetStockListItembyProductID("P110");
            toDeleteStockItem = expected;
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void RemoveStockListItem()
        {
            Discount discount = new Discount("D106", discountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true);
            Product product = new Product("P110", "BOX", 100, "this is a plastic box");
            StockListItem expected = new StockListItem(10, product, discount, PurchaseEnum.Immediate, "S1");
            handler.DataLayer.AddStockListItemToDataBase(expected);
            StockListItem find = handler.DataLayer.GetStockListItembyProductID("P110");
            Assert.AreEqual(expected, find);
            handler.DataLayer.RemoveStockListItem(expected);
            find = handler.DataLayer.GetStockListItembyProductID("P110");
            toDeleteStockItem = expected;
            Assert.IsNull(find);
        }
        [TestMethod]
        public void EditStockInDatabase()
        {
            Discount discount = new Discount("D107", discountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true);
            Product product = new Product("P111", "BOX", 100, "this is a plastic box");
            StockListItem expected = new StockListItem(10, product, discount, PurchaseEnum.Immediate, "S1");
            handler.DataLayer.AddStockListItemToDataBase(expected);
            StockListItem find = handler.DataLayer.GetStockListItembyProductID("P111");
            toDeleteStockItem = find;
            Assert.AreEqual(expected, find);
            expected.Quantity = 3;
            Assert.AreNotEqual(expected, find);
            handler.DataLayer.EditStockInDatabase(expected);
            find = handler.DataLayer.GetStockListItembyProductID("P111");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void GetLotteryByProductID()
        {
            Product P = handler.DataLayer.GetProductID("P1");//exist in DL by SQL injection
            LotterySaleManagmentTicket expected = new LotterySaleManagmentTicket("L1", P, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018")); //exist in DL by SQL injection
            LotterySaleManagmentTicket find = handler.DataLayer.GetLotteryByProductID(P.SystemId);
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void AddLottery()
        {
            Product P = handler.DataLayer.GetProductID("P3");//exist in DL by SQL injection
            LotterySaleManagmentTicket expected = new LotterySaleManagmentTicket("L101", P, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"));
            LotterySaleManagmentTicket find = handler.DataLayer.GetLotteryByProductID(P.SystemId);
            toDeleteLottery = expected;
            Assert.IsNull(find);
            handler.DataLayer.AddLottery(expected);
            find = handler.DataLayer.GetLotteryByProductID(P.SystemId);
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void removeLottery()
        {
            Product P = handler.DataLayer.GetProductID("P3");//exist in DL by SQL injection
            LotterySaleManagmentTicket expected = new LotterySaleManagmentTicket("L102", P, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"));
            handler.DataLayer.AddLottery(expected);
            LotterySaleManagmentTicket find = handler.DataLayer.GetLotteryByProductID(P.SystemId);
            Assert.AreEqual(expected, find);
            handler.DataLayer.RemoveLottery(expected);
            find = handler.DataLayer.GetLotteryByProductID(P.SystemId);
            Assert.IsNull(find);
        }
        [TestMethod]
        public void EditLotteryInDatabase()
        {
            Product P = handler.DataLayer.GetProductID("P3");//exist in DL by SQL injection
            LotterySaleManagmentTicket expected = new LotterySaleManagmentTicket("L101", P, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"));
            handler.DataLayer.AddLottery(expected);
            LotterySaleManagmentTicket find = handler.DataLayer.GetLotteryByProductID(P.SystemId);
            toDeleteLottery = expected;
            Assert.AreEqual(expected, find);
            expected.TotalMoneyPayed = 50;
            Assert.AreNotEqual(expected, find);
            handler.DataLayer.EditLotteryInDatabase(expected);
            find = handler.DataLayer.GetLotteryByProductID("P3");
            Assert.AreEqual(expected, find); ;
        }
        [TestMethod]
        public void GetAllActiveStores()
        {
            Store[] expected =
            {
                new Store("S1", "X", "Here 4"),
                new Store("S2", "Y", "Here 4"),
                new Store("S3", "M", "Here 4"),
                new Store("S4", "Cluckin Bell", "Los Santos"),
                new Store("S5", "The Red Rock", "Mivtza Yoav"),
                new Store("S6", "24", "Mezada"),
            };
            Store[] actual = handler.DataLayer.GetAllActiveStores().ToArray();
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
        [TestMethod]
        public void GetLotteryTicket()
        {
            LotteryTicket expected = new LotteryTicket("T1", "L1", 0, 0, 0, 0); //Exists in DB
            LotteryTicket find = handler.DataLayer.GetLotteryTicket("T1");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void AddLotteryTicket()
        {
            LotteryTicket expected = new LotteryTicket("T2", "L1", 0, 0, 0, 0); ;
            LotteryTicket find = handler.DataLayer.GetLotteryTicket("T2");
            toDeleteTicket = expected;
            Assert.IsNull(find);
            handler.DataLayer.AddLotteryTicket(expected);
            find = handler.DataLayer.GetLotteryTicket("T2");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void RemoveLotteryTicket()
        {
            LotteryTicket expected = new LotteryTicket("T3", "L1", 0, 0, 0, 0); ;
            handler.DataLayer.AddLotteryTicket(expected);
            LotteryTicket find = handler.DataLayer.GetLotteryTicket("T3");
            Assert.AreEqual(expected, find);
            handler.DataLayer.RemoveLotteryTicket(expected);
            find = handler.DataLayer.GetLotteryTicket("T3");
            Assert.IsNull(find);
        }
        [TestMethod]
        public void getAllTickets()
        {
            LinkedList<LotteryTicket> expected = new LinkedList<LotteryTicket>();
            LotteryTicket ticket2 = new LotteryTicket("T2", "L1", 0, 0, 0, 0); ;
            LotteryTicket ticket1 = new LotteryTicket("T1", "L1", 0, 0, 0, 0); ; //Exists in DB
            handler.DataLayer.AddLotteryTicket(ticket2);
            expected.AddLast(ticket1);
            expected.AddLast(ticket2);
            LinkedList<LotteryTicket> find = handler.DataLayer.getAllTickets("L1");
            Assert.AreEqual(expected.Count, find.Count);
            LotteryTicket[] findResults = new LotteryTicket[find.Count];
            find.CopyTo(findResults, 0);
            LotteryTicket[] expectedResults = new LotteryTicket[expected.Count];
            expected.CopyTo(expectedResults, 0);
            for (int i = 0; i < findResults.Length; i++)
            {
                Assert.AreEqual(findResults[i], expectedResults[i]);
            }
            handler.DataLayer.RemoveLotteryTicket(ticket2);
        }
        [TestMethod]
        public void GetAllStoreProductsID()
        {
            LinkedList<string> expected = new LinkedList<string>();
            expected.AddLast("P1");
            LinkedList<string> find = handler.DataLayer.GetAllStoreProductsID("S1");
            Assert.AreEqual(expected.Count, find.Count);
            string[] findResults = new string[find.Count];
            find.CopyTo(findResults, 0);
            string[] expectedResults = new string[expected.Count];
            expected.CopyTo(expectedResults, 0);
            for (int i = 0; i < findResults.Length; i++)
            {
                Assert.AreEqual(findResults[i], expectedResults[i]);
            }
        }

        [TestCleanup]
        public void CleanDb()
        {
            if (toDeleteTicket != null)
            {
                handler.DataLayer.RemoveLotteryTicket(toDeleteTicket);
            }
            if (toDeleteLottery != null)
            {
                handler.DataLayer.RemoveLottery(toDeleteLottery);
            }
            if (toDeleteDiscount != null)
            {
                handler.DataLayer.RemoveDiscount(toDeleteDiscount);
            }
            if (toDeleteProduct != null)
            {
                handler.DataLayer.RemoveProduct(toDeleteProduct);
            }
            if (toDeleteStockItem != null)
            {
                handler.DataLayer.RemoveStockListItem(toDeleteStockItem);
            }
            if (toDeleteStore != null)
            {
                handler.DataLayer.RemoveStore(toDeleteStore);
            }
        }
    }
}
