using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class EditCategoryDiscountSlave : AbstractStoreCenterSlave
    {
        public MarketAnswer Answer { get; set; }
        public EditCategoryDiscountSlave(string storeName, IUserSeller storeManager, IStoreDL storeDl) : base(storeName, storeManager, storeDl)
        {
        }

        internal void EditCategoryDiscount(string categoryName, string whatToEdit, string newValue)
        {
            throw new NotImplementedException();
        }
    }
}