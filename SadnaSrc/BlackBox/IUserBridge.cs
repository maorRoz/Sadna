using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace BlackBox
{
	interface IUserBridge
	{
		/*
		 * Regular user
		 */
		MarketAnswer EnterSystem();
		MarketAnswer SignUp(string name, string address, string password, string creditCard);
		MarketAnswer SignIn(string name, string password);
		MarketAnswer ViewCart();
		MarketAnswer EditCartItem(string store, string product, double unitPrice, int quantity);
		MarketAnswer RemoveFromCart(string store, string product, double unitPrice);
		void CleanSession();
		void CleanMarket();

		/*
		 * Admin system
		 */
		MarketAnswer RemoveUser(string userName);
		void GetAdminService();
		MarketAnswer ViewPurchaseHistoryByUser(string userName);
		MarketAnswer ViewPurchaseHistoryByStore(string storeName);

		/*
		 * StoreSystem
		 */
		void GetStoreShoppingService();
		MarketAnswer OpenStore(string name, string address);
	}
}
