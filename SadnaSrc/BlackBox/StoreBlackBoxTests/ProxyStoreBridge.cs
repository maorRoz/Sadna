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
				real.GetStoreManagementService(userService, store);
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

		public MarketAnswer AddNewProduct(string _name, int _price, string _description, int quantity)
		{
			if (real != null)
			{
				return real.AddNewProduct(_name, _price, _description, quantity);
			}
			throw new NotImplementedException();
		}

		public MarketAnswer RemoveProduct(string productName)
		{
			if (real != null)
			{
				return real.RemoveProduct(productName);
			}
			throw new NotImplementedException();
		}

		public MarketAnswer EditProduct(string productName, string whatToEdit, string newValue)
		{
			if (real != null)
			{
				return real.EditProduct(productName, whatToEdit, newValue);
			}
			throw new NotImplementedException();
		}

		public MarketAnswer AddQuanitityToProduct(string productName, int quantity)
		{
			if (real != null)
			{
				return real.AddQuanitityToProduct(productName, quantity);
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
