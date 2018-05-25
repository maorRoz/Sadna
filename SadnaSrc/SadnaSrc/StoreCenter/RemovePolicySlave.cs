using SadnaSrc.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.MarketHarmony;
using SadnaSrc.PolicyComponent;

namespace SadnaSrc.StoreCenter
{
    public class RemovePolicySlave
    {
        public MarketAnswer Answer;
        private readonly IStorePolicyManager _manager;
        private readonly IUserSeller _storeManager;

        public RemovePolicySlave(IUserSeller storeManager, IStorePolicyManager manager)
        {
            _manager = manager;
            _storeManager = storeManager;
        }

        public void RemovePolicy(string type, string subject,string optProd)
        {
            try
            {
                MarketLog.Log("StoreCenter", "Checking store manager status.");
                _storeManager.CanDeclarePurchasePolicy();
                MarketLog.Log("StoreCenter", "Trying to remove policy.");
                CheckInput(type, subject,optProd);
                if(type == "Store")
                    _manager.RemovePolicy(GetPolicyType(type), subject);
                else
                    _manager.RemovePolicy(GetPolicyType(type), subject + "." + optProd);

                MarketLog.Log("StoreCenter", "Policy removed successfully.");
                Answer = new StoreAnswer(EditStorePolicyStatus.Success, "Policy removed.");

            }
            catch (StoreException e)
            {
                Answer = new StoreAnswer((EditStorePolicyStatus)e.Status, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                Answer = new StoreAnswer(EditStorePolicyStatus.NoAuthority, e.GetErrorMessage());
            }
        }



        private void CheckInput(string type, string subject,string optProd)
        {
            if (type == "Store" && !string.IsNullOrEmpty(subject) && string.IsNullOrEmpty(optProd)) return;
            if (type == "Stock Item" && !string.IsNullOrEmpty(subject) && !string.IsNullOrEmpty(optProd)) return;
            MarketLog.Log("StoreCenter", " Removing policy failed, invalid data.");
            throw new StoreException(EditStorePolicyStatus.InvalidPolicyData, "Invalid Policy data");

        }
        private PolicyType GetPolicyType(string type)
        {
            switch (type)
            {
                case "Stock Item":
                    return PolicyType.Product;
                case "Store":
                    return PolicyType.Category;
                default:
                    MarketLog.Log("StoreCenter", " Removing policy failed, invalid data.");
                    throw new StoreException(EditStorePolicyStatus.InvalidPolicyData, "Invalid Policy data");
            }
        }
    }
}
