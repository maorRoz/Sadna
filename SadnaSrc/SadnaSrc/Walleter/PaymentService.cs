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
        private PaymentSystem sock;

       
        //TODO: change this once info about external systems is available.
        public void AttachExternalSystem()
        {
            sock = new PaymentSystem();
            MarketLog.Log("Walleter", "Connection to external payment system established successfully ! ");

        }

        public void ProccesPayment(Order order, string creditCardetails)
        {
            if (sock == null)
            {
                throw new WalleterException(WalleterStatus.NoPaymentSystem, "Failed, an error in the payment system occured.");
            }
            MarketLog.Log("Walleter", "Attempting to proccess payment for order ID: " + order.GetOrderID());
            if (CheckCreditCard(creditCardetails))
            {
                if (sock.ProccessPayment(creditCardetails, order.GetPrice()))
                {
                    MarketLog.Log("Walleter", "Payment for order ID: " + order.GetOrderID() + " was completed.");
                    return;
                }
                throw new WalleterException(WalleterStatus.PaymentSystemError, "Failed, an error in the payment system occured.");
            }
            throw new WalleterException(WalleterStatus.InvalidCreditCardSyntax, "Failed, Invalid credit card details..");

        }

        public void Refund(double sum, string creditCardetails,string username)
        {
            if (sock == null)
            {
                throw new WalleterException(WalleterStatus.NoPaymentSystem, "Failed, an error in the payment system occured.");
            }
            MarketLog.Log("Walleter", "Attempting to make a refund for user: " + username );
            if (CheckCreditCard(creditCardetails))
            {
                if (sock.ProccessPayment(creditCardetails,  -1 * sum))
                {
                    MarketLog.Log("Walleter", "Refund for user: "+ username + " was completed !");
                    return;
                }
                throw new WalleterException(WalleterStatus.PaymentSystemError, "Failed, an error in the payment system occured.");
            }
            throw new WalleterException(WalleterStatus.InvalidCreditCardSyntax, "Failed, Invalid credit card details..");

        }

        public bool CheckCreditCard(string details)
        {
            int x;
            return (details.Length == 8) && Int32.TryParse(details, out x);
        }

        public void BreakExternal()
        {
            sock.fuckUp();
        }
    }
}
