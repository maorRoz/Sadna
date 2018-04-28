using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketServer.Models;

namespace MarketWeb.Models
{
    public class UserListModel : UserModel
    {
		string[] users;
        public UserListModel(int systemId, string state, string message, string[] userNames) : base(systemId, state, message)
        {
			users = new string[userNames.Length];
			for (int i = 0; i < users.Length; i++)
			{
				users[i] = userNames[i];
			}
        }
    }
}
