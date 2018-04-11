using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackBox.OrderBlackBoxTests
{
	class OrderDriver
	{
		public static IOrderBridge getBridge()
		{
			ProxyOrderBridge bridge = new ProxyOrderBridge();
			bridge.real = new RealOrderBridge();
			return bridge;
		}
	}
}
