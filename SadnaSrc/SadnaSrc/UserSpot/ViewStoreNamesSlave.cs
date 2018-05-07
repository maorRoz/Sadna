using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.UserSpot;

namespace SadnaSrc.StoreCenter
{
	public class ViewStoreNamesSlave
	{
		public MarketAnswer answer;
	    private IStoreDL storeLogic;

		public ViewStoreNamesSlave(IStoreDL storeDl)
		{
		
		    storeLogic = storeDl;
		}

		public void ViewStores()
		{
            try
			{
                var storeNames = UserDL.Instance.GetAllActiveStoreNames();
				answer = new StoreAnswer(StoreEnum.Success, "you've got all the store names!", storeNames);
			}
			catch (StoreException e)
			{
				answer = new StoreAnswer(e);
			}
			catch (MarketException)
			{
				MarketLog.Log("StoreCenter", "no premission");
				answer = new StoreAnswer(StoreEnum.NoPermission,
					"User validation as valid customer has been failed . only valid users can browse market. Error message has been created!");
			}
		}

	}
}

