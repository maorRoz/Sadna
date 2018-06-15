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
    public class ViewErrorSlave
    {
        public MarketAnswer Answer;
        private readonly IUserAdmin _admin;
        private readonly IAdminDL _adminDlInstacne;
        public ViewErrorSlave(IAdminDL adminDl, IUserAdmin admin)
        {
            _admin = admin;
            _adminDlInstacne = adminDl;
        }

        public void ViewError()
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to view system error log");
                _admin.ValidateSystemAdmin();
                var eventErrorLogReport = _adminDlInstacne.GetEventErrorLogReport();
                Answer = new AdminAnswer(ViewSystemErrorStatus.Success, "Successfully retrived the system error log entries!", eventErrorLogReport);
            }
            catch (AdminException e)
            {
                Answer = new AdminAnswer((ViewSystemErrorStatus)e.Status, e.GetErrorMessage());
            }
            catch (DataException e)
            {
                Answer = new AdminAnswer(ViewSystemErrorStatus.NoDB, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                Answer = new AdminAnswer(ViewSystemErrorStatus.NotSystemAdmin, e.GetErrorMessage());
            }
        }
    }
}
