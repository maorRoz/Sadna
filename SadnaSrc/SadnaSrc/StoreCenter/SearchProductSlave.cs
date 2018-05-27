using System;
using System.Collections.Generic;
using System.Linq;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;
using SadnaSrc.StoreCenter;

namespace SadnaSrc.StoreCenter
{
	public class SearchProductSlave
	{
		public MarketAnswer Answer;
		private readonly IUserShopper _shopper;
		private readonly IStoreDL _storeLogic;

		public SearchProductSlave(IUserShopper shopper, IStoreDL storeDl)
		{
			_shopper = shopper;
			_storeLogic = storeDl;
		}

		public void SearchProduct(string type, string value, double minPrice, double maxPrice, string category)
		{
			try
			{
				MarketLog.Log("StoreCenter", "searching for a product!");
				_shopper.ValidateCanBrowseMarket();
				MarketLog.Log("StoreCenter", "User enetred the system!");
				validateData(value);
				Product[] products = null;
				switch (type)
				{
					case "Name":
						products = _storeLogic.GetProductsByName(value);
						break;
					case "Category":
						
						break;
					case "KeyWord":
						
						break;
				}

				products = FilterResultsByPrice(products,minPrice, maxPrice);
				products = FilterResultByCategory(products, category);
				string[] result = new string[products.Length];
				string[] stores = GetProductsStores(products);
				for (int i = 0; i < result.Length; i++)
				{
					string productId = products[i].SystemId;
					result[i] = GetProductStockInformation(productId,false) + " Store: "+ stores[i];
				}

				Answer = new StoreAnswer(SearchProductStatus.Success,"Data retrieved successfully!", result);
			}

			catch (StoreException e)
			{
				Answer = new StoreAnswer((SearchProductStatus)e.Status, e.GetErrorMessage());
			}

			catch (MarketException)
			{
				MarketLog.Log("StoreCenter", "no premission");
				Answer = new StoreAnswer(SearchProductStatus.DidntEnterSystem,
					"User Didn't enter the system!");
			}

			catch (DataException e)
			{
				Answer = new StoreAnswer((SearchProductStatus) e.Status, e.GetErrorMessage());
			}	
		}


		private void validateData(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new StoreException(SearchProductStatus.NullValue,
					"The data given is null or empty!");
			}
		}

		private Product[] FilterResultsByPrice(Product[] products, double minPrice, double maxPrice)
		{
			LinkedList<Product> productsAfterFilter = new LinkedList<Product>();

			foreach (var product in products)
			{
				productsAfterFilter.AddLast(product);
			}

			if (maxPrice!=0)
			{
				foreach (var product2 in productsAfterFilter)
				{
					double product2Price = product2.BasePrice;
					if (product2Price > maxPrice)
					{
						productsAfterFilter.Remove(product2);
					}
				}

			}

			if (minPrice!=0)
				{
					foreach (var product2 in productsAfterFilter)
					{
						double product2Price = product2.BasePrice;
						if (product2Price < minPrice)
						{
							productsAfterFilter.Remove(product2);
						}
					}

			}
			return productsAfterFilter.ToArray();
		}

		private Product[] FilterResultByCategory(Product[] products, string category)
		{
			if (category != null)
			{
				LinkedList<Product> productsAfterFilter = new LinkedList<Product>();
				Category cat = _storeLogic.GetCategoryByName(category);
				LinkedList<Product> categoryProducts = _storeLogic.GetAllCategoryProducts(cat.SystemId);
				foreach (var product in products)
				{
					foreach (var categoryProduct in categoryProducts)
					{
						if (categoryProduct.Name == product.Name)
						{
							productsAfterFilter.AddLast(product);
						}
					}
				}
				return productsAfterFilter.ToArray();
			}

			return products;

		}

		private string[] GetProductsStores(Product[] products)
		{
			string[] stores = new string[products.Length];
			for (int i = 0; i < products.Length; i++)
			{
				string storeId = _storeLogic.GetStoreByProductId(products[i].SystemId);
				stores[i] = _storeLogic.GetStorebyID(storeId).Name;
			}

			return stores;
		}

		private string GetProductStockInformation(string productID, bool showAll)
		{
			StockListItem stockListItem = _storeLogic.GetStockListItembyProductID(productID);
			if (stockListItem == null)
			{
				MarketLog.Log("storeCenter", "product not exists");
				throw new StoreException(StoreEnum.ProductNotFound, "product " + productID + " does not exist in Stock");
			}
			if (stockListItem.PurchaseWay == PurchaseEnum.Lottery && !showAll)
			{
				LotterySaleManagmentTicket managmentTicket =
					_storeLogic.GetLotteryByProductID((productID));
				StockListItem sli = _storeLogic.GetStockListItembyProductID(productID);
				if ((managmentTicket.EndDate < MarketYard.MarketDate) ||
				    (managmentTicket.StartDate > MarketYard.MarketDate) ||
				    ((managmentTicket.TotalMoneyPayed == managmentTicket.ProductNormalPrice) && sli.Quantity == 0))
					return "";
			}
			string discount = " Discount: {";
			string product = stockListItem.Product.ToString();
			if (stockListItem.Discount != null)
				discount += stockListItem.Discount;
			else
			{
				discount += "null";
			}
			discount += "}";
			string purchaseWay = " Purchase Way: " + EnumStringConverter.PrintEnum(stockListItem.PurchaseWay);
			string quanitity = " Quantity: " + stockListItem.Quantity;
			string result = product + discount + purchaseWay + quanitity;
			return result;
		}

	}
}

		

		