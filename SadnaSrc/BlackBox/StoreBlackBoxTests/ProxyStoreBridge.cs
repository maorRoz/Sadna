using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace BlackBox
{
	class ProxyStoreBridge : IStoreBridge
	{
		public IStoreBridge real;

		public void GetStoreShoppingService(IUserService userService)
		{
			if (real != null)
			{
				real.GetStoreShoppingService(userService);
			}
			else
			{
				throw new NotImplementedException();
			}
		}

		public void GetStoreManagementService(IUserService userService, string store)
		{
			if (real != null)
			{
				real.GetStoreManagementService(userService,store);
			}
			else
			{
				throw new NotImplementedException();
			}

		}

		public MarketAnswer OpenStore(string name, string address)
		{
			if (real != null)
			{
				return real.OpenStore(name, address);
			}
			throw new NotImplementedException();
		}

		public MarketAnswer ViewStoreInfo(string store)
		{
			if (real != null)
			{
				return real.ViewStoreInfo(store);
			}

			throw new NotImplementedException();
		}

		public MarketAnswer PromoteToStoreManager(string someoneToPromoteName, string actions)
		{
			if (real != null)
			{
				return real.PromoteToStoreManager(someoneToPromoteName, actions);
			}
			throw new NotImplementedException();
		}


		public void CleanSession()
	    {
		    if (real != null)
		    {
			    real.CleanSession();
		    }
		    else
		    {
				throw new NotImplementedException();
			}
	    } 


	}
}
