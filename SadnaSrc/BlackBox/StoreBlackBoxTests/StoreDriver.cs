using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackBox.StoreBlackBoxTests
{
	class StoreDriver
	{
		public static IStoreBridge getBridge()
		{
			ProxyStoreBridge bridge = new ProxyStoreBridge();
			bridge.real = new RealStoreBridge();
			return bridge;
		}
	}
}
