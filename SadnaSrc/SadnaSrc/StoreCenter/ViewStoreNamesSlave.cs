using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
	class ViewStoreNamesSlave
	{
		internal MarketAnswer answer;
		private IUserShopper _shopper;
	    private IStoreDL storeLogic;

		public ViewStoreNamesSlave(IUserShopper shopper,IStoreDL storeDl)
		{
			_shopper = shopper;
		    storeLogic = storeDl;
		}

		internal void ViewStores()
		{
            try
			{
			    _shopper.ValidateCanBrowseMarket();
                var storeNames = storeLogic.GetAllActiveStoreNames();
				answer = new StoreAnswer(StoreEnum.Success, "you've got all the store names!", storeNames);
			}
			catch (StoreException e)
			{
				answer = new StoreAnswer(e);
			}
			catch (MarketException)
			{
				MarketLog.Log("StoreCenter", "no premission");
				answer = new StoreAnswer(StoreEnum.NoPremmision,
					"User validation as valid customer has been failed . only valid users can browse market. Error message has been created!");
			}
		}

	}
}

