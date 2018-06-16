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
				CheckIfStoreExistsAndActive(storeName);
				_shopper.ValidateCanBrowseMarket();
				MarketLog.Log("StoreCenter", "User has enetered the system!");
				string[] categories = _storeLogic.GetCategoriesWhichHaveDiscounts(storeName);
				Answer = new StoreAnswer(GetCategoriesDiscountStatus.Success, "All categories names have been granted!", categories);
			}
			catch (StoreException e)
			{
				MarketLog.Log("StoreCenter", "");
				Answer = new StoreAnswer((GetCategoriesDiscountStatus) e.Status, e.GetErrorMessage());
			}

			catch (DataException e)
			{
				Answer = new StoreAnswer((StoreEnum) e.Status, e.GetErrorMessage());
			}

			catch (MarketException)
			{
				MarketLog.Log("StoreCenter", "no premission");
				Answer = new StoreAnswer(GetCategoriesDiscountStatus.DidntEnterSystem,
					"User didn't enter the system!");
			}
		}

		private void CheckIfStoreExistsAndActive(string _storename)
		{
			if (!_storeLogic.IsStoreExistAndActive(_storename))
			{ throw new StoreException(GetCategoriesDiscountStatus.NoStore, "store not exists or active"); }
		}
	}
}