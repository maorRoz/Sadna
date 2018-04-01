using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.UserSpot;

namespace SadnaSrc.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            var marketSystem = MarketYard.Instance;
                  IUserService service = marketSystem.GetUserService();
                  service.EnterSystem();
                 service.SignIn("Arik1","123");
                  ISystemAdminService sysService= marketSystem.GetSystemAdminService(service);
                  sysService.RemoveUser(2);
            MarketYard.Exit();
        }
    }
}
