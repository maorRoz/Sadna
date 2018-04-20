using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    public interface IUserDL
    {
        int[] GetAllSystemIDs();
        void SaveUser(User user);
    }
}
