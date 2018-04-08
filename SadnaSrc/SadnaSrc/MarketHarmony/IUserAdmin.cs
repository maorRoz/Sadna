using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketHarmony
{
    //integration between UserSpot to AdminView 
    public interface IUserAdmin
    {
        void ValidateSystemAdmin();

        int GetAdminSystemID();

        string GetAdminName();
    }
}
