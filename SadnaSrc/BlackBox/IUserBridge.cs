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
		MarketAnswer SignUp(string name, string address, string password);
		MarketAnswer SignIn(string name, string password);
		MarketAnswer ViewCart();
		MarketAnswer EditCartItem(string store, string product, double unitPrice, string sale, int quantity);
		MarketAnswer RemoveFromCart(string store, string product, double unitPrice, string sale);
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
		void GetStoreService();
		MarketAnswer OpenStore(string name, string store);
	}
}
