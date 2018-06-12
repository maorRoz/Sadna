using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
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
				validatePrices(minPrice, maxPrice);
				switch (type)
				{
					case "Name":
						products = _storeLogic.GetProductsByName(value);
						if (products.Length == 0)
						{
							string similarProduct = FindSimilarProductByName(value);
							if (similarProduct != "")
							{
								Answer = new StoreAnswer(SearchProductStatus.MistakeTipGiven, "Did you mean: " + similarProduct + "?");
								return;
							}
							
						}

						break;
					case "Category":
						Category cat = _storeLogic.GetCategoryByName(value);
						if (cat == null)
						{
							string similarProduct = FindSimilarCategoriesByName(value);
							if (similarProduct != "")
							{
								Answer = new StoreAnswer(SearchProductStatus.MistakeTipGiven, "Did you mean: " + similarProduct + "?");
								return;
							}

							Answer = new StoreAnswer(SearchProductStatus.CategoryNotFound, "Category wasn't found in the system!");
							return;
						}
						products = _storeLogic.GetAllCategoryProducts(cat.SystemId).ToArray();
						
						break;
					case "KeyWord":
						products = FindKeyWord(value);
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

			catch (DataException e)
			{
			    Answer = new StoreAnswer((SearchProductStatus)e.Status, e.GetErrorMessage());
			}

            catch (MarketException)
			{
				MarketLog.Log("StoreCenter", "no premission");
				Answer = new StoreAnswer(SearchProductStatus.DidntEnterSystem,
					"User Didn't enter the system!");
			}
	
		}

		private string FindSimilarCategoriesByName(string value)
		{
			string similarProduct = "";
			string[] caetgories = _storeLogic.GetAllCategorysNames();
			foreach (var category in caetgories)
			{
				if (MarketMistakeService.IsSimilar(value, category))
				{
					similarProduct = category;
				}
			}

			return similarProduct;
		}

		private string FindSimilarProductByName(string value)
		{
			string similarProduct = "";
			Product[] products = _storeLogic.GetAllProducts();
			foreach (var product in products)
			{
				if (MarketMistakeService.IsSimilar(value, product.Name))
				{
					similarProduct = product.Name;
				}
			}

			return similarProduct;
		}

		private Product[] FindKeyWord(string value)
		{
			LinkedList<Product> result = new LinkedList<Product>();
			Product[] products = _storeLogic.GetAllProducts();
			foreach (var product in products)
			{
				if (product.Name.Contains(value) || product.Description.Contains(value) || Convert.ToString(product.BasePrice)==value)
				{
					result.AddLast(product);
				}
			}

			return result.ToArray();

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
				double price = product.BasePrice;
				if (minPrice != 0 && maxPrice == 0)
				{
					if (price >= minPrice)
					{
						productsAfterFilter.AddLast(product);
					}
				}

				else if (minPrice == 0 && maxPrice != 0)
				{
					if (price <= maxPrice)
					{
						productsAfterFilter.AddLast(product);
					}

				}

				else if (minPrice != 0 && maxPrice != 0)
				{
					if (price >= minPrice && price <= maxPrice)
					{
						productsAfterFilter.AddLast(product);
					}
				}

				else
				{
					productsAfterFilter.AddLast(product);
				}

			}
			
			return productsAfterFilter.ToArray();
		}

		private Product[] FilterResultByCategory(Product[] products, string category)
		{
			if (category != "None")
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

		private string GetProductStockInformation(string productId, bool showAll)
		{
			StockListItem stockListItem = _storeLogic.GetStockListItembyProductID(productId);
			if (stockListItem == null)
			{
				MarketLog.Log("storeCenter", "product not exists");
				throw new StoreException(StoreEnum.ProductNotFound, "product " + productId + " does not exist in Stock");
			}
			if (stockListItem.PurchaseWay == PurchaseEnum.Lottery && !showAll)
			{
				LotterySaleManagmentTicket managmentTicket =
					_storeLogic.GetLotteryByProductID((productId));
				StockListItem sli = _storeLogic.GetStockListItembyProductID(productId);
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
				discount += "none";
			}
			discount += "}";
			string purchaseWay = " Purchase Way: " + EnumStringConverter.PrintEnum(stockListItem.PurchaseWay);
			string quanitity = " Quantity: " + stockListItem.Quantity;
			string result = product + discount + purchaseWay + quanitity;
			return result;
		}

		private void validatePrices(double minPrice, double maxPrice)
		{
			if (minPrice<0 || maxPrice<0 || (minPrice>maxPrice && maxPrice!=0))
			{
				throw new StoreException(SearchProductStatus.PricesInvalid,
					"The prices range is illegal!!");
			}
		}


	}
}

		

		