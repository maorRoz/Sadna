using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarketServer.Models
{
    public class UserModel
    {
        public int SystemId { get; set; }
        public string State { get; set; }
        public UserModel(int systemId, string state)
        {
            SystemId = systemId;
            State = state;
        }
    }
}
