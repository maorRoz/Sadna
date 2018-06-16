using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
	public class GetAllDiscountCategoriesInStoreSlave
	{
		public MarketAnswer Answer;
		private readonly IUserShopper _shopper;
		private readonly IStoreDL _storeLogic;

		public GetAllDiscountCategoriesInStoreSlave(IUserShopper shopper, IStoreDL storeDl)
		{
			_shopper = shopper;
			_storeLogic = storeDl;
		}

		public void GetAllDiscountCategoriesNameInStore(string storeName)
		{
			try
			{
				_shopper.ValidateCanBrowseMarket();
				MarketLog.Log("StoreCenter", "User has enetered the system!");
				string[] categories = _storeLogic.GetCategoriesWhichHaveDiscounts(storeName);
				Answer = new StoreAnswer(GetCategoriesStatus.Success, "All categories names have been granted!", categories);
			}
			catch (StoreException e)
			{
				MarketLog.Log("StoreCenter", "");
				Answer = new StoreAnswer((GetCategoriesStatus) e.Status, e.GetErrorMessage());
			}

			catch (DataException e)
			{
				Answer = new StoreAnswer((StoreEnum) e.Status, e.GetErrorMessage());
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