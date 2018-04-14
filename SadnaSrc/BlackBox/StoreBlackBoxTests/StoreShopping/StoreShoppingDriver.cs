using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackBox
{
	class StoreShoppingDriver
	{
		public static IStoreShoppingBridge getBridge()
		{
			ProxyStoreShoppingBridge bridge = new ProxyStoreShoppingBridge();
			bridge.real = new RealStoreShoppingBridge();
			return bridge;
		}
	}
}
