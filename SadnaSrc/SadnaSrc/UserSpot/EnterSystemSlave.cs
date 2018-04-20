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

        private readonly UserServiceDL userDB;
        public UserAnswer Answer { get; private set; }

        public EnterSystemSlave()
        {
            userDB = UserServiceDL.Instance;
            Answer = null;
        }
        public User EnterSystem()
        {
            MarketLog.Log("UserSpot", "New User attempting to enter the system...");
            User newGuest  = new User(GenerateSystemID());
            MarketLog.Log("UserSpot", "User " + newGuest.SystemID + " has entered the system! " +
                                      "attempting to save the user entry...");
            userDB.SaveUser(newGuest);
            MarketLog.Log("UserSpot", "User " + newGuest.SystemID + " has been saved successfully as new " +
                                      "guest entry in the system!");
            Answer = new UserAnswer(EnterSystemStatus.Success, "You've been entered the system successfully!");
            return newGuest;
        }

        private int GenerateSystemID()
        {
            var random = new Random();
            var newID = random.Next(1000, 10000);
            int[] savedIDs = userDB.GetAllSystemIDs();
            while (savedIDs.Contains(newID))
            {
                newID = random.Next(1000, 10000);
            }

            return newID;
        }
    }
}
