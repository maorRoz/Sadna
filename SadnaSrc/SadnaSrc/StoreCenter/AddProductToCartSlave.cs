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

        public AddProductToCartSlave(IUserShopper shopper, I_StoreDL storeDL)
        {
            _shopper = shopper;
            storeLogic = storeDL;
        }

        internal void AddProductToCart(string store, string productName, int quantity)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to add something to the cart");
                MarketLog.Log("StoreCenter", "checking if store exists");
                CheckIfStoreExitsts(store);
                MarketLog.Log("StoreCenter", "checking if user has premmisions");
                _shopper.ValidateCanBrowseMarket();
                MarketLog.Log("StoreCenter", "checking if product exists");
                CheckIsProductNameAvailableInStore(store, productName);
                StockListItem stockListItem = storeLogic.GetProductFromStore(store, productName);
                checkifQuantityIsOK(quantity, stockListItem);   
                checkIfDiscountExistsAndCalcValue(ref stockListItem);
                _shopper.AddToCart(stockListItem.Product, store, quantity);
                MarketLog.Log("StoreCenter", "add product successeded");
                answer = new StoreAnswer(StoreEnum.Success, quantity + " " + productName + " from " + store + "has been" +
                                                                 " successfully added to the user's cart!");
            }
            catch (StoreException e)
            {
                MarketLog.Log("StoreCenter", "adding to cart failed");
                answer = new StoreAnswer((AddProductStatus)e.Status, "There is no product or store or quantity of that type in the market." +
                                                                  " request has been denied. Error message has been created!");
            }
            catch (MarketException)
            {
                MarketLog.Log("StoreCenter", "no premission");
                answer = new StoreAnswer(StoreEnum.NoPremmision,
                    "User validation as valid customer has been failed . only valid users can browse market. Error message has been created!");
            }
        }

        private void checkifQuantityIsOK(int quantity, StockListItem stockListItem)
        {
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
        }

        private void checkIfDiscountExistsAndCalcValue(ref StockListItem stockListItem)
        {
            if (stockListItem.Discount != null)
            {
                if (stockListItem.Discount.discountType == discountTypeEnum.Visible)
                    if (stockListItem.Discount.CheckTime())
                        stockListItem.Product.BasePrice = stockListItem.Discount.CalcDiscount(stockListItem.Product.BasePrice);
            }
        }

        private void CheckIsProductNameAvailableInStore(string store, string productName)
        {
            if (IsProductNameAvailableInStore(store, productName)) //aka product is NotFiniteNumberException in store
            {
                MarketLog.Log("StoreCenter", "Product is not exists in the store");
                throw new StoreException(StoreEnum.ProductNotFound, "product is not exists");
            }
        }

        private void CheckIfStoreExitsts(string store)
        {
            if (!storeLogic.IsStoreExistAndActive(store))
            { throw new StoreException(StoreEnum.StoreNotExists, "store not exists or active"); }
        }

        private bool IsProductNameAvailableInStore(string storeName, string productName)
        {
            Product P = storeLogic.GetProductByNameFromStore(storeName, productName);
            return (P == null);
        }
    }
}