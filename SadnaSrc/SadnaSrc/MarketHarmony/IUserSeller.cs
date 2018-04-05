using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.UserSpot;

namespace SadnaSrc.MarketHarmony
{
    //integration between UserSpot to StoreCenter for store managing
    interface IUserSeller
    {
        void Promote(string userName, StoreManagerPolicy.StoreAction [] permission);

        bool CanPromoteStoreAdmin();

        bool CanManagePurchasePolicy();

        bool CanDeclarePurchasePolicy();

        bool CanDeclareDiscountPolicy();

        bool CanViewPurchaseHistory();

    }
}
