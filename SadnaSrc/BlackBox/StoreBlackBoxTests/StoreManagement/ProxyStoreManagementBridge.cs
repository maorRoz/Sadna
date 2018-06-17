using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace BlackBox
{
    class ProxyStoreManagementBridge : IStoreManagementBridge
    {
        public IStoreManagementBridge real;

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

        public MarketAnswer EditProduct(string productName, string productNewName, string basePrice, string description)
		{
            if (real != null)
            {
                return real.EditProduct(productName, productNewName, basePrice, description);
            }
            throw new NotImplementedException();
        }
        public MarketAnswer AddNewLottery(string _name, double _price, string _description, DateTime startDate,
            DateTime endDate)
        {
            if (real != null)
            {
                return real.AddNewLottery(_name, _price,_description, startDate, endDate);
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

        public MarketAnswer AddDiscountToProduct(string productName, DateTime startDate, DateTime endDate,
            int discountAmount, string discountType, bool presenteges)
        {
            if (real != null)
            {
                return real.AddDiscountToProduct(productName, startDate, endDate, discountAmount,
                    discountType, presenteges);
            }

            throw new NotImplementedException();
        }

        public MarketAnswer ViewStoreHistory()
        {
            if (real != null)
            {
                return real.ViewStoreHistory();
            }

            throw new NotImplementedException();
        }

        public MarketAnswer ViewPromotionHistory()
        {
            if (real != null)
            {
                return real.ViewPromotionHistory();
            }

            throw new NotImplementedException();
        }

        public MarketAnswer EditDiscount(string product, string discountCode, bool isHidden, string startDate, string EndDate, string discountAmount, bool isPercentage)
        {
            if (real != null)
            {
                return real.EditDiscount(product, discountCode, isHidden, startDate, EndDate, discountAmount, isPercentage);
            }

            throw new NotImplementedException();
        }

        public MarketAnswer RemoveDiscountFromProduct(string productName)
        {
            if (real != null)
            {
                return real.RemoveDiscountFromProduct(productName);
            }

            throw new NotImplementedException();
        }

        public MarketAnswer CloseStore()
        {
            if (real != null)
            {
                return real.CloseStore();
            }
            throw new NotImplementedException();
        }

    }
}