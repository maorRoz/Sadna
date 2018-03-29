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
            var marketSystem = new MarketYard();
        //    IUserService service = marketSystem.GetUserService();
        //    service.EnterSystem();
       //     service.SignUp("Moshe","Here 4","123");
            marketSystem.Exit();
        }
    }
}
