using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    class EnterSystemSlave
    {

        private readonly UserServiceDL userDL;
        public UserAnswer Answer { get; private set; }

        public EnterSystemSlave()
        {
            Answer = null;
        }
        public User EnterSystem()
        {
            MarketLog.Log("UserSpot", "New User attempting to enter the system...");
            int systemID = userDL.GetSystemID();
            MarketLog.Log("UserSpot", "User " + systemID + " has entered the system!");
            Answer = new UserAnswer(EnterSystemStatus.Success, "You've been entered the system successfully!");
            return new User(systemID);
        }
    }
}
