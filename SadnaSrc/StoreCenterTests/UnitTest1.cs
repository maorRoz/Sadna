using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SadnaSrc.StoreCenter;
using SadnaSrc.Main;
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
            Product product = new Product("P100", "BOX", 100, "this is a plastic box"); // THIS exists in DB by SQL injection
            Product prod = handler.DataLayer.GetProductID("P100");
            Assert.IsTrue(product.Equals(prod));
        }
        [TestMethod]
        public void AddProductToDatabase()
        {
            Product product = new Product("P105", "X", 100, "Exits ForTests Only"); // THIS exists in DB by SQL injection
            handler.DataLayer.AddProductToDatabase(product);
            Product find = handler.DataLayer.GetProductID("P105");
            Assert.IsTrue(product.Equals(find));
        }
        [TestMethod]
        public void RemoveProductToDatabase()
        {
            Product product = new Product("P105", "X", 100, "Exits ForTests Only"); // THIS exists in DB by SQL injection
            handler.DataLayer.AddProductToDatabase(product);
            handler.DataLayer.RemoveProduct(product);
            Product find = handler.DataLayer.GetProductID("P105");
            Assert.IsTrue(find == null);
        }
        [TestMethod]
        public void EditProduct()
        {
            Product product = new Product("P105", "X", 100, "Exits ForTests Only"); // THIS exists in DB by SQL injection
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
            Store Copy = new Store("S1", "X", "HERE 4"); // THIS exists in DB by SQL injection
            Store find = handler.DataLayer.GetStore("S1");
            Assert.IsTrue(Copy.Equals(find));
        }
        [TestMethod]
        public void AddStore()
        {
            Store Copy = new Store("S5", "X", "HERE 4"); // THIS exists in DB by SQL injection
            Store find = handler.DataLayer.GetStore("S5");
            Assert.IsFalse(Copy.Equals(find));

            handler.DataLayer.AddStore(Copy);
            find = handler.DataLayer.GetStore("S5");
            Assert.IsTrue(Copy.Equals(find));
        }

    }
}

        /**
         * 
        private PurchaseHistory[] GetPurchaseHistory(SQLiteDataReader dbReader)
        public StockListItem GetStockListItembyProductID(string product)
        public void AddStore(Store temp)
        public void AddLotteryTicket(LotteryTicket lottery)
        public string[] GetHistory(Store store)
        public void AddDiscount(Discount discount)
        public void AddStockListItemToDataBase(StockListItem stockListItem)
        public void RemoveLottery(LotterySaleManagmentTicket lotteryManagment)
        public void RemoveStockListItem(StockListItem stockListItem)
        public void EditDiscountInDatabase(Discount discount)
        public void EditStore(Store store)
        public void EditStockInDatabase(StockListItem stockListItem)
        public LinkedList<Store> GetAllActiveStores() // all active stores
        public void RemoveDiscount(Discount discount)
        public void AddLottery(LotterySaleManagmentTicket lotteryManagment)
        public LinkedList<string> GetAllStoreProductsID(object systemID)
        public LotterySaleManagmentTicket GetLotteryByProductID(string productID)
        public void EditLotteryInDatabase(LotterySaleManagmentTicket lotteryManagment)
        }

    }
}
    }
}
**/
 