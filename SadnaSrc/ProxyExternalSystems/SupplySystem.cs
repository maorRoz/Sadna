using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyExternalSystems
{
    public class SupplySystem
    {
        private bool ans = true;
        public bool ProcessDelivery(int orderId, string username, string address)
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
