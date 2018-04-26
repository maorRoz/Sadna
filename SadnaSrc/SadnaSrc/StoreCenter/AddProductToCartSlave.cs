using System;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    internal class AddProductToCartSlave
    {
        internal MarketAnswer answer;
        private IUserShopper _shopper;
        StoreDL storeLogic;

        public AddProductToCartSlave(IUserShopper shopper)
        {
            _shopper = shopper;
            storeLogic = StoreDL.Instance;
        }

        internal void AddProductToCart(string store, string productName, int quantity)
        {
            MarketLog.Log("StoreCenter", "trying to add something to the cart");
            MarketLog.Log("StoreCenter", "checking if store exists");
            if (!storeLogic.IsStoreExistAndActive(store))
            { throw new StoreException(StoreEnum.StoreNotExists, "store not exists or active"); }
            MarketLog.Log("StoreCenter", "checking if user has premmisions");
            _shopper.ValidateCanBrowseMarket();
            MarketLog.Log("StoreCenter", "checking if product exists");
            if (IsProductNameAvailableInStore(store, productName)) //aka product is NotFiniteNumberException in store
            {
                MarketLog.Log("StoreCenter", "Product is not exists in the store");
                throw new StoreException(StoreEnum.ProductNotFound, "product is not exists");
            }
            StockListItem stockListItem = storeLogic.GetProductFromStore(store, productName);
            MarketLog.Log("StoreCenter", "checking that the required quantity is not too big");
            if (quantity > stockListItem.Quantity)
            {
                MarketLog.Log("StoreCenter", "required quantity is not too big");
                throw new StoreException(StoreEnum.QuantityIsTooBig, "required quantity is not too big");
            }
            MarketLog.Log("StoreCenter", "checking that the required quantity is not negative or zero");
            if (quantity <= 0)
            {
                MarketLog.Log("StoreCenter", "required quantity is negative or zero");
                throw new StoreException(StoreEnum.quantityIsNegatie, "required quantity is negative");
            }
            if (stockListItem.Discount != null)
            {
                if (stockListItem.Discount.discountType == discountTypeEnum.Visible)
                    if (stockListItem.Discount.checkTime())
                        stockListItem.Product.BasePrice = stockListItem.Discount.CalcDiscount(stockListItem.Product.BasePrice);
            }
            _shopper.AddToCart(stockListItem.Product, store, quantity);
            MarketLog.Log("StoreCenter", "add product successeded");
            answer = new StoreAnswer(StoreEnum.Success, quantity + " " + productName + " from " + store + "has been" +
                                                             " successfully added to the user's cart!");
        }

        private bool IsProductNameAvailableInStore(string storeName, string productName)
        {
            Product P = storeLogic.getProductByNameFromStore(storeName, productName);
            return (P == null);
        }
    }
}