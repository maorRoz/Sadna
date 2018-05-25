using System;
using System.Collections.Generic;
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

		public void SearchProduct(string type, string value, int minPrice, int maxPrice, string category)
		{
			try
			{
				MarketLog.Log("StoreCenter", "searching for a product");
				_shopper.ValidateCanBrowseMarket();
				validateData(value);
				string[] products = null;
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
				Answer = new StoreAnswer(SearchProductStatus.Success,"Data retrieved successfully!",products);
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
	}
}

		

		