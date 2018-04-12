using System;
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

	    public void CleanSession()
	    {
            real?.CleanSession();
	    } 


	}
}
