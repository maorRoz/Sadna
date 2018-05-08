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
    class AddPolicySlave
    {
        public MarketAnswer Answer;
        private readonly IUserAdmin _admin;
        private IGlobalPolicyManager manager;

        public AddPolicySlave(IUserAdmin admin)
        {
            manager = MarketYard.Instance.GetGlobalPolicyManager();
            _admin = admin;
        }


        public void CreatePolicy(string type, string subject, string op, string arg1, string optArg)
        {
            try
            {
                MarketLog.Log("AdminView", "Checking admin status.");
                _admin.ValidateSystemAdmin();
                MarketLog.Log("AdminView", "Trying to add policy.");
                BuildPolicy(type, subject, op, arg1, optArg);
                MarketLog.Log("AdminView", " Adding policy successfully.");
                Answer = new AdminAnswer(EditPolicyStatus.Success, "Policy created.");

            }
            catch (AdminException e)
            {
                Answer = new AdminAnswer((EditPolicyStatus)e.Status, e.GetErrorMessage());
            }
        }

        public void SaveFullPolicy()
        {
            try
            {
                MarketLog.Log("AdminView", "Checking admin status.");
                _admin.ValidateSystemAdmin();
                manager.AddPolicy(manager.GetSessionPolicies().Length);
                MarketLog.Log("AdminView", "Policy saved.");
                Answer = new AdminAnswer(EditPolicyStatus.Success, "Policy saved.");

            }
            catch (AdminException e)
            {
                Answer = new AdminAnswer((EditPolicyStatus)e.Status, e.GetErrorMessage());
            }
        }

        private void BuildPolicy(string type, string subject, string op, string arg1, string optArg)
        {
            int numericArg;
            if (type == "Global" && subject == null)
            {
                return;
            }
               
            MarketLog.Log("AdminView", " Adding policy failed, invalid data.");
            throw new AdminException(EditPolicyStatus.InvalidPolicyData,"Invalid Policy data");

        }
    
        private ConditionType GetConditionType(string cond)
        {
            if (cond.Contains("Price >="))
                return ConditionType.PriceGreater;
            if (cond.Contains("Price <="))
                return ConditionType.PriceLesser;
            if (cond.Contains("Quantity >="))
                return ConditionType.QuantityGreater;
            if (cond.Contains("Quantity <="))
                return ConditionType.QuantityLesser;
            if (cond.Contains("Username ="))
                return ConditionType.UsernameEqual;
            if (cond.Contains("Address ="))
                return ConditionType.AddressEqual;
            MarketLog.Log("AdminView", " Adding policy failed, invalid data.");
            throw new AdminException(EditPolicyStatus.InvalidPolicyData, "Invalid Policy data");
        }

        private OperatorType GetOperand(string op)
        {
            if (op.Contains("AND"))
                return OperatorType.AND;
            if (op.Contains("OR"))
                return OperatorType.OR;
            if (op.Contains("NOT"))
                return OperatorType.NOT;
            MarketLog.Log("AdminView", " Adding policy failed, invalid data.");
            throw new AdminException(EditPolicyStatus.InvalidPolicyData, "Invalid Policy data");
        }
    }

}
