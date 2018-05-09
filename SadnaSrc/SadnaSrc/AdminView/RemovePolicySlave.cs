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
    public class RemovePolicySlave
    {
        public MarketAnswer Answer;
        private readonly IUserAdmin _admin;
        private IGlobalPolicyManager _manager;

        public RemovePolicySlave(IUserAdmin admin, IGlobalPolicyManager manager)
        {
            _manager = manager;
            _admin = admin;
        }


        public void RemovePolicy(string type, string subject)
        {
            try
            {
                MarketLog.Log("AdminView", "Checking admin status.");
                _admin.ValidateSystemAdmin();
                MarketLog.Log("AdminView", "Trying to add policy.");
                CheckInput(type, subject);
                _manager.RemovePolicy(GetPolicyType(type),subject);
                MarketLog.Log("AdminView", "Policy removed successfully.");
                Answer = new AdminAnswer(EditPolicyStatus.Success, "Policy removed.");

            }
            catch (AdminException e)
            {
                Answer = new AdminAnswer((EditPolicyStatus)e.Status, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                Answer = new AdminAnswer(EditPolicyStatus.NoAuthority, e.GetErrorMessage());
            }
        }

     

        private void CheckInput(string type, string subject)
        {
            if (type.Contains("Global") && subject == null) return;
            if (!string.IsNullOrEmpty(subject)) return;
            MarketLog.Log("AdminView", " Removing policy failed, invalid data.");
            throw new AdminException(EditPolicyStatus.InvalidPolicyData, "Invalid Policy data");

        }
        private PolicyType GetPolicyType(string op)
        {
            if (op.Contains("Global"))
                return PolicyType.Global;
            if (op.Contains("Product"))
                return PolicyType.Product;
            if (op.Contains("Category"))
                return PolicyType.Category;
            MarketLog.Log("AdminView", " Removing policy failed, invalid data.");
            throw new AdminException(EditPolicyStatus.InvalidPolicyData, "Invalid Policy data");
        }
    }

}
