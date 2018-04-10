using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.StoreCenter;
using SadnaSrc.Main;
using System.Collections.Generic;

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
        /**
        [TestMethod]
        public void GetProductID()
        {
            Product product = new Product("P100", "BOX", 100, "this is a plastic box"); // THIS exists in DB by SQL injection
            Product prod = handler.DataLayer.GetProductID("P100");
            Assert.IsTrue(product.Equals(prod));
        }
        [TestMethod]
        public void AddProductToDatabase()
        {
            Product product = new Product("P105", "X", 100, "Exits ForTests Only");
            handler.DataLayer.AddProductToDatabase(product);
            Product find = handler.DataLayer.GetProductID("P105");
            Assert.IsTrue(product.Equals(find));
            handler.DataLayer.RemoveProduct(product);
        }
        [TestMethod]
        public void RemoveProductToDatabase()
        {
            Product product = new Product("P105", "X", 100, "Exits ForTests Only");
            handler.DataLayer.AddProductToDatabase(product);
            handler.DataLayer.RemoveProduct(product);
            Product find = handler.DataLayer.GetProductID("P105");
            Assert.IsTrue(find == null);
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
            Assert.IsTrue(find.Name.Equals("lili"));
            Assert.IsTrue(find.Description.Equals("momo"));
            Assert.IsTrue(find.BasePrice == 110);
            handler.DataLayer.RemoveProduct(product);
        }
        [TestMethod]
        public void GetStore()
        {
            Store Copy = new Store("S1", "X", "Here 4"); // THIS exists in DB by SQL injection
            Store find = handler.DataLayer.GetStore("S1");
            Assert.IsTrue(Copy.Equals(find));
        }
        [TestMethod]
        public void AddStore() 
        {
            Store Copy = new Store("Stest", "X", "Here 4");
            handler.DataLayer.AddStore(Copy);
            Store find = handler.DataLayer.GetStore("Stest");
            Assert.IsTrue(Copy.Equals(find));
            handler.DataLayer.RemoveStore(find);
        }
        [TestMethod]
        public void EditStore()
        {
            Store Copy = new Store("S9", "X", "Here 4");
            Store find = handler.DataLayer.GetStore("S9");

               handler.DataLayer.AddStore(Copy);
               find = handler.DataLayer.GetStore("S9");
               Assert.IsTrue(Copy.Equals(find));

               Copy.Name = "mojo";
               Copy.Address = "NOT HERE";
               Assert.IsFalse(Copy.Equals(find));

               handler.DataLayer.EditStore(Copy);
               find = handler.DataLayer.GetStore("S9");
               Assert.IsTrue(Copy.Equals(find));
               handler.DataLayer.RemoveStore(find);
        }

        [TestMethod]
        public void GetDiscount()
        {
            Discount discount = new Discount("D101", discountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true); // THIS exists in DB by SQL injection
            Discount find = handler.DataLayer.GetDiscount("D101");
            Assert.IsTrue(find.Equals(discount));
        }
        [TestMethod]
        public void AddDiscount()
        {
            Discount Copy = new Discount("D102", discountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true); // THIS exists in DB by SQL injection
            handler.DataLayer.AddDiscount(Copy);
            Discount find = handler.DataLayer.GetDiscount("D102");
            Assert.IsTrue(Copy.Equals(find));
            handler.DataLayer.RemoveDiscount(Copy);
        }
        [TestMethod]
        public void EditDiscount()
        {
           Discount Copy = new Discount("D103", discountTypeEnum.Hidden, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"), 50, true);
           handler.DataLayer.AddDiscount(Copy);
           Discount find = handler.DataLayer.GetDiscount("D103");
           Assert.IsTrue(Copy.Equals(find));
           Copy.DiscountAmount = 30;
           handler.DataLayer.EditDiscountInDatabase(Copy);
           find = handler.DataLayer.GetDiscount("D103");
           Assert.IsTrue(Copy.Equals(find));
           handler.DataLayer.RemoveDiscount(Copy);
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
            Assert.IsTrue(find==null);
        }
        
        [TestMethod]
        public void GetStockListItembyProductID()
        {
            Discount D = handler.DataLayer.GetDiscount("D101");//exist in DL by SQL injection
            Product P = handler.DataLayer.GetProductID("P100");//exist in DL by SQL injection
            StockListItem Copy = new StockListItem(5, P, D, PurchaseEnum.Immediate, "S1"); //exist in DL by SQL injection
            StockListItem find = handler.DataLayer.GetStockListItembyProductID("P100");
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
            Product P = handler.DataLayer.GetProductID("P101");//exist in DL by SQL injection
            LotterySaleManagmentTicket Copy = new LotterySaleManagmentTicket("L100", P, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018")); //exist in DL by SQL injection
            LotterySaleManagmentTicket find = handler.DataLayer.GetLotteryByProductID(P.SystemId);
            Assert.AreEqual(Copy, find);
        }
        [TestMethod]
        public void AddLottery()
        {
            Product P = handler.DataLayer.GetProductID("P102");//exist in DL by SQL injection
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
            Product P = handler.DataLayer.GetProductID("P102");//exist in DL by SQL injection
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
            Product P = handler.DataLayer.GetProductID("P102");//exist in DL by SQL injection
            LotterySaleManagmentTicket Copy = new LotterySaleManagmentTicket("L101", P, DateTime.Parse("01/01/2018"), DateTime.Parse("31/12/2018"));
            handler.DataLayer.AddLottery(Copy);
            LotterySaleManagmentTicket find = handler.DataLayer.GetLotteryByProductID(P.SystemId);
            Assert.AreEqual(Copy, find);
            Copy.TotalMoneyPayed = 50;
            Assert.AreNotEqual(Copy, find);
            handler.DataLayer.EditLotteryInDatabase(Copy);
            find = handler.DataLayer.GetLotteryByProductID("P102");
            Assert.AreEqual(Copy, find);
            handler.DataLayer.RemoveLottery(Copy);
        }
        [TestMethod]
        public void GetAllActiveStores()
        {
            LinkedList<Store> Copy = new LinkedList<Store>();
            Store S = new Store("S1", "X", "Here 4"); //Exists in DB
            Copy.AddLast(S);
            S = new Store("S2", "Y", "Here 4");
            Copy.AddLast(S);
            S = new Store("S3", "M", "Here 4");
            Copy.AddLast(S);
            LinkedList<Store> find = handler.DataLayer.GetAllActiveStores();
            Assert.AreEqual(find.Count, Copy.Count);
            Store[] findResults = new Store[find.Count];
            find.CopyTo(findResults, 0);
            Store[] CopyResults = new Store[Copy.Count];
            Copy.CopyTo(CopyResults, 0);
            for (int i=0; i<findResults.Length; i++)
            {
                Assert.AreEqual(findResults[i], CopyResults[i]);
            }
        }
        [TestMethod]
         public void GetLotteryTicket()
         {
             LotteryTicket Copy = new LotteryTicket(0, 0, "L100", "T100",1); //Exists in DB
             LotteryTicket find = handler.DataLayer.GetLotteryTicket("T100");
             Assert.AreEqual(Copy, find);
         }
         [TestMethod]
         public void AddLotteryTicket()
         {
             LotteryTicket Copy = new LotteryTicket(0, 0, "L100", "T101",1); 
             LotteryTicket find = handler.DataLayer.GetLotteryTicket("T101");
             Assert.IsNull(find);
             handler.DataLayer.AddLotteryTicket(Copy);
             find = handler.DataLayer.GetLotteryTicket("T101");
             Assert.AreEqual(find, Copy);
             handler.DataLayer.RemoveLotteryTicket(Copy);
         }
        [TestMethod]
        public void RemoveLotteryTicket()
        {
            LotteryTicket Copy = new LotteryTicket(0, 0, "L100", "T101", 1);
            handler.DataLayer.AddLotteryTicket(Copy);
            LotteryTicket find = handler.DataLayer.GetLotteryTicket("T101");
            Assert.AreEqual(find, Copy);
            handler.DataLayer.RemoveLotteryTicket(Copy);
            find = handler.DataLayer.GetLotteryTicket("T101");
            Assert.IsNull(find);
        }**/

    }
}
//'D101', 'HIDDEN', '01/01/2018', '31/12/2018', 50, 'true'
/**
internal LinkedList<LotteryTicket> getAllTickets(string systemID)    
private PurchaseHistory[] GetPurchaseHistory(SQLiteDataReader dbReader)
public string[] GetHistory(Store store)
public LinkedList<string> GetAllStoreProductsID(string systemID)
**/
