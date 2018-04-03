using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxyExternalSystems;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using SadnaSrc.UserSpot;

namespace SadnaSrc.Walleter
{
    public class PaymentService : IPaymentService
    {
        private readonly OrderService _orderService;
        private readonly PaymentSystem sock;

        public PaymentService(UserService userService, OrderService orderService)
        {
            _orderService = orderService;
            sock = new PaymentSystem();
        }
        public MarketAnswer ProccesPayment(int orderId, string address, List<string> creditCardetails)
        {
            Order order = _orderService.getOrder(orderId);
            if (CheckCreditCard(creditCardetails))
            {
                if (sock.ProccessPayment(creditCardetails, order.GetPrice()))
                {
                    return new WalleterAnswer(WalleterStatus.Success, "Success, payment proccess completed.");
                }
                throw new WalleterException(WalleterStatus.PaymentSystemError, "Failed, an error in the payment system occured.");
            }
            throw new WalleterException(WalleterStatus.InvalidCreditCardSyntax, "Failed, Invalid credit card details..");

        }

        public bool CheckCreditCard(List<string> details)
        {
            int x;
            for (int i = 0; i < 4; i++)
            {
                if (!Int32.TryParse(details[i],out x))
                {
                    return false;
                }
            }

            if (!Int32.TryParse(details[4], out x) || x > 12 || x < 0) return false;
            if (!Int32.TryParse(details[5], out x)) return false;
            if (!Int32.TryParse(details[6], out x)) return false;
            return true;
        }
    }
}
