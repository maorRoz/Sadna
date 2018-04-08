using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProxyExternalSystems;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;

namespace SadnaSrc.Walleter
{
    public class PaymentService : IPaymentService
    {
        private readonly OrderService _orderService;
        private PaymentSystem sock;

        public PaymentService(OrderService orderService)
        {
            _orderService = orderService;
        }
        //TODO: you shouldn't let the client get any interaction with this interface, no MarketAnswer is needed here
        //TODO: change this once info about external systems is available.
        public MarketAnswer AttachExternalSystem()
        {
            sock = new PaymentSystem();
            return new WalleterAnswer(WalleterStatus.Success, "Success, External payment system attached.");

        }

        public MarketAnswer ProccesPayment(int orderId, string address, List<string> creditCardetails)
        {
            if (sock == null)
            {
                throw new WalleterException(WalleterStatus.NoPaymentSystem, "Failed, an error in the payment system occured.");
            }
            Order order = _orderService.GetOrder(orderId);
            if (CheckCreditCard(creditCardetails))
            {
                if (sock.ProccessPayment(creditCardetails, order.GetPrice()))
                {
                    MarketLog.Log("Walleter", "Payment for order ID: " + orderId + " was completed.");
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
                if (details.ElementAt(i).Length != 4 || !Int32.TryParse(details.ElementAt(i),out x) || x < 0)
                {
                    return false;
                }
            }

            if (details.ElementAt(4).Length != 2 || !Int32.TryParse(details.ElementAt(4), out x) || x > 12 || x < 0) return false;
            if (details.ElementAt(5).Length != 2 || !Int32.TryParse(details.ElementAt(5), out x) || x < 0) return false;
            if (details.ElementAt(6).Length != 3 || !Int32.TryParse(details.ElementAt(6), out x) || x < 0) return false;
            return true;
        }

        // Method for testing only WILL BE REMOVED
        public void FuckUpExternal()
        {
            sock.fuckUp();
        }
    }
}
