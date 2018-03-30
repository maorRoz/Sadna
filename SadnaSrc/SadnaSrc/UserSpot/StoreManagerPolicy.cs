using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    public class StoreManagerPolicy : UserPolicy
    {
        public enum StoreAction { StoreOwner,PromoteStoreAdmin, ManageProducts, DeclarePurchasePolicy, DeclareDiscountPolicy, ViewPurchaseHistory}
        public string Store { get; }
        public StoreAction Action { get; }
        public StoreManagerPolicy(StoreAction storeAction, string store) : base(State.StoreManager)
        {
            Action = storeAction;
            Store = store;
        }

        public string GetStoreActionString()
        {
            switch (Action)
            {
                case StoreAction.StoreOwner:
                    return "StoreOwner";
                case StoreAction.PromoteStoreAdmin:
                    return "PromoteStoreAdmin";
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
