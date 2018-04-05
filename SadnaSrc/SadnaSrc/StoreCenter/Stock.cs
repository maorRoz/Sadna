using SadnaSrc.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    /**
     * required functions:
     * being able to add new product to the store by any quantity (>=0)
     * being able to add any quantity (>=0) of exsiting product
     * being able to add\remove\edit discount to existing product
     * NOT being able to add\remove\edit discount unexisting product
     * ???? being able to add discount to new product??? (maybe need to split to 2 different actions? check use case)
     **/
    class Stock
    {
        public class StockListItem
        {
            public int SystemId { get; set; }
            public int quantity { get; set; }
            public Product product { get; set; }
            public Discount discount { get; set; }

            public PurchaseEnum PurchaseWay { get; set; }

            public StockListItem(int _quantity, Product _product, Discount _discount, PurchaseEnum _PurchaseWay)
            {
                quantity = _quantity;
                product = _product;
                discount = _discount;
                PurchaseWay = _PurchaseWay;
            }
        }
        public string myStoreID;
        public LinkedList<StockListItem> StockList;
        public Stock(string _myStoreID)
        {
            myStoreID = _myStoreID;
            StockList = new LinkedList<StockListItem>();
        }


        internal StockListItem findstockListItembyProductID(string _product)
        {
            foreach (StockListItem item in StockList)
            {
                if (item.product.SystemId.Equals(_product)) { return item; }
            }
            return null;
        }
        internal StockListItem findByProduct(Product _product)
        {
            foreach (StockListItem item in StockList) {
                if (findItemByProductPred(item, _product))
                {
                    return item;
                }
            }
            return null;
        }
        public Product getProductById(string ID)
        {
            foreach (StockListItem item in StockList)
            {
                if (item.product.SystemId==ID)
                {
                    return item.product;
                }
            }
            return null;
        }
        private static bool findItemByProductPred(StockListItem item, Product _product)
        {
            return item.product.equal(_product);
        }

        internal StoreAnswer addDiscountToProduct(Product p, Discount discount)
        {
            throw new NotImplementedException();
        }

        private bool CheckIfAvailable(Product _product, int _quantity)
        {
            StockListItem item = findByProduct(_product);
            if (item!=null)
                return (item.quantity >= _quantity);
            return false;
        }

        /**
         * this function will be use by store, no deligation
         **/

        public Discount getProductDiscount(Product _product)
        {
            StockListItem item = findByProduct(_product);
            if (item != null) { 
            return item.discount;
            }
            return null;

        }


        public double CalculateSingleItemPrice(Product _product, int _DiscountCode, int _quantity)
        {
            StockListItem item = findByProduct(_product);
            if (item != null)
            {
                if (_quantity < item.quantity && item.discount != null)
                {
                    return item.discount.calcDiscount(item.product.BasePrice, _DiscountCode) * _quantity;
                }
                return item.product.BasePrice * _quantity;
            }
            return -1;
        }

        public LinkedList<Product> getAllProducts()
        {
            LinkedList<Product> result = new LinkedList<Product>();
            foreach (StockListItem item in StockList)
            {
                result.AddLast(item.product);
            }
            return result;
        }

        public StoreAnswer removeDiscountToProduct(Product _product)
        {
            StockListItem item = findByProduct(_product);
            if (item != null)
            {
                item.discount = null;
                return new StoreAnswer(StoreEnum.Success, "item " + _product.toString() + " discount has removed");
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + _product.toString() + " does not exist in Stock");
        }

        /**public StoreAnswer addPurchaseWayToProduct(Product _product, PurchaseEnum _PurchaseWay)
        {
            StockListItem item = findByProduct(_product);
            if (item != null)
            {
                item.PurchaseWay = _PurchaseWay;
                return new StoreAnswer(StoreEnum.Success, "item " + _product.toString() + " added PurchaseWay of" + _PurchaseWay);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + _product.toString() + " does not exist in Stock");
        }
        public StoreAnswer removePurchaseWayToProduct(Product _product)
        {
            StockListItem item = findByProduct(_product);
            if (item != null)
            {
                item.PurchaseWay = PurchaseEnum.IMMEDIATE;
                return new StoreAnswer(StoreEnum.Success, "item " + _product.toString() + " removed PurchaseWay and set back to IMMIDIEATE");
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + _product.toString() + " does not exist in Stock");
        }**/
        /**
        * assume that the product is in the list
        **/
        public StoreAnswer addProductToStock(Product _product, int _quantity)
        {
            if (findByProduct(_product)!=null)
            { return addExistingProductToStock(_product, _quantity); }
            return addNewProductToStock(_product, _quantity);
        }
        public StoreAnswer removeProductFromStock (Product _product)
        {
            StockListItem item = findByProduct(_product);
            if (item != null)
            {
                StockList.Remove(item);
                return new StoreAnswer(StoreEnum.Success, "item " + _product.toString() + " removed");
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + _product.toString() + " does not exist in Stock");
        }
        private StoreAnswer addExistingProductToStock(Product _product, int _quantity)
        {
            if (_quantity >= 0)
            {
                StockListItem item = findByProduct(_product);
                item.quantity += _quantity;
                return new StoreAnswer(StoreEnum.Success, "item " + item + " added by amound of " + _quantity);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "quantity " + _quantity + " is less then 0");

        }
        private StoreAnswer addNewProductToStock(Product item, int quantity)
        {
            if (quantity >= 0)
            {
                StockList.AddLast(new StockListItem(quantity, item, null, PurchaseEnum.IMMEDIATE));
                return new StoreAnswer(StoreEnum.Success, "item " + item + " added by amound of " + quantity);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "quantity " + quantity + " is less then 0");
        }
        internal void addAllProductsToExistingList(LinkedList<Product> result)
        {
            foreach (StockListItem item in StockList)
            {
                result.AddLast(item.product);
            }
        }
    }
}
