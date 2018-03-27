using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.UserSpot;
using SadnaSrc.Main;

namespace BlackBox
{
	public class RealBridge : UserBridge
	{
		public IUserService userServie;

		public RealBridge()
		{
			var marketSession = new MarketYard();
			userServie = marketSession.GetUserService();
		}

		public string EnterSystem()
		{
			return userServie.EnterSystem();
		}

		public string SignUp(string name, string address, string password)
		{
			return userServie.SignUp(name, address, password);
		}
	}
}
