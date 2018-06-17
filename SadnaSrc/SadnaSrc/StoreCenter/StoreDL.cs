using SadnaSrc.Main;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class StoreDL : IStoreDL
    {

        private static StoreDL _instance;

        public static StoreDL Instance => _instance ?? (_instance = new StoreDL());

        private IMarketDB dbConnection;

        private StoreDL()
        {
            dbConnection = new ProxyMarketDB();
        }

        public string[] GetAllStoresIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("Store", "SystemID"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids.ToArray();
        }
        public string[] GetAllProductIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("Products", "SystemID"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids.ToArray();
        }
        public string[] GetAllDiscountIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("Discount", "DiscountCode"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids.ToArray();
        }
        public void AddProductToCategory(string categoryid, string productid)
        {
            dbConnection.CheckInput(categoryid); dbConnection.CheckInput(productid);
            string[] paramsNames = {"@categoryParam", "@productParam"};
            object[] values = { categoryid, productid };
            dbConnection.InsertTable("CategoryProductConnection", "CategoryID, ProductID",
                paramsNames, values);
        }
        public void RemoveProductFromCategory(string categoryid, string productid)
        {
            dbConnection.CheckInput(categoryid); dbConnection.CheckInput(productid);
            dbConnection.DeleteFromTable("CategoryProductConnection", "ProductID = '" + productid + "' AND CategoryID = '"+ categoryid + "'");
        }

        public void AddPromotionHistory(string store, string managerName, string promotedName, string[] permissions,string description)
        {
            dbConnection.CheckInput(store); dbConnection.CheckInput(managerName);
            dbConnection.CheckInput(promotedName); dbConnection.CheckInput(description);
            foreach (string val in permissions)
                dbConnection.CheckInput(val);

            dbConnection.InsertTable("PromotionHistory",
                "Store,Promoter,Promoted,Permissions,PromotionDate,Description",
                new []{"@storeParam","@promoterParam","@promotedParam","@permissionsParam","@dateParam","@descriptionParam"},
                new object[]{store,managerName,promotedName, string.Join(",",permissions), DateTime.Now,description});
        }

        public string[] GetPromotionHistory(string store)
        {
            dbConnection.CheckInput(store);
            var historyRecords = new LinkedList<string>();
            using(var dbReader = dbConnection.freeStyleSelect("SELECT * FROM PromotionHistory WHERE Store = '"+store+"' ORDER BY PromotionDate ASC"))
            {
                while (dbReader.Read())
                {
                    historyRecords.AddLast("Store: " + dbReader.GetString(0) + " Promoter: " + dbReader.GetString(1) +
                                           " Promoted: " + dbReader.GetString(2) + " Permissions: " + dbReader.GetString(3) +
                                           " Date: " + dbReader.GetDateTime(4).ToString("dd/MM/yyyy") + " Description: " + dbReader.GetString(5));
                }
            }
            return historyRecords.ToArray();
        }

        public string[] GetAllLotteryTicketIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("LotteryTicket", "myID"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids.ToArray();
        }
        public string[] GetAllLotteryManagmentIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("LotteryTable", "SystemID"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids.ToArray();
        }
      
        public Store GetStorebyName(string storeName)
        {
            dbConnection.CheckInput(storeName);
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Store", "*", "Name = '" + storeName + "'"))
            {
                while (dbReader.Read())
                {
                    return new Store(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetString(2),
                        dbReader.GetString(3));
                }

                return null;
            }
        }

		public Product[] GetProductsByName(string name)
		{
		    dbConnection.CheckInput(name);
            LinkedList<Product> products = new LinkedList<Product>();
			using (var dbReader = dbConnection.SelectFromTableWithCondition("Products", "*", "Name = '" + name + "'"))
			{
				while (dbReader.Read())
				{
					products.AddLast(new Product(dbReader.GetString(0),dbReader.GetString(1), dbReader.GetDouble(2), dbReader.GetString(3)));
				}

				return products.ToArray();
			}
		}

	    public string GetStoreByProductId(string productId)
	    {
	        dbConnection.CheckInput(productId);
            string store = null;
			using (var dbReader = dbConnection.SelectFromTableWithCondition("Stock", "StockID", "ProductSystemID = '" + productId + "'"))
			{
				while (dbReader.Read())
				{
					store = dbReader.GetString(0);
				}

				return store;
			}
		}

	    public void EditLotteryTicketInDatabase(LotteryTicket ticket)
        {

            string[] columnNames =
            {
                "myID",
                "LotteryID",
                "IntervalStart",
                "IntervalEnd",
                "Cost",
                "Status",
                "UserID"
            };
            dbConnection.CheckInput(ticket.myID);
            object[] ticketVals = ticket.GetTicketValuesArray();
            foreach (object val in ticketVals)
                dbConnection.CheckInput(val.ToString());
            dbConnection.UpdateTable("LotteryTicket", "myID = '" + ticket.myID + "'", columnNames,
                new []{"@idParam","@lotteryParam","@interStart","@interEnd","@cost","@stat","@user"}, ticketVals);
        }


        public Store GetStorebyID(string storeid)
        {
            dbConnection.CheckInput(storeid);
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Store", "*", "SystemID = '" + storeid + "'"))
            {
                while (dbReader.Read())
                {
                    return new Store(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetString(2),
                        dbReader.GetString(3));
                }

                return null;
            }
        }

        public void AddProductToDatabase(Product product)
        {
            object[] productVals = product.GetProductValuesArray();
            foreach (object val in productVals)
                dbConnection.CheckInput(val.ToString());
            dbConnection.InsertTable("Products", "SystemID, name, BasePrice, description",
                new []{"@idParam","@name","@price","@desc"}, productVals);
            if (product.Categories == null)
                return;
            foreach (string category in product.Categories)
            {
                dbConnection.CheckInput(category); dbConnection.CheckInput(product.SystemId);
                dbConnection.InsertTable("CategoryProductConnection", "CategoryID, ProductID",
                    new [] { "@categoryParam", "@productParam" }, new []{category, product.SystemId});
            }
        }

        public void EditProductInDatabase(Product product)
        {
            string[] columnNames =
            {
                "SystemID",
                "name",
                "BasePrice",
                "description"
            };
            object[] productVals = product.GetProductValuesArray();
            foreach (object val in productVals)
                dbConnection.CheckInput(val.ToString());
            dbConnection.UpdateTable("Products", "SystemID = '" + product.SystemId + "'", columnNames,
                new[] { "@idParam", "@name", "@price", "@desc" }, productVals);
        }

        public LinkedList<LotteryTicket> GetAllTickets(string systemid)
        {
            dbConnection.CheckInput(systemid);
            LinkedList<LotteryTicket> result = new LinkedList<LotteryTicket>();
            using (var dbReader = dbConnection.SelectFromTableWithCondition("LotteryTicket", "*", "LotteryID = '" + systemid + "'"))
            {
                while (dbReader.Read())
                {
                    LotteryTicket lottery = new LotteryTicket(dbReader.GetString(0), dbReader.GetString(1),
                        dbReader.GetDouble(2), dbReader.GetDouble(3), dbReader.GetDouble(4), dbReader.GetInt32(6));
                    lottery.myStatus = EnumStringConverter.GetLotteryStatusString(dbReader.GetString(5));
                    result.AddLast(lottery);
                }
            }
            return result;
        }

        private PurchaseHistory[] GetPurchaseHistory(SqlDataReader dbReader)
        {
            var historyData = new List<PurchaseHistory>();
            while (dbReader.Read())
            {
                historyData.Add(new PurchaseHistory(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetString(2),
                    dbReader.GetString(3), dbReader.GetInt32(4), dbReader.GetDouble(5), dbReader.GetDateTime(6).ToString()));
            }

            return historyData.ToArray();
        }

        public StockListItem GetStockListItembyProductID(string productid)
        {
            dbConnection.CheckInput(productid);
            Product product = GetProductID(productid);
            StockListItem stockListItem = null;
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Stock", "*", "ProductSystemID = '" + productid + "'"))
            {
                while (dbReader.Read())
                {
                    stockListItem = new StockListItem(dbReader.GetInt32(2), product,
                        GetDiscount(dbReader.GetString(3)), EnumStringConverter.GetPurchaseEnumString(dbReader.GetString(4)),
                        dbReader.GetString(0));
                    return stockListItem;
                }
            }

            return stockListItem;
        }

        public Discount GetDiscount(string discountCode)
        {
            dbConnection.CheckInput(discountCode);
            Discount discount = null;
            using (var discountReader =
                dbConnection.SelectFromTableWithCondition("Discount", "*", "DiscountCode = '" + discountCode + "'"))
            {
                while (discountReader.Read())
                {
                    discount = new Discount(discountCode,
                        EnumStringConverter.GetdiscountTypeEnumString(discountReader.GetString(1)),
                        discountReader.GetDateTime(2)
                        , discountReader.GetDateTime(3)
                        , discountReader.GetInt32(4),
                        Boolean.Parse(discountReader.GetString(5)));
                }
            }

            return discount;
        }
        
        public bool IsStoreExistAndActive(string store)
        {
            dbConnection.CheckInput(store);
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("Store", "*", " Name = '" + store + "' AND Status = 'Active'"))
            {
                return dbReader.Read();
            }
        }
       
        public void AddStore(Store store)
        {
            object[] storeVals = store.GetStoreArray();
            foreach (object val in storeVals)
                dbConnection.CheckInput(val.ToString());

            dbConnection.InsertTable("Store", "SystemID, Name, Address, Status",
                new []{"@idParam","@nameParam","@addressParam","@statParam"}, storeVals);
        }

        public LotteryTicket GetLotteryTicket(string ticketid)
        {
            dbConnection.CheckInput(ticketid);
            using (var dbReader = dbConnection.SelectFromTableWithCondition("LotteryTicket", "*", "myID = '" + ticketid + "'"))
            {
                while (dbReader.Read())
                {
                    LotteryTicket lotty = new LotteryTicket(dbReader.GetString(0), dbReader.GetString(1),
                        dbReader.GetDouble(2), dbReader.GetDouble(3), dbReader.GetDouble(4), dbReader.GetInt32(6));
                    lotty.myStatus = EnumStringConverter.GetLotteryStatusString(dbReader.GetString(5));
                    return lotty;
                }
            }

            return null;
        }

        public void RemoveLotteryTicket(LotteryTicket lottery)
        {
            dbConnection.CheckInput(lottery.myID);
            dbConnection.DeleteFromTable("LotteryTicket", "myID = '" + lottery.myID + "'");
        }

        public void AddLotteryTicket(LotteryTicket ticket)
        {
            object[] ticketVals = ticket.GetTicketValuesArray();
            foreach (object val in ticketVals)
                dbConnection.CheckInput(val.ToString());

            dbConnection.InsertTable("LotteryTicket", "myID, LotteryID, IntervalStart, IntervalEnd,Cost, Status, UserID",
                new[] { "@idParam", "@lotteryParam", "@interStart", "@interEnd", "@cost", "@stat", "@user" },
                ticketVals);
        }

        public Product GetProductID(string iD)
        {
            dbConnection.CheckInput(iD);
            Product product = null;
            using (var productReader = dbConnection.SelectFromTableWithCondition("Products", "*", "SystemID = '" + iD + "'"))
            {
                while (productReader.Read())
                {
                    product = new Product(iD, productReader.GetString(1), productReader.GetDouble(2),
                        productReader.GetString(3));
                    product.Categories = GetAllCategoriesOfProduct(iD);
                }
            }

            return product;

        }

        public string[] GetHistory(Store store)
        {
            dbConnection.CheckInput(store.Name);
            string[] result;
            using (var dbReader = dbConnection.SelectFromTableWithCondition("PurchaseHistory", "*", "Store = '" + store.Name + "'"))
            {
                PurchaseHistory[] resultPurchase = GetPurchaseHistory(dbReader);
                result = new string[resultPurchase.Length];
                int i = 0;
                foreach (PurchaseHistory purchaseHistory in resultPurchase)
                {
                    result[i] = purchaseHistory.ToString();
                    i++;
                }
            }
            return result;
        }

        public void AddDiscount(Discount discount)
        {
            object[] discountVals = discount.GetDiscountValuesArray();
            foreach (object val in discountVals)
                dbConnection.CheckInput(val.ToString());

            dbConnection.InsertTable("Discount", "DiscountCode, DiscountType, StartDate, EndDate, DiscountAmount,Percentages ",
                new []{"@codeParam","@typeParam","@startParam","@endParam","@amountParam","@percentParam"}
                , discountVals);
        }


        public void AddStockListItemToDataBase(StockListItem stockListItem)
        {
            object[] itemVals = stockListItem.GetStockListItemArray();
            foreach (object val in itemVals)
                dbConnection.CheckInput(val.ToString());

            if (stockListItem.Discount != null)
            {
                AddDiscount(stockListItem.Discount);
            }

            AddProductToDatabase(stockListItem.Product);
            dbConnection.InsertTable("Stock", "StockID, ProductSystemID, quantity, discount, PurchaseWay",
                new []{"@idParam","@productIdParam","@quantParam","@discountParam","@purchParam"}, itemVals);
        }



        public void RemoveLottery(LotterySaleManagmentTicket lotteryManagment)
        {
            dbConnection.CheckInput(lotteryManagment.SystemID);
            dbConnection.DeleteFromTable("LotteryTable", "SystemID = '" + lotteryManagment.SystemID + "'");
        }

        public void RemoveStockListItem(StockListItem stockListItem)
        {
            if (stockListItem.PurchaseWay == PurchaseEnum.Lottery)
            {
                var lsmt = GetLotteryByProductID(stockListItem.Product.SystemId);
                if (lsmt != null)
                    RemoveLottery(lsmt);
            }
            if (stockListItem.Discount != null)
            {
                RemoveDiscount(stockListItem.Discount);
            }

            RemoveProduct(stockListItem
                .Product); // the DB will delete the StockListItem due to the conection between of the 2 tables
        }

        public void EditDiscountInDatabase(Discount discount)
        {
            string[] columnNames =
            {
                "DiscountCode",
                "DiscountType",
                "StartDate",
                "EndDate",
                "DiscountAmount",
                "Percentages"
            };
            object[] discountVals = discount.GetDiscountValuesArray();
            foreach (object val in discountVals)
                dbConnection.CheckInput(val.ToString());

            dbConnection.UpdateTable("Discount", "DiscountCode = '" + discount.discountCode + "'", columnNames,
                new[] { "@codeParam", "@typeParam", "@startParam", "@endParam", "@amountParam", "@percentParam" }
                , discountVals);
        }

        public void EditStore(Store store)
        {

            string[] columnNames =
            {
                "SystemID",
                "Name",
                "Address",
                "Status",
            };
            object[] storeVals = store.GetStoreArray();
            foreach (object val in storeVals)
                dbConnection.CheckInput(val.ToString());

            dbConnection.UpdateTable("Store", "SystemID = '" + store.SystemId + "'", columnNames,
                new[] { "@idParam", "@nameParam", "@addressParam", "@statParam" }, storeVals);
        }




        public void RemoveProduct(Product product)
        {
            dbConnection.CheckInput(product.SystemId);
            dbConnection.DeleteFromTable("Products", "SystemID = '" + product.SystemId + "'");
        }

        public void EditStockInDatabase(StockListItem stockListItem)
        {
            string[] columnNames =
            {
                "StockID",
                "ProductSystemID",
                "quantity",
                "discount",
                "PurchaseWay"
            };
            object[] itemVals = stockListItem.GetStockListItemArray();
            foreach (object val in itemVals)
                dbConnection.CheckInput(val.ToString());

            dbConnection.UpdateTable("Stock", "ProductSystemID = '" + stockListItem.Product.SystemId + "'",
                columnNames, new[] { "@idParam", "@productIdParam", "@quantParam", "@discountParam", "@purchParam" },
                itemVals);
        }

        public void RemoveStore(Store store)
        {
            dbConnection.CheckInput(store.SystemId);
            dbConnection.DeleteFromTable("Store", "SystemID = '" + store.SystemId + "'");
        }

        public string[] GetStoreInfo(string store)
        {
            dbConnection.CheckInput(store);
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Store", "Name,Address",
                " Name = '" + store + "' AND Status = 'Active'"))
            {
                while (dbReader.Read())
                {
                    return new[] { dbReader.GetString(0), dbReader.GetString(1) };

                }
            }
            return null; 
        }
        public Product GetProductByNameFromStore(string storeName, string productName)
        {
            Store store = GetStorebyName(storeName);
            var productsid = GetAllStoreProductsID(store.SystemId);
            foreach (string id in productsid)
            {
                Product product = GetProductID(id);
                if (product.Name == productName)
                {
                    return product;
                }
            }

            return null;
        }
        public string[] GetStoreStockInfo(string store)
        {
            dbConnection.CheckInput(store);
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Stock", "Name,Address",
                " Store = '" + store + " AND Status = 'Active'"))
            {
                while (dbReader.Read())
                {
                    return new[] { dbReader.GetString(1), dbReader.GetString(2) };

                }
            }
            return null;
        }
        public StockListItem GetProductFromStore(string store, string productName)
        {
            Product product = GetProductByNameFromStore(store, productName);
            if (product == null) return null;
            return GetStockListItembyProductID(product.SystemId);

        }

        public void RemoveDiscount(Discount discount)
        {
            dbConnection.CheckInput(discount.discountCode);
            dbConnection.DeleteFromTable("Discount", "DiscountCode = '" + discount.discountCode + "'");
        }



        public void AddLottery(LotterySaleManagmentTicket lotteryManagment)
        {
            object[] lotteryVals = lotteryManagment.GetLotteryManagmentValuesArray();
            foreach (object val in lotteryVals)
                dbConnection.CheckInput(val.ToString());

            dbConnection.InsertTable("LotteryTable",
                "SystemID, ProductSystemID, ProductNormalPrice, TotalMoneyPayed, storeName ,StartDate,EndDate,IsActive ",
                new []{"@idParam","@prodIdParam","@prodPriceParam","@totalPriceParam","@storeParam","@startDateParam","@endDataParam","@activeParam"}
                , lotteryVals);

        }


        public string[] GetAllStoreProductsID(string systemid)
        {
            dbConnection.CheckInput(systemid);
            List<string> result = new List<string>();
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("Stock", "ProductSystemID", "StockID = '" + systemid + "'"))
            {
                while (dbReader.Read())
                {
                    result.Add(dbReader.GetString(0));
                }
            }

            return result.ToArray();
        }

        public LotterySaleManagmentTicket GetLotteryByProductNameAndStore(string storeName, string productName)
        {
            Product product = GetProductByNameFromStore(storeName, productName);
            return GetLotteryByProductID(product.SystemId);
        }

        public int GetUserIDFromUserName(string userName)
        {
            dbConnection.CheckInput(userName);
            using (var dbReader = dbConnection.SelectFromTableWithCondition("Users", "SystemID", "Name = '" + userName + "'"))
            {
                while (dbReader.Read())
                {
                    return dbReader.GetInt32(0);
                }

                return -1;
            }

        }

        public LotterySaleManagmentTicket GetLotteryByProductID(string productid)
        {
            dbConnection.CheckInput(productid);
            Product product = GetProductID(productid);
            LotterySaleManagmentTicket lotteryManagement = null;
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("LotteryTable", "*", "ProductSystemID = '" + productid + "'"))
            {
                while (dbReader.Read())
                {
                    lotteryManagement = new LotterySaleManagmentTicket(dbReader.GetString(0), dbReader.GetString(4), product,
                        dbReader.GetDateTime(5), dbReader.GetDateTime(6));
                    lotteryManagement.TotalMoneyPayed = dbReader.GetDouble(3);

                    lotteryManagement.IsActive = (bool.Parse(dbReader.GetString(7)));
                }
            }

            return lotteryManagement;
        }
        public void EditLotteryInDatabase(LotterySaleManagmentTicket lotteryManagment)
        {
            string[] columnNames =
            {
                "SystemID",
                "ProductSystemID",
                "ProductNormalPrice",
                "TotalMoneyPayed",
                "storeName",
                "StartDate",
                "EndDate",
                "IsActive"
            };
            object[] lotteryVals = lotteryManagment.GetLotteryManagmentValuesArray();
            foreach (object val in lotteryVals)
                dbConnection.CheckInput(val.ToString());

            dbConnection.UpdateTable("LotteryTable", "SystemID = '" + lotteryManagment.SystemID + "'", columnNames,
                new[] { "@idParam", "@prodIdParam", "@prodPriceParam", "@totalPriceParam", "@storeParam", "@startDateParam", "@endDataParam", "@activeParam" }
                , lotteryVals);
        }
        public Category GetCategoryByName(string categoryname)
        {
            dbConnection.CheckInput(categoryname);
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("Category", "*", "name = '" + categoryname + "'"))
            {
                while (dbReader.Read())
                {
                    return new Category(dbReader.GetString(0), dbReader.GetString(1));
                }
            }
            return null;
        }

        public Category GetCategoryByID(string categoryid)
        {
            dbConnection.CheckInput(categoryid);
            Category category = null;
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("Category", "*", "SystemID = '" + categoryid + "'"))
            {
                while (dbReader.Read())
                {
                    category = new Category(dbReader.GetString(0), dbReader.GetString(1));
                }
            }
            return category;
        }

        public string GetCategoryName(string categoryid)
        {
            dbConnection.CheckInput(categoryid);
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("Category", "name", "SystemID = '" + categoryid + "'"))
            {
                while (dbReader.Read())
                {
                    return dbReader.GetString(0);
                }
            }

            return null;
        }

        public LinkedList<Product> GetAllCategoryProducts(string categoryid)
        {
            dbConnection.CheckInput(categoryid);
            LinkedList<Product> products = new LinkedList<Product>();
            LinkedList<string> productids = new LinkedList<string>();
            Product product;
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("CategoryProductConnection", "ProductID", "CategoryID = '" + categoryid + "'"))
            {
                while (dbReader.Read())
                {
                    productids.AddLast(dbReader.GetString(0));
                }
            }
            foreach (string id in productids)
            {
                product = GetProductID(id);
                products.AddLast(product);
            }
            return products;
        }

        private List<string> GetAllCategoriesOfProduct(string productID)
        {
            dbConnection.CheckInput(productID);
            List<string> categories = new List<string>();
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("CategoryProductConnection", "CategoryID", "ProductID = '" + productID + "'"))
            {
                while (dbReader.Read())
                {
                    categories.Add(GetCategoryName(dbReader.GetString(0)));
                }
            }

            return categories;
        }

        public string[] GetAllCategorysIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("Category", "SystemID"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids.ToArray();
        }


	    public string[] GetAllCategorysNames()
	    {
		    LinkedList<string> ids = new LinkedList<string>();
		    using (var dbReader = dbConnection.SelectFromTable("Category", "name"))
		    {
			    while (dbReader.Read())
			    {
				    ids.AddLast(dbReader.GetString(0));
			    }
		    }
		    return ids.ToArray();
		}

	    public Product[] GetAllProducts()
	    {
		    LinkedList<Product> products = new LinkedList<Product>();
		    using (var dbReader = dbConnection.SelectFromTable("Products", "*"))
		    {
			    while (dbReader.Read())
			    {
				    products.AddLast(new Product(dbReader.GetString(0), dbReader.GetString(1), dbReader.GetDouble(2),
					    dbReader.GetString(3)));
			    }
		    }
		    return products.ToArray();
		}
        public CategoryDiscount GetCategoryDiscount(string categoryName, string storeName)
        {
            dbConnection.CheckInput(categoryName); dbConnection.CheckInput(storeName);
            CategoryDiscount categoryDiscount = null;
            using (var dbReader =
                dbConnection.SelectFromTableWithCondition("CategoryDiscount", "*",
                    "CategoryName = '" + categoryName + "' AND StoreName = '" + storeName + "'"))
            {
                while (dbReader.Read())
                {
                    categoryDiscount = new CategoryDiscount(dbReader.GetString(0),
                        dbReader.GetString(1),
                        dbReader.GetString(2),
                        dbReader.GetDateTime(3)
                        , dbReader.GetDateTime(4)
                        , dbReader.GetInt32(5)
                        );
                }
            }

            return categoryDiscount;
        }

        public void AddCategoryDiscount(CategoryDiscount categorydiscount)
        {
            object[] discountVals = categorydiscount.GetDiscountValuesArray();
            foreach (object val in discountVals)
                dbConnection.CheckInput(val.ToString());

            dbConnection.InsertTable("CategoryDiscount", "SystemID, CategoryName, StoreName, StartDate, EndDate, DiscountAmount",
                new[] { "@idParam", "@categoryParam", "@storeParam", "@startParam", "@endParam", "@amountParam"}
                , discountVals);
        }

        public void RemoveCategoryDiscount(CategoryDiscount categoryDiscount)
        {
            dbConnection.CheckInput(categoryDiscount.SystemId);
            dbConnection.DeleteFromTable("CategoryDiscount", "SystemId = '" + categoryDiscount.SystemId + "'");
        }

        public void EditCategoryDiscount(CategoryDiscount categoryDiscount)
        {
            string[] columnNames =
            {
                "SystemID",
                "CategoryName",
                "StoreName",
                "StartDate",
                "EndDate",
                "DiscountAmount",
            };
            object[] discountVals = categoryDiscount.GetDiscountValuesArray();
            foreach (object val in discountVals)
                dbConnection.CheckInput(val.ToString());

            dbConnection.UpdateTable("CategoryDiscount", "SystemId = '" + categoryDiscount.SystemId + "'", columnNames,
                new[] { "@idParam", "@categoryParam", "@storeParam", "@startParam", "@endParam", "@amountParam" }
                , discountVals);
        }

        public string[] GetAllCategoryDiscountIDs()
        {
            LinkedList<string> ids = new LinkedList<string>();
            using (var dbReader = dbConnection.SelectFromTable("CategoryDiscount", "SystemID"))
            {
                while (dbReader.Read())
                {
                    ids.AddLast(dbReader.GetString(0));
                }
            }
            return ids.ToArray();
        }

	    public string[] GetCategoriesWhichHaveDiscounts(string storeName)
	    {
		    LinkedList<string> categories = new LinkedList<string>();
			using (var dbReader =
				dbConnection.SelectFromTableWithCondition("CategoryDiscount", "CategoryName", "StoreName = '" + storeName + "'"))
			{
			    while (dbReader.Read())
			    {
				    categories.AddLast(dbReader.GetString(0));
			    }
		    }

		    return categories.ToArray();
	    }
    }
}