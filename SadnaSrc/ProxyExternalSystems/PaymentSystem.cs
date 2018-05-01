using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyExternalSystems
{
    public class PaymentSystem
    {
        private static bool ans = true;


        private static PaymentSystem _instance;

        public static PaymentSystem Instance => _instance ?? (_instance = new PaymentSystem());

        private PaymentSystem()
        {

        }
        public bool ProccessPayment(string creditCardetails, double price)
        {
            return ans;
        }

        public void fuckUp()
        {
            ans = false;
        }

        public void fuckDown()
        {
            ans = true;
        }
    }
}
