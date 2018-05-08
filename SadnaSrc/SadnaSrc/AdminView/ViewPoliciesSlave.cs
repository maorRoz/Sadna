using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.PolicyComponent;

namespace SadnaSrc.AdminView
{
    public class ViewPoliciesSlave
    {
        public MarketAnswer Answer;
        private readonly IUserAdmin _admin;
        private IGlobalPolicyManager _manager;

        public ViewPoliciesSlave(IUserAdmin admin, IGlobalPolicyManager manager)
        {
            _manager = manager;
            _admin = admin;
        }


        public void ViewPolicies()
        {
            try
            {
                MarketLog.Log("AdminView", "Checking admin status.");
                _admin.ValidateSystemAdmin();
                MarketLog.Log("AdminView", "Trying to add policy.");
                string[] result = _manager.GetSessionPoliciesStrings();
                MarketLog.Log("AdminView", " Adding policy successfully.");
                Answer = new AdminAnswer(ViewPolicyStatus.Success, "Policy created.", result);

            }
            catch (AdminException e)
            {
                Answer = new AdminAnswer((EditPolicyStatus)e.Status, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                Answer = new AdminAnswer(ViewPolicyStatus.NoAuthority, e.GetErrorMessage(),null);
            }
        }
        
    }

}
