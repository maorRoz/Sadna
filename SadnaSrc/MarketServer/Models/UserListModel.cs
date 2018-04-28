using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketServer.Models;

namespace MarketWeb.Models
{
    public class UserListModel : UserModel
    {
        public UserListModel(int systemId, string state, string message) : base(systemId, state, message)
        {

        }
    }
}
