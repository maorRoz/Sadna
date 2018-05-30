using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;
using SadnaSrc.PolicyComponent;

namespace SadnaSrc.AdminView
{
    public class ViewPoliciesSlave
    {
        public MarketAnswer Answer;
        private readonly IUserAdmin _admin;
        private readonly IGlobalPolicyManager _manager;

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
                MarketLog.Log("AdminView", "Trying to view policies.");
                string[] result = _manager.ViewPolicies();
                MarketLog.Log("AdminView", "Successfully got policiy ids.");
                Answer = new AdminAnswer(ViewPolicyStatus.Success, "Successfully got policiy ids.", result);

            }
            catch (AdminException e)
            {
                Answer = new AdminAnswer((ViewPolicyStatus)e.Status, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                Answer = new AdminAnswer(ViewPolicyStatus.NoAuthority, e.GetErrorMessage(),null);
            }
            catch (DataException e)
            {
                Answer = new AdminAnswer((ViewPolicyStatus)e.Status, e.GetErrorMessage());
            }
        }

	    public void ViewSessionPolicies()
	    {
		    try
		    {
			    MarketLog.Log("AdminView", "Checking admin status.");
			    _admin.ValidateSystemAdmin();
			    MarketLog.Log("AdminView", "Trying to view policies.");
			    string[] result = _manager.ViewSessionPolicies();
			    MarketLog.Log("AdminView", "Successfully got policiy ids.");
			    Answer = new AdminAnswer(ViewPolicyStatus.Success, "Successfully got policiy ids.", result);

		    }
		    catch (AdminException e)
		    {
			    Answer = new AdminAnswer((ViewPolicyStatus)e.Status, e.GetErrorMessage());
		    }
		    catch (MarketException e)
		    {
			    Answer = new AdminAnswer(ViewPolicyStatus.NoAuthority, e.GetErrorMessage(), null);
		    }
		    catch (DataException e)
		    {
			    Answer = new AdminAnswer((ViewPolicyStatus)e.Status, e.GetErrorMessage());
		    }
	    }

	}

}
