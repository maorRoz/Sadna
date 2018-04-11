namespace BlackBox
{
	class AdminDriver
	{
		public static IAdminBridge getBridge()
		{
			ProxyAdminBridge bridge = new ProxyAdminBridge();
			bridge.real = new RealAdminBridge();
			return bridge;
		}
	}
}
