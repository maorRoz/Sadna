using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackBox.OrderBlackBoxTests
{
	class ProxyOrderBridge :IOrderBridge
	{
		public IOrderBridge real;
	}
}
