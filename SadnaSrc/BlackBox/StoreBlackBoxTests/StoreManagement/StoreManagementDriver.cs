using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackBox
{
	class StoreManagementDriver
	{
		public static IStoreManagementBridge getBridge()
		{
			ProxyStoreManagementBridge bridge = new ProxyStoreManagementBridge();
			bridge.real = new RealStoreManagement();
			return bridge;
		}
	}
}
