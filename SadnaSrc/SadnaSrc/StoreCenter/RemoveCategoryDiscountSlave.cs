using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class RemoveCategoryDiscountSlave
    {
        private string _storeName;
        private IUserSeller _storeManager;
        private IStoreDL storeDL;

        public RemoveCategoryDiscountSlave(string storeName, IUserSeller storeManager, IStoreDL storeDL)
        {
            _storeName = storeName;
            _storeManager = storeManager;
            this.storeDL = storeDL;
        }

        public MarketAnswer Answer { get; set; }

        public void RemoveCategoryDiscount(string categoryName)
        {
            throw new System.NotImplementedException();
        }
    }
}