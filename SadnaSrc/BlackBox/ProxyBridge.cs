using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace BlackBox
{
	class ProxyBridge : IUserBridge
	{
		public IUserBridge real;

		public ProxyBridge()
		{
			real = null;
		}

		public MarketAnswer EnterSystem()
		{
			if (real != null)
			{
				real.EnterSystem();
			}

			throw new NotImplementedException();
		}

		public MarketAnswer SignUp(string name, string address, string password)
		{
			if (real != null)
			{
				real.SignUp(name,address,password);
			}
			throw new NotImplementedException();
		}

		public MarketAnswer SignIn(string name, string password)
		{
			if (real != null)
			{
				real.SignIn(name, password);
			}
			throw new NotImplementedException();
		}

		public MarketAnswer ViewCart()
		{
			if (real != null)
			{
				real.ViewCart();
			}
		
			throw new NotImplementedException();
		}

		public MarketAnswer EditCartItem(string store, string product, double unitPrice, int quantity)
		{
			if (real != null)
			{
				real.EditCartItem(store, product, unitPrice, quantity);
			}
			throw new NotImplementedException();
		}

		public MarketAnswer RemoveFromCart(string store, string product, double unitPrice)
		{
			if (real != null)
			{
				real.RemoveFromCart(store, product, unitPrice);
			}
			throw new NotImplementedException();
		}

		public void CleanSession()
		{
			if (real != null)
			{
				real.CleanSession();
			}
			throw new NotImplementedException();
		}

		public void CleanMarket()
		{
			if (real != null)
			{
				real.CleanMarket();
			}
			throw new NotImplementedException();
		}

		public MarketAnswer RemoveUser(string userName)
		{
			if (real != null)
			{
				real.RemoveUser(userName);
			}
			throw new NotImplementedException();
		}

		public void GetAdminService()
		{
			if (real != null)
			{
				real.GetAdminService();
			}
			throw new NotImplementedException();
		}

		public MarketAnswer ViewPurchaseHistoryByUser(string userName)
		{
			if (real != null)
			{
				real.ViewPurchaseHistoryByUser(userName);
			}
			throw new NotImplementedException();
		}

		public MarketAnswer ViewPurchaseHistoryByStore(string storeName)
		{
			if (real != null)
			{
				real.ViewPurchaseHistoryByStore(storeName);
			}
			throw new NotImplementedException();
		}

		public void GetStoreService()
		{
			if (real != null)
			{
				real.GetStoreService();
			}
			throw new NotImplementedException();
		}

		public MarketAnswer OpenStore(string name, string store)
		{
			if (real != null)
			{
				real.OpenStore(name, store);
			}
			throw new NotImplementedException();
		}
	}
}
