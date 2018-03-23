using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    interface User
    {
        bool addUserPolicy(UserPolicy policy);
        bool removeUserPolicy(UserPolicy policy);
    }
}
