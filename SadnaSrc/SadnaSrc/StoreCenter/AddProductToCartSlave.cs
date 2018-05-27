using System;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class AddProductToCartSlave
    {
        public MarketAnswer answer;
        private IUserShopper _shopper;
        IStoreDL storeLogic;
        
        public AddProductToCartSlave(IUserShopper shopper, IStoreDL storeDL)
        {
            _shopper = shopper;
            storeLogic = storeDL;
        }

        public void AddProductToCart(string store, string productName, int quantity)
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
                CheckifQuantityIsOK(quantity, stockListItem);   
                CheckIfDiscountExistsAndCalcValue(ref stockListItem);
                _shopper.AddToCart(stockListItem.Product, store, quantity);
                MarketLog.Log("StoreCenter", "add product successeded");
                answer = new StoreAnswer(StoreEnum.Success, quantity + " " + productName + " from " + store + "has been" +
                                                                 " successfully added to the user's cart!");
            }
            catch (StoreException e)
            {
                answer = new StoreAnswer((AddProductStatus)e.Status, "There is no product or store or quantity of that type in the market." +
                                                                  " request has been denied. Error message has been created!");
            }
            catch (MarketException)
            {
                answer = new StoreAnswer(StoreEnum.NoPermission,
                    "User validation as valid customer has been failed . only valid users can browse market. Error message has been created!");
            }
            catch (DataException e)
            {
                answer = new StoreAnswer((StoreEnum)e.Status, e.GetErrorMessage());
            }
        }

        private void CheckifQuantityIsOK(int quantity, StockListItem stockListItem)
        {
            MarketLog.Log("StoreCenter", "checking that the required quantity is not too big");
            if (quantity > stockListItem.Quantity)
            {
                MarketLog.Log("StoreCenter", "required quantity is not too big");
                throw new StoreException(StoreEnum.QuantityIsTooBig, "required quantity is not too big");
            }
            MarketLog.Log("StoreCenter", "checking that the required quantity is not negative or zero");
            if (quantity > 0) return;
            MarketLog.Log("StoreCenter", "required quantity is negative or zero");
            throw new StoreException(StoreEnum.QuantityIsNegative, "required quantity is negative");
        }

        private void CheckIfDiscountExistsAndCalcValue(ref StockListItem stockListItem)
        {
            if (stockListItem.Discount?.discountType != DiscountTypeEnum.Visible) return;
            if (stockListItem.Discount.CheckTime())
                stockListItem.Product.BasePrice = stockListItem.Discount.CalcDiscount(stockListItem.Product.BasePrice);
        }

        private void CheckIsProductNameAvailableInStore(string store, string productName)
        {
            if (!IsProductNameAvailableInStore(store, productName)) return;
            MarketLog.Log("StoreCenter", "Product is not exists in the store");
            throw new StoreException(StoreEnum.ProductNotFound, "product is not exists");
        }

        private void CheckIfStoreExitsts(string store)
        {
            if (!storeLogic.IsStoreExistAndActive(store))
            { throw new StoreException(StoreEnum.StoreNotExists, "store not exists or active"); }
        }

        private bool IsProductNameAvailableInStore(string storeName, string productName)
        {
            Product product = storeLogic.GetProductByNameFromStore(storeName, productName);
            return product == null;
        }
    }
}