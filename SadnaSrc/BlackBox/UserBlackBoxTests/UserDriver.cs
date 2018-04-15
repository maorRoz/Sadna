using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackBox
{
    class UserDriver
    {
        public static IUserBridge getBridge()
        {
            ProxyUserBridge bridge = new ProxyUserBridge();
            bridge.real = new RealUserBridge();
            return bridge;
        }
    }
}