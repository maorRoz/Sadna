namespace BlackBoxUserTests
{
	public class ProxyBridge : UserBridge
	{
		public UserBridge real;

		public string EnterSystem()
		{
			return real != null ? real.EnterSystem() : "";
		}

		public string SignUp(string name, string address, string password)
		{
			return real != null ? real.SignUp(name, address, password) : "";
		}
	}
}
