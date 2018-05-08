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
    public class AddPolicySlave
    {
        public MarketAnswer Answer;
        private readonly IUserAdmin _admin;
        private IGlobalPolicyManager _manager;

        public AddPolicySlave(IUserAdmin admin, IGlobalPolicyManager manager)
        {
            _manager = manager;
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
            catch (MarketException e)
            {
                Answer = new AdminAnswer(EditPolicyStatus.NoAuthority, e.GetErrorMessage());
            }
        }

        public void SaveFullPolicy()
        {
            try
            {
                MarketLog.Log("AdminView", "Checking admin status.");
                _admin.ValidateSystemAdmin();
                _manager.AddPolicy(_manager.GetSessionPolicies().Length);
                MarketLog.Log("AdminView", "Policy saved.");
                Answer = new AdminAnswer(EditPolicyStatus.Success, "Policy saved.");

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

        private void BuildPolicy(string type, string subject, string op, string arg1, string optArg)
        {
            CheckOpArgsCombo(op, arg1, optArg);
            int numericArg1, numericArg2;
            if (type == "Global" && subject == null)
            {
                if (IsCondtion(op))
                {
                    _manager.CreateGlobalSimplePolicy(GetConditionType(op), arg1);
                    return;
                }
                if (IsOperator(op))
                {
                    Int32.TryParse(arg1, out numericArg1);
                    Int32.TryParse(optArg, out numericArg2);
                    _manager.CreateGlobalPolicy(GetOperand(op), numericArg1, numericArg2);
                    return;
                }
            }
            if (type == "Category")
            {
                if (IsCondtion(op))
                {
                    _manager.CreateCategorySimplePolicy(subject,GetConditionType(op), arg1);
                    return;
                }
                if (IsOperator(op))
                {
                    Int32.TryParse(arg1, out numericArg1);
                    Int32.TryParse(optArg, out numericArg2);
                    _manager.CreateCategoryPolicy(subject, GetOperand(op), numericArg1, numericArg2);
                    return;
                }
            }
            if (type == "Product")
            {
                if (IsCondtion(op))
                {
                    _manager.CreateProductSimplePolicy(subject, GetConditionType(op), arg1);
                    return;
                }
                if (IsOperator(op))
                {
                    Int32.TryParse(arg1, out numericArg1);
                    Int32.TryParse(optArg, out numericArg2);
                    _manager.CreateProductPolicy(subject, GetOperand(op), numericArg1, numericArg2);
                    return;
                }
            }
            MarketLog.Log("AdminView", " Adding policy failed, invalid data.");
            throw new AdminException(EditPolicyStatus.InvalidPolicyData,"Invalid Policy data");

        }

        private void CheckOpArgsCombo(string op, string arg1, string optArg)
        {
            int numericArg1, numericArg2;
            if (IsNumericCondtion(op) && Int32.TryParse(arg1, out numericArg1)) return;
            if (IsOperator(op) && Int32.TryParse(arg1, out numericArg1) &&
                Int32.TryParse(optArg, out numericArg2)) return;
            MarketLog.Log("AdminView", " Adding policy failed, invalid data.");
            throw new AdminException(EditPolicyStatus.InvalidPolicyData, "Invalid Policy data");
        }
        
        private bool IsNumericCondtion(string cond)
        {
            return cond.Contains("Price >=") || cond.Contains("Price <=") || cond.Contains("Quantity >=") ||
                   cond.Contains("Quantity <=");
        }

        private bool IsCondtion(string cond)
        {
            return cond.Contains("Price >=") || cond.Contains("Price <=") || cond.Contains("Quantity >=") ||
                   cond.Contains("Quantity <=") || cond.Contains("Username =") || cond.Contains("Address =");
        }

        private bool IsOperator(string op)
        {
            return op.Contains("AND") || op.Contains("OR") || op.Contains("NOT");
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
