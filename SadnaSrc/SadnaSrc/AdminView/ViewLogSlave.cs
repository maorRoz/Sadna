using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.AdminView
{
    class ViewLogSlave
    {
        public MarketAnswer Answer;
        private readonly IUserAdmin _admin;
        private readonly IAdminDL _adminDlInstacne;
        public ViewLogSlave(IAdminDL adminDl, IUserAdmin admin)
        {
            _admin = admin;
            _adminDlInstacne = adminDl;
        }

        public void ViewLog()
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to view system log");
                _admin.ValidateSystemAdmin();
                var eventLogReport = _adminDlInstacne.GetEventLogReport();
                Answer = new AdminAnswer(ViewSystemLogStatus.Success,"Successfully retrived the system log entries!", eventLogReport);
            }
            catch (AdminException e)
            {
                Answer = new AdminAnswer(ViewSystemLogStatus.NotSystemAdmin, e.GetErrorMessage());
            }
            catch (DataException e)
            {
                Answer = new AdminAnswer(ViewSystemLogStatus.NoDB, e.GetErrorMessage());
            }
        }
    }
}
