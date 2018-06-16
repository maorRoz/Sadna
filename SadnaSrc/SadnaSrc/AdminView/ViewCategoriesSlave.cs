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
    public class ViewCategoriesSlave
    {
        public MarketAnswer Answer;
        private readonly IUserAdmin _admin;
        private readonly IAdminDL _adminDL;

        public ViewCategoriesSlave(IUserAdmin admin, IAdminDL adminDL)
        {
            _admin = admin;
            _adminDL = adminDL;
        }

        public void ViewPolicies()
        {
            try
            {
                MarketLog.Log("AdminView", "Checking admin status.");
                _admin.ValidateSystemAdmin();
                Category[] catList = _adminDL.GetAllCategories();
                List<string> result = new List<string>();
                foreach (var cat in catList)
                    result.Add(cat.Name);                                  
                Answer = new AdminAnswer(ViewPolicyStatus.Success, "Successfully got all category names.", result.ToArray());

            }
            catch (AdminException e)
            {
                Answer = new AdminAnswer((ViewPolicyStatus)e.Status, e.GetErrorMessage());
            }
            catch (DataException e)
            {
                Answer = new AdminAnswer((ViewPolicyStatus)e.Status, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                Answer = new AdminAnswer(ViewPolicyStatus.NoAuthority, e.GetErrorMessage(), null);
            }
        }
    }
}
