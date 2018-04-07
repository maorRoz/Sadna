using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyExternalSystems
{
    public class PaymentSystem
    {
        private bool ans = true;
        public bool ProccessPayment(List<string> creditCardetails, double price)
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
