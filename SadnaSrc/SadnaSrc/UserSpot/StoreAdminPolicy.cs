using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    public class StoreAdminPolicy : UserPolicy
    {
        public enum StoreAction { StoreOwner,PromoteStoreAdmin, ManageProducts, DeclarePurchasePolicy, DeclareDiscountPolicy, ViewPurchaseHistory}
        private string _store;
        private StoreAction _storeAction;
        public StoreAdminPolicy(StoreAction storeAction, string store) : base(State.StoreManager)
        {
            _storeAction = storeAction;
            _store = store;
        }

        public StoreAction GetAction()
        {
            return _storeAction;
        }

        public string GetStoreActionString()
        {
            switch (_storeAction)
            {
                case StoreAction.StoreOwner:
                    return "StoreManager";
                case StoreAction.PromoteStoreAdmin:
                    return "SystemAdmin";
                case StoreAction.ManageProducts:
                    return "ManageProducts";
                case StoreAction.DeclarePurchasePolicy:
                    return "DeclarePurchasePolicy";
                case StoreAction.DeclareDiscountPolicy:
                    return "DeclareDiscountPolicy";
                default:
                    return "ViewPurchaseHistory";
            }
        }
        public static StoreAction GetActionFromString(string actionString)
        {
            if (actionString.Equals("StoreOwner"))
            {
                return StoreAction.StoreOwner;
            }

            if (actionString.Equals("PromoteStoreAdmin"))
            {
                return StoreAction.StoreOwner;
            }

            if (actionString.Equals("ManageProducts"))
            {
                return StoreAction.StoreOwner;
            }

            if (actionString.Equals("DeclarePurchasePolicy"))
            {
                return StoreAction.StoreOwner;
            }

            if (actionString.Equals("ViewPurchaseHistory"))
            {
                return StoreAction.StoreOwner;
            }

            throw new UserException(MarketError.LogicError, "Procedure to cast string into store action has failed! there is no state by that name...");
        }
    }
}
