using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackBox
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
