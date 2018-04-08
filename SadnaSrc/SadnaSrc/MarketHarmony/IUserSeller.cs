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
        /// <summary>
        /// return the store manager id
        /// </summary>
        int GetID();
        /// <summary>
        /// Promote user with name of <paramref name="permission"/> with promotion of <paramref name="permission"/>
        /// <para />throw UserException if permission name is invalid
        /// <para />throw UserException if the user cannot given that exact promotion 
        /// </summary>
        /// <param name="permission"> The name of the permission action </param>
        /// <param name="userName"> The name of the user to be promoted </param>
        void Promote(string userName, string permission);
        /// <summary>
        /// Validate that the userName is not the promoter name.
        /// <para /> throw UserException if <paramref name="userName"/> is the name of the promoter
        /// </summary>
        /// <param name="userName"> The name of the user to be promoted </param>
        void ValidateNotPromotingHimself(string userName);
        /// <summary>
        /// Validate that the user can do that action
        /// <para /> throw UserException otherwise
        /// </summary>
        void CanPromoteStoreOwner();
        /// <summary>
        /// Validate that the user can do that action
        /// <para /> throw UserException otherwise
        /// </summary>
        void CanPromoteStoreAdmin();
        /// <summary>
        /// Validate that the user can do that action
        /// <para /> throw UserException otherwise
        /// </summary>
        void CanManagePurchasePolicy();
        /// <summary>
        /// Validate that the user can do that action
        /// <para /> throw UserException otherwise
        /// </summary>
        void CanDeclarePurchasePolicy();
        /// <summary>
        /// Validate that the user can do that action
        /// <para /> throw UserException otherwise
        /// </summary>
        void CanDeclareDiscountPolicy();
        /// <summary>
        /// Validate that the user can do that action
        /// <para /> throw UserException otherwise
        /// </summary>
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
