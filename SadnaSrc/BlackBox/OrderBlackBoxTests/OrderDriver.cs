using BlackBox.OrderBlackBoxTests;

namespace BlackBox
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
