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
        /// <summary>
        /// Validate that the user has been entered the system and is a system admin
        /// </summary>
        void ValidateSystemAdmin();
        /// <summary>
        /// get the user id
        /// Make assumption that ValidateSystemAdmin() has been called already
        /// </summary>
        int GetAdminSystemID();
        /// <summary>
        /// get the user name
        /// Make assumption that ValidateSystemAdmin() has been called already
        /// </summary>
        string GetAdminName();
    }
}
