using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
	public class GetAllCategoryNamesSlave
	{
		public MarketAnswer Answer;
		private readonly IUserShopper _shopper;
		private readonly IStoreDL _storeLogic;

		public GetAllCategoryNamesSlave(IUserShopper shopper, IStoreDL storeDl)
		{
			_shopper = shopper;
			_storeLogic = storeDl;
		}

		public void GetAllCategoryNames()
		{
			try
			{
				_shopper.ValidateCanBrowseMarket();
				MarketLog.Log("StoreCenter", "User has enetered the system!");
				string[] categories = _storeLogic.GetAllCategorysNames();
				Answer = new StoreAnswer(GetCategoriesStatus.Success, "All categories names have been granted!", categories);
			}
			catch (StoreException e)
			{
				MarketLog.Log("StoreCenter", "");
				Answer = new StoreAnswer((GetCategoriesStatus)e.Status, e.GetErrorMessage());
			}

			catch (DataException e)
			{
			    Answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
			}

            catch (MarketException)
			{
				MarketLog.Log("StoreCenter", "no premission");
				Answer = new StoreAnswer(GetCategoriesStatus.DidntEnterSystem,
					"User didn't enter the system!");
			}
		}
	}
}
