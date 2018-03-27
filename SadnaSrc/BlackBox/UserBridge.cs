using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackBox
{
	public interface UserBridge
	{
		string EnterSystem();
		string SignUp(string name, string address, string password);
	}
}
