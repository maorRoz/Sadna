﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace BlackBox
{
	interface IUserBridge
	{

		MarketAnswer EnterSystem();
		MarketAnswer SignUp(string name, string address, string password);
		MarketAnswer SignIn(string name, string password);
		void CleanSession();
		void CleanMarket();
	}
}
