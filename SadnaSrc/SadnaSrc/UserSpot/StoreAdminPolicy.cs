using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    class StoreAdminPolicy : UserPolicy
    {
        public enum StoreAction { StoreOwner,PromoteStoreAdmin, ManageProducts, DeclarePurchasePolicy, DeclareDiscountPolicy, ViewPurchaseHistory}
        private string _store;
        private StoreAction _storeAction;
        public StoreAdminPolicy(string store, StoreAction storeAction) : base(State.StoreAdmin)
        {
            _store = store;
            _storeAction = storeAction;
        }
    }
}
