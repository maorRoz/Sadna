﻿using System;
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

		public void SearchProduct(string value, double minPrice, double maxPrice, string category)
		{
			try
			{
				MarketLog.Log("StoreCenter", "searching for a product!");
				_shopper.ValidateCanBrowseMarket();
				MarketLog.Log("StoreCenter", "User enetred the system!");
				validatePrices(minPrice, maxPrice);
			    Product[] products;
				if (value.IsNullOrEmpty())
				{
					products = _storeLogic.GetAllProducts();
				}

				else
				{
					Product[] productsKeyWord = FindKeyWord(value);
					Product[] productsCategory = findProductsCategory(findSimilarCategories(value));
					List<Product> product = new List<Product>(productsKeyWord);
					foreach (Product prod in productsCategory)
					{
						product.Add(prod);
					}

					products = product.ToArray();
				}
					
				products = FilterResultsByPrice(products,minPrice, maxPrice);
				products = FilterResultByCategory(products, category);
				
				Answer = new StoreAnswer(SearchProductStatus.Success,"Data retrieved successfully!", AddStoreToProducts(products));
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

		private Product[] FilterResultsByPrice(Product[] products, double minPrice, double maxPrice)
		{
			List<Product> productsAfterFilter = new List<Product>();

			foreach (var product in products)
			{
				double price = product.BasePrice;
				if (minPrice != 0 && maxPrice == 0)
				{
					if (price >= minPrice)
					{
						productsAfterFilter.Add(product);
					}
				}

				else if (minPrice == 0 && maxPrice != 0)
				{
					if (price <= maxPrice)
					{
						productsAfterFilter.Add(product);
					}

				}

				else if (minPrice != 0 && maxPrice != 0)
				{
					if (price >= minPrice && price <= maxPrice)
					{
						productsAfterFilter.Add(product);
					}
				}

				else
				{
					productsAfterFilter.Add(product);
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

		private Category[] findSimilarCategories(string category)
		{
			LinkedList<Category> categories = new LinkedList<Category>();
			string[] allCategories = _storeLogic.GetAllCategorysNames();
			for (int i = 0; i < allCategories.Length; i++)
			{
				if (allCategories[i].Contains(category))
				{
					categories.AddLast(_storeLogic.GetCategoryByName(allCategories[i]));
				}
			}

			return categories.ToArray();
		}

		private Product[] findProductsCategory(Category[] categories)
		{
			List<Product> products = new List<Product>();
			foreach (Category cat in categories)
			{
				Product[] tempProducts = _storeLogic.GetAllCategoryProducts(cat.SystemId).ToArray();
				foreach (Product prod in tempProducts)
				{
					products.Add(prod);
				}
			}

			return products.ToArray();


		}

		private string[] AddStoreToProducts(Product[] products)
		{
			string[] result = new string[products.Length];
			string[] stores = GetProductsStores(products);
			for (int i = 0; i < result.Length; i++)
			{
				string productId = products[i].SystemId;
				result[i] = GetProductStockInformation(productId, false) + " Store: " + stores[i];
			}
			return result;
		}


	}
}

		

		