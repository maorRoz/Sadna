using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
	class ViewStoreSlave
	{
		internal MarketAnswer answer;
		private IUserShopper _shopper;
		StoreDL storeLogic;

		public ViewStoreSlave(IUserShopper shopper)
		{
			_shopper = shopper;
			storeLogic = StoreDL.GetInstance();
		}

		internal void ViewStores()
		{
			try
			{
				LinkedList<string> result = new LinkedList<string>();
				LinkedList<Store> IDS = storeLogic.GetAllActiveStores();
				foreach (Store item in IDS)
				{
					result.AddLast(((Store)item).Name);
				}
				string[] resultArray = new string[result.Count];
				result.CopyTo(resultArray, 0);
				answer = new StoreAnswer(StoreEnum.Success, "", resultArray);
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

