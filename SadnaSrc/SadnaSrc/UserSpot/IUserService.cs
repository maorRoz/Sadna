using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    public interface IUserService
    {
        string EnterSystem();
        string SignUp(string name,string address,string password);

        //   string SignIn();

    }
}
