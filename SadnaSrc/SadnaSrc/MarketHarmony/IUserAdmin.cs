using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketHarmony
{
    public interface IUserAdmin
    {
        bool IsSystemAdmin();

        int GetAdminSystemID();
    }
}
