using SadnaSrc.Main;
using SadnaSrc.PolicyComponent;
using System;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class AddPolicySlave
    {
        public MarketAnswer Answer;
        private readonly IUserSeller _storeManager;
        private readonly IStorePolicyManager _manager;

        public AddPolicySlave(IUserSeller storeManager, IStorePolicyManager manager)
        {
            _manager = manager;
            _storeManager = storeManager;
        }


        public void CreatePolicy(string type, string subject, string optSubject, string op, string arg1, string optArg)
        {
            try
            {
                MarketLog.Log("StoreCenter", "Checking admin status.");
                _storeManager.CanDeclarePurchasePolicy();
                MarketLog.Log("StoreCenter", "Trying to add policy.");
                BuildPolicy(type, subject, optSubject, op, arg1, optArg);
                MarketLog.Log("StoreCenter", " Adding policy successfully.");
                Answer = new StoreAnswer(EditStorePolicyStatus.Success, "Policy created.");

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

        public void SaveFullPolicy()
        {
            try
            {
                MarketLog.Log("StoreCenter", "Checking store manager status.");
                _storeManager.CanDeclarePurchasePolicy();
                _manager.AddPolicy(_manager.GetSessionPolicies().Length);
                MarketLog.Log("StoreCenter", "Policy saved.");
                Answer = new StoreAnswer(EditStorePolicyStatus.Success, "Policy saved.");

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

        private void BuildPolicy(string type, string subject, string optSubject, string op, string arg1, string optArg)
        {
            CheckInput(type, subject, optSubject,op, arg1, optArg);
            switch (type)
            {
                case "Store":
                    BuildStorePolicy(subject, op, arg1, optArg);
                    break;
                case "Stock Item":
                    BuildStockItemPolicy(subject,optSubject, op, arg1, optArg);
                    break;
            }
        }

        private void BuildStorePolicy( string subject, string op, string arg1, string optArg)
        {
            int numericArg1, numericArg2;
            if (IsCondtion(op))
            {
                _manager.CreateStoreSimplePolicy(subject, GetConditionType(op), arg1);
                return;
            }
            Int32.TryParse(arg1, out numericArg1);
            Int32.TryParse(optArg, out numericArg2);
            _manager.CreateStorePolicy(subject, GetOperand(op), numericArg1, numericArg2);
        }

        private void BuildStockItemPolicy( string subject, string optSubject, string op, string arg1, string optArg)
        {
            int numericArg1, numericArg2;
            if (IsCondtion(op))
            {
                _manager.CreateStockItemSimplePolicy(subject, optSubject,GetConditionType(op), arg1);
                return;
            }
            Int32.TryParse(arg1, out numericArg1);
            Int32.TryParse(optArg, out numericArg2);
            _manager.CreateStockItemPolicy(subject, optSubject, GetOperand(op), numericArg1, numericArg2);
        }

        private void CheckInput(string type, string subject, string optProd, string op, string arg1, string optArg)
        {
            int numericArg1, numericArg2;
            if (CheckPolicySubject(type, subject, optProd) && IsNumericCondtion(op) && Int32.TryParse(arg1, out numericArg1)) return;
            if (CheckPolicySubject(type, subject, optProd) && IsOperator(op) && Int32.TryParse(arg1, out numericArg1) &&
                Int32.TryParse(optArg, out numericArg2)) return;
            MarketLog.Log("StoreCenter", " Adding policy failed, invalid data.");
            throw new StoreException(EditStorePolicyStatus.InvalidPolicyData, "Invalid Policy data");
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

        private bool CheckPolicySubject(string type, string subject, string optProd)
        {
            return (type.Contains("Store") & !string.IsNullOrEmpty(subject) & string.IsNullOrEmpty(optProd)) ||
                   (type.Contains("Stock Item") & !string.IsNullOrEmpty(subject) & !string.IsNullOrEmpty(optProd));
        }

        private ConditionType GetConditionType(string cond)
        {
            switch (cond)
            {
                case "Price >=":
                    return ConditionType.PriceGreater;
                case "Price <=":
                    return ConditionType.PriceLesser;
                case "Quantity >=":
                    return ConditionType.QuantityGreater;
                case "Quantity <=":
                    return ConditionType.QuantityLesser;
                case "Username =":
                    return ConditionType.UsernameEqual;
                case "Address =":
                    return ConditionType.AddressEqual;
                default:
                    MarketLog.Log("StoreCenter", " Adding policy failed, invalid data.");
                    throw new StoreException(EditStorePolicyStatus.InvalidPolicyData, "Invalid Policy data");
            }
        }

        private OperatorType GetOperand(string op)
        {
            switch (op)
            {
                case "AND":
                    return OperatorType.AND;
                case "OR":
                    return OperatorType.OR;
                case "NOT":
                    return OperatorType.NOT;
                default:
                    MarketLog.Log("StoreCenter", " Adding policy failed, invalid data.");
                    throw new StoreException(EditStorePolicyStatus.InvalidPolicyData, "Invalid Policy data");
            }
        }
    }
}
