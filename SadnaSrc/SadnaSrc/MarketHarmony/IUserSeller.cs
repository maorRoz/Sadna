using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.UserSpot;

namespace SadnaSrc.MarketHarmony
{
    //integration between UserSpot to StoreCenter for store managing
    public interface IUserSeller
    {
        int GetID();
        void Promote(string userName, string permission);

        void ValidateNotPromotingHimself(string userName);

        void CanPromoteStoreOwner();
        void CanPromoteStoreAdmin();

        void CanManagePurchasePolicy();

        void CanDeclarePurchasePolicy();

        void CanDeclareDiscountPolicy();

        void CanViewPurchaseHistory();

    }
    public enum PromoteStoreStatus
    {
        Success,
        InvalidStore,
        PromoteSelf,
        PromotionOutOfReach,
        NoAuthority,
        NoUserFound,
        InvalidPromotion
    }
}
