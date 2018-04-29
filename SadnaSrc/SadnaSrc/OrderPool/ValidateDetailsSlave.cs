using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.SupplyPoint;
using SadnaSrc.Walleter;

namespace SadnaSrc.OrderPool
{
    public class ValidateDetailsSlave
    {

        public OrderAnswer Answer { get; private set; }

        public void GiveDetails(string userName, string address, string creditCard)
        {
            try
            {
                MarketLog.Log("OrderPool",
                    "User entering name and address for later usage in market order. validating data ...");
                IsValidUserDetails(userName, address, creditCard);
                MarketLog.Log("OrderPool",
                    "Validation has been completed. User name and address are valid and been updated");
                Answer = new OrderAnswer(GiveDetailsStatus.Success,
                    "User name and address has been updated successfully!");
            }
            catch (OrderException e)
            {
                Answer = new OrderAnswer(GiveDetailsStatus.InvalidNameOrAddress, e.GetErrorMessage());

            }

        }

        private void IsValidUserDetails(string userName, string address, string creditCard)
        {
            int x;
            if (userName == null || address == null || creditCard == null || creditCard.Length != 8 ||
                !Int32.TryParse(creditCard, out x))
            {
                MarketLog.Log("OrderPool", "User entered name or address which is invalid by the system standards!");
                throw new OrderException(GiveDetailsStatus.InvalidNameOrAddress,
                    "User entered invalid name or address into the order");
            }
        }
    }
}
