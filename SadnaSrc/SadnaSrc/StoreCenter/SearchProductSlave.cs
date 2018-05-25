using System;
using System.Collections.Generic;
using System.Linq;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

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

				products = filterResultsByPrice(products,minPrice, maxPrice);
				products = filterResultByCategory(products, category);
				string[] result = new string[products.Length];
				for (int i = 0; i < result.Length; i++)
				{
					result[i] = products[i].ToString();
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

		private Product[] filterResultsByPrice(Product[] products, double minPrice, double maxPrice)
		{
			LinkedList<Product> productsAfterFilter = new LinkedList<Product>();
			foreach (var product in products)
			{
				double productPrice = product.BasePrice;
				if (productPrice >= minPrice && productPrice <= maxPrice)
				{
					productsAfterFilter.AddLast(product);
				}
			}
			return productsAfterFilter.ToArray();
		}

		private Product[] filterResultByCategory(Product[] products, string category)
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

	}
}

		

		