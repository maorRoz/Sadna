﻿using System;
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
        [TestInitialize]
        public void BuildSupplyPoint()
        {
            market = MarketYard.Instance;
            handler = ModuleGlobalHandler.GetInstance();
        }
        
        [TestMethod]
        public void GetProductID()
        {
            Product product = new Product("P1", "BOX", 100, "this is a plastic box"); // THIS exists in DB by SQL injection
            Product prod = handler.DataLayer.GetProductID("P1");
            Assert.AreEqual(product,prod);
        }
        [TestMethod]
        public void AddProductToDatabase()
        {
            Product product = new Product("P105", "X", 100, "Exits ForTests Only");
            handler.DataLayer.AddProductToDatabase(product);
            Product find = handler.DataLayer.GetProductID("P105");
            Assert.AreEqual(product,find);
            handler.DataLayer.RemoveProduct(product);
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
            Assert.AreEqual("lili",find.Name);
            Assert.AreEqual("momo",find.Description);
            Assert.AreEqual(110,find.BasePrice);
            handler.DataLayer.RemoveProduct(product);
        }
        [TestMethod]
        public void GetStore()
        {
            Store expected = new Store("S1", "X", "Here 4"); // THIS exists in DB by SQL injection
            Store find = handler.DataLayer.GetStore("S1");
            Assert.AreEqual(expected, find);
        }
        [TestMethod]
        public void AddStore() 
        {
            Store copy = new Store("Stest", "X2", "Here 4");
            handler.DataLayer.AddStore(copy);
            Store find = handler.DataLayer.GetStore("Stest");
            Assert.AreEqual(copy, find);
            handler.DataLayer.RemoveStore(find);
        }
        [TestMethod]
        public void EditStore()
        {
            Store copy = new Store("S9", "X3", "Here 4");
            Store find = handler.DataLayer.GetStore("S9");

            handler.DataLayer.AddStore(copy);
            find = handler.DataLayer.GetStore("S9");
            Assert.IsTrue(copy.Equals(find));

            copy.Name = "mojo";
            copy.Address = "NOT HERE";
            Assert.IsFalse(copy.Equals(find));

            handler.DataLayer.EditStore(copy);
            find = handler.DataLayer.GetStore("S9");
            Assert.AreEqual(copy, find);
            handler.DataLayer.RemoveStore(find);
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
            Discount copy = new Discount("D102", discountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true); // THIS exists in DB by SQL injection
            handler.DataLayer.AddDiscount(copy);
            Discount find = handler.DataLayer.GetDiscount("D102");
            Assert.AreEqual(copy, find);
            handler.DataLayer.RemoveDiscount(copy);
        }
        [TestMethod]
        public void EditDiscount()
        {
           Discount copy = new Discount("D103", discountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true);
           handler.DataLayer.AddDiscount(copy);
           Discount find = handler.DataLayer.GetDiscount("D103");
           Assert.AreEqual(copy, find);
           copy.DiscountAmount = 30;
           handler.DataLayer.EditDiscountInDatabase(copy);
           find = handler.DataLayer.GetDiscount("D103");
           Assert.AreEqual(copy,find);
           handler.DataLayer.RemoveDiscount(copy);
        }
        [TestMethod]
        public void RemoveDiscount()
        {
            Discount Copy = new Discount("D104", discountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true); 
            handler.DataLayer.AddDiscount(Copy);
            Discount find = handler.DataLayer.GetDiscount("D104");
            Assert.IsTrue(Copy.Equals(find));
            handler.DataLayer.RemoveDiscount(Copy);
            find = handler.DataLayer.GetDiscount("D104");
            Assert.IsNull(find);
        }
        
        [TestMethod]
        public void GetStockListItembyProductID()
        {
            Discount D = handler.DataLayer.GetDiscount("D1");//exist in DL by SQL injection
            Product P = handler.DataLayer.GetProductID("P1");//exist in DL by SQL injection
            StockListItem Copy = new StockListItem(5, P, D, PurchaseEnum.Immediate, "S1"); //exist in DL by SQL injection
            StockListItem find = handler.DataLayer.GetStockListItembyProductID("P1");
            Assert.AreEqual(Copy, find);

        }
        
        [TestMethod]
        public void AddStockListItemToDataBase()
        {
            Discount discount = new Discount("D105", discountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true);
            Product product = new Product("P110", "BOX", 100, "this is a plastic box");
            StockListItem Copy = new StockListItem(10, product, discount, PurchaseEnum.Immediate, "S1");
            StockListItem find = handler.DataLayer.GetStockListItembyProductID("P110");
            Assert.IsNull(find);
            handler.DataLayer.AddStockListItemToDataBase(Copy);
            find = handler.DataLayer.GetStockListItembyProductID("P110");
            Assert.AreEqual(Copy, find);
            handler.DataLayer.RemoveStockListItem(Copy);
        }
        [TestMethod]
        public void RemoveStockListItem()
        {
            Discount discount = new Discount("D105", discountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true);
            Product product = new Product("P110", "BOX", 100, "this is a plastic box");
            StockListItem Copy = new StockListItem(10, product, discount, PurchaseEnum.Immediate, "S1");
            handler.DataLayer.AddStockListItemToDataBase(Copy);
            StockListItem find = handler.DataLayer.GetStockListItembyProductID("P110");
            Assert.AreEqual(Copy, find);
            handler.DataLayer.RemoveStockListItem(Copy);
            find = handler.DataLayer.GetStockListItembyProductID("P110");
            Assert.IsNull(find);
        }
        [TestMethod]
        public void EditStockInDatabase()
        {
            Discount discount = new Discount("D106", discountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true);
            Product product = new Product("P111", "BOX", 100, "this is a plastic box");
            StockListItem Copy = new StockListItem(10, product, discount, PurchaseEnum.Immediate, "S1");
            handler.DataLayer.AddStockListItemToDataBase(Copy);
            StockListItem find = handler.DataLayer.GetStockListItembyProductID("P111");
            Assert.AreEqual(Copy, find);
            Copy.Quantity = 3;
            Assert.AreNotEqual(Copy, find);
            handler.DataLayer.EditStockInDatabase(Copy);
            find = handler.DataLayer.GetStockListItembyProductID("P111");
            Assert.AreEqual(Copy, find);
            handler.DataLayer.RemoveStockListItem(Copy);
        }
        [TestMethod]
        public void GetLotteryByProductID()
        {
            Product P = handler.DataLayer.GetProductID("P1");//exist in DL by SQL injection
            LotterySaleManagmentTicket Copy = new LotterySaleManagmentTicket("L1", P, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018")); //exist in DL by SQL injection
            LotterySaleManagmentTicket find = handler.DataLayer.GetLotteryByProductID(P.SystemId);
            Assert.AreEqual(Copy, find);
        }
        [TestMethod]
        public void AddLottery()
        {
            Product P = handler.DataLayer.GetProductID("P3");//exist in DL by SQL injection
            LotterySaleManagmentTicket Copy = new LotterySaleManagmentTicket("L101", P, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"));
            LotterySaleManagmentTicket find = handler.DataLayer.GetLotteryByProductID(P.SystemId);
            Assert.IsNull(find);
            handler.DataLayer.AddLottery(Copy);
            find = handler.DataLayer.GetLotteryByProductID(P.SystemId);
            Assert.AreEqual(Copy, find);
            handler.DataLayer.RemoveLottery(Copy);
        }
        [TestMethod]
        public void removeLottery()
        {
            Product P = handler.DataLayer.GetProductID("P3");//exist in DL by SQL injection
            LotterySaleManagmentTicket Copy = new LotterySaleManagmentTicket("L102", P, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"));
            handler.DataLayer.AddLottery(Copy);
            LotterySaleManagmentTicket  find = handler.DataLayer.GetLotteryByProductID(P.SystemId);
            Assert.AreEqual(Copy, find);
            handler.DataLayer.RemoveLottery(Copy);
            find = handler.DataLayer.GetLotteryByProductID(P.SystemId);
            Assert.IsNull(find);
        }
        [TestMethod]
        public void EditLotteryInDatabase()
        {
            Product P = handler.DataLayer.GetProductID("P3");//exist in DL by SQL injection
            LotterySaleManagmentTicket Copy = new LotterySaleManagmentTicket("L101", P, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"));
            handler.DataLayer.AddLottery(Copy);
            LotterySaleManagmentTicket find = handler.DataLayer.GetLotteryByProductID(P.SystemId);
            Assert.AreEqual(Copy, find);
            Copy.TotalMoneyPayed = 50;
            Assert.AreNotEqual(Copy, find);
            handler.DataLayer.EditLotteryInDatabase(Copy);
            find = handler.DataLayer.GetLotteryByProductID("P3");
            Assert.AreEqual(Copy, find);
            handler.DataLayer.RemoveLottery(Copy);
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
            for (int i=0; i< actual.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
        [TestMethod]
         public void GetLotteryTicket()
         {
             LotteryTicket copy = new LotteryTicket(0, 0, "L1", "T1",0); //Exists in DB
             LotteryTicket find = handler.DataLayer.GetLotteryTicket("T1");
             Assert.AreEqual(copy, find);
         }
         [TestMethod]
         public void AddLotteryTicket()
         {
             LotteryTicket Copy = new LotteryTicket(0, 0, "L1", "T2",0); 
             LotteryTicket find = handler.DataLayer.GetLotteryTicket("T2");
             Assert.IsNull(find);
             handler.DataLayer.AddLotteryTicket(Copy);
             find = handler.DataLayer.GetLotteryTicket("T2");
             Assert.AreEqual(find, Copy);
             handler.DataLayer.RemoveLotteryTicket(Copy);
         }
        [TestMethod]
        public void RemoveLotteryTicket()
        {
            LotteryTicket Copy = new LotteryTicket(0, 0, "L1", "T3", 0);
            handler.DataLayer.AddLotteryTicket(Copy);
            LotteryTicket find = handler.DataLayer.GetLotteryTicket("T3");
            Assert.AreEqual(find, Copy);
            handler.DataLayer.RemoveLotteryTicket(Copy);
            find = handler.DataLayer.GetLotteryTicket("T3");
            Assert.IsNull(find);
        }

    }
}
//'D1', 'HIDDEN', '01/01/2018', '31/12/2018', 50, 'true'
/**
internal LinkedList<LotteryTicket> getAllTickets(string systemID)    
private PurchaseHistory[] GetPurchaseHistory(SQLiteDataReader dbReader)
public string[] GetHistory(Store store)
public LinkedList<string> GetAllStoreProductsID(string systemID)
**/
