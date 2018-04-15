using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace BlackBox
{
	interface IStoreManagementBridge
	{
		void GetStoreManagementService(IUserService userService, string store);
		MarketAnswer PromoteToStoreManager(string someoneToPromoteName, string actions);
		MarketAnswer AddNewProduct(string _name, int _price, string _description, int quantity);
		MarketAnswer RemoveProduct(string productName);
		MarketAnswer EditProduct(string productName, string whatToEdit, string newValue);
		MarketAnswer AddQuanitityToProduct(string productName, int quantity);
		MarketAnswer CloseStore();
		void CleanSession();
	}
}
