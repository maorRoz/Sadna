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
		MarketAnswer AddNewProduct(string name, int price, string description, int quantity);

		MarketAnswer RemoveProduct(string productName);
		MarketAnswer EditProduct(string productName, string productNewName, string basePrice, string description);

		MarketAnswer AddNewLottery(string _name, double _price, string _description, DateTime startDate,
	        DateTime endDate);
        MarketAnswer AddQuanitityToProduct(string productName, int quantity);
		MarketAnswer CloseStore();
		MarketAnswer AddDiscountToProduct(string productName, DateTime startDate, DateTime endDate,
			int discountAmount, string discountType, bool presenteges);

	    MarketAnswer EditDiscount(string product, string discountCode, bool isHidden, string startDate, string EndDate,
	        string discountAmount, bool isPercentage);

        MarketAnswer RemoveDiscountFromProduct(string productName);
        MarketAnswer ViewStoreHistory();

	    MarketAnswer ViewPromotionHistory();
	}
}
