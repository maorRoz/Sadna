using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyExternalSystems
{
    public class SupplySystem
    {
        private static bool ans = true;

        private static SupplySystem _instance;

        public static SupplySystem Instance => _instance ?? (_instance = new SupplySystem());

        private SupplySystem()
        {

        }
        public bool ProcessDelivery(int orderId, string username, string address)
        {
            return ans;
        }

        public static void FuckUp()
        {
            ans = false;
        }

        public static void FuckDown()
        {
            ans = true;
        }
    }
}
