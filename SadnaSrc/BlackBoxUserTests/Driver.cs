namespace BlackBoxUserTests
{
	public abstract class Driver
	{
		public static ProxyBridge getBridge()
		{
			ProxyBridge bridge = new ProxyBridge();
			bridge.real = new RealBridge();
			return bridge;
		}
	}
}
