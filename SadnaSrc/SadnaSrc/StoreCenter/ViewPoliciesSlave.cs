using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;
using SadnaSrc.PolicyComponent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.MarketData;

namespace SadnaSrc.StoreCenter
{
    public class ViewPoliciesSlave
    {
        public MarketAnswer Answer;
        private readonly IUserSeller _storeManager;
        private readonly IStorePolicyManager _manager;

        public ViewPoliciesSlave(IUserSeller storeManager, IStorePolicyManager manager)
        {
            _manager = manager;
            _storeManager = storeManager;
        }


        public void ViewPolicies()
        {
            try
            {
                MarketLog.Log("StoreCenter", "Checking store manager status.");
                _storeManager.CanDeclarePurchasePolicy();
                MarketLog.Log("StoreCenter", "Trying to view policies.");
                string[] result = _manager.ViewStorePolicies();
                MarketLog.Log("StoreCenter", "Successfully got policiy ids.");
                Answer = new StoreAnswer(ViewStorePolicyStatus.Success, "Successfully got policiy ids.", result);

            }
            catch (StoreException e)
            {
                Answer = new StoreAnswer((EditStorePolicyStatus)e.Status, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                Answer = new StoreAnswer(ViewStorePolicyStatus.NoAuthority, e.GetErrorMessage(), null);
            }
        }

        public void ViewPolicies(string store)
        {
            try
            {
                MarketLog.Log("StoreCenter", "Checking store manager status.");
                _storeManager.CanDeclarePurchasePolicy();
                MarketLog.Log("StoreCenter", "Trying to view policies.");
                string[] result = _manager.ViewStorePolicies(store);
                MarketLog.Log("StoreCenter", "Successfully got policiy ids.");
                Answer = new StoreAnswer(ViewStorePolicyStatus.Success, "Successfully got policiy ids.", result);

            }
            catch (StoreException e)
            {
                Answer = new StoreAnswer((EditStorePolicyStatus) e.Status, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                Answer = new StoreAnswer(ViewStorePolicyStatus.NoAuthority, e.GetErrorMessage(), null);
            }
        }

	    public void ViewSessionPolicies()
	    {
		    try
		    {
			    MarketLog.Log("StoreCenter", "Checking store manager status.");
			    _storeManager.CanDeclarePurchasePolicy();
			    MarketLog.Log("StoreCenter", "Trying to view policies.");
			    string[] result = _manager.ViewSessionPolicies();
			    MarketLog.Log("StoreCenter", "Successfully got policiy ids.");
			    Answer = new StoreAnswer(ViewStorePolicyStatus.Success, "Successfully got policiy ids.", result);

		    }
		    catch (StoreException e)
		    {
			    Answer = new StoreAnswer((EditStorePolicyStatus)e.Status, e.GetErrorMessage());
		    }
		    catch (DataException e)
		    {
		        Answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
		    }
            catch (MarketException e)
		    {
			    Answer = new StoreAnswer(ViewStorePolicyStatus.NoAuthority, e.GetErrorMessage(), null);
		    }

		}

	}
}
