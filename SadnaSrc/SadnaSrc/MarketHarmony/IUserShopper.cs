using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.StoreCenter;

namespace SadnaSrc.MarketHarmony
{
    public interface IUserShopper
    {
        /// <summary>
        /// Add <paramref name="product"/> from <paramref name="store"/> in quantity of <paramref name="quantity"/>
        /// <para /> Make assumption that ValidateCanBrowseMarket() has been called already
        /// </summary>
        void AddToCart(Product product, string store, int quantity);
        /// <summary>
        /// Grant store ownership on <paramref name="store"/> . 
        /// <para /> Make assumption that ValidateCanOpenStore() has been called already
        /// </summary>
        void AddOwnership(string store);
        /// <summary>
        /// Validate that the user can do registered user action : open store/join lottery/join auction and has been entered the market
        /// <para /> throw UserException otherwise
        /// </summary>
        void ValidateRegistered();
        /// <summary>
        /// Validate that the user entered the market
        /// <para /> throw UserException otherwise
        /// </summary>
        void ValidateCanBrowseMarket();
        /// <summary>
        /// Grant shopper system id . 
        /// <para /> Make assumption that ValidateCanOpenStore() has been called already
        /// </summary>
        int GetShopperID();
        /// <summary>
        /// Grant shopper name . 
        /// <para /> Make assumption that ValidateCanOpenStore() has been called already
        /// </summary>
        string GetShopperName();

    }
}
