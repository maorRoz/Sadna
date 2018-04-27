using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SadnaSrc.AdminView;
using SadnaSrc.Main;

namespace MarketServer.Models
{
    public class UserModel
    {
        public int SystemId { get; set; }
        public string State { get; set; }

        public string Message { get; set; }

        public UserModel(int systemId, string state)
        {
            SystemId = systemId;
            State = state;
        }
        public UserModel(int systemId, string state, string message)
        {
            SystemId = systemId;
            State = state;
            Message = message;
        }


    }
}
