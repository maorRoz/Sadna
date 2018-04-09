using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackBox
{
	class Driver
	{
		public static IUserBridge getBridge()
		{
			ProxyBridge bridge = new ProxyBridge();
			bridge.real = new RealBridge();
			return bridge;
		}
	}
}
