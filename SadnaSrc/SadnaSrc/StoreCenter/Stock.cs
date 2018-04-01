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
            public int quantity { get; set; }
            public Product product { get; set; }
            public Discount discount { get; set; }

            public PurchesEnum PurchesWay { get; set; }

            public StockListItem(int _quantity, Product _product, Discount _discount, PurchesEnum _PurchesWay)
            {
                quantity = _quantity;
                product = _product;
                discount = _discount;
                PurchesWay = _PurchesWay;
            }
        }
        public LinkedList<StockListItem> StockList;
        public Stock()
        {
            StockList = new LinkedList<StockListItem>();
        }


        private StockListItem findByProduct(Product _product)
        {
            IEnumerator< StockListItem> iter = StockList.GetEnumerator();
            foreach (StockListItem item in StockList) {
                if (findItemByProductPred(item, _product))
                {
                    return item;
                }
            }
            return null;
            //return StockList.Find(item => item.product.equal(_product));
        }
        private static bool findItemByProductPred(StockListItem item, Product _product)
        {
            return item.product.equal(_product);
        }
        private bool CheckIfAvailable(Product _product, int _quantity)
        {
            StockListItem item = findByProduct(_product);
            return (item.quantity >= _quantity);
        }

        /**
         * this function will be use by store, no deligation
         **/
        public Product getProduct(Product _product)
        {
            StockListItem item = findByProduct(_product);
            return item.product;
        }

        public Discount getProductDiscount(Product _product)
        {
            StockListItem item = findByProduct(_product);
            return item.discount;

        }

        public int getProductQuantity(Product _product)
        {
            StockListItem item = findByProduct(_product);
            return item.quantity;

        }
        public PurchesEnum getProductPurchaseWay(Product _product)
        {
            StockListItem item = findByProduct(_product);
            return item.PurchesWay;

        }
        public StoreAnswer UpdateQuantityAfterPruchese(Product _product, int _quantity)
        {
            if (CheckIfAvailable(_product, _quantity))
            {
                StockListItem item = findByProduct(_product);
                item.quantity = item.quantity - _quantity;
                return new StoreAnswer(1, "amount of " + _quantity + " unites has been removed from product" + _product);
            }
            else
            {
                return new StoreAnswer(0, "there are not enough units from product" + _product);
            }
        }
        /**
         * this function will be use by store, no deligation
         **/
        public double CalculateSingleItemPrice(Product _product, int _DiscountCode, int _quantity)
        {
            StockListItem item = findByProduct(_product);
            if (item!=null)
            {
                if (_quantity < item.quantity)
                {
                    if (item.discount != null)
                    {
                        return item.discount.calcDiscount(item.product.BasePrice, _DiscountCode) * _quantity;
                    }
                    return item.product.BasePrice * _quantity;
                }
            }
            return -1;
        }
        /**
         * find product in the list, not clear if this is internal or external function.
         **/
        /**
         * assume that the product is in the list
         **/
        public StoreAnswer editProductDiscount(Product _product, Discount _discount)
        {
            return addDiscountToProduct(_product, _discount);
        }
        public StoreAnswer addDiscountToProduct(Product _product, Discount _discount)
        {
            StockListItem item = findByProduct(_product);
            if (item!=null)
            {
                item.discount = _discount;
                return new StoreAnswer(1, "item " + _product.toString() + " added discount of" + _discount.toString());
            }
            else
            {
                return new StoreAnswer(0, "product " + _product.toString() + " does not exist in Stock");
            }
        }
        public StoreAnswer removeDiscountToProduct(Product _product)
        {
            StockListItem item = findByProduct(_product);
            if (item != null)
            {
                item.discount = null;
                return new StoreAnswer(1, "item " + _product.toString() + " discount has removed");
            }
            else
            {
                return new StoreAnswer(0, "product " + _product.toString() + " does not exist in Stock");
            }
        }

        public StoreAnswer editProductPurchesWay(Product _product, PurchesEnum _purchesWay)
        {
            return addPurchesWayToProduct(_product, _purchesWay);
        }
        public StoreAnswer addPurchesWayToProduct(Product _product, PurchesEnum _purchesWay)
        {
            StockListItem item = findByProduct(_product);
            if (item != null)
            {
                item.PurchesWay = _purchesWay;
                return new StoreAnswer(1, "item " + _product.toString() + " added PurchesWay of" + _purchesWay);
            }
            else
            {
                return new StoreAnswer(0, "product " + _product.toString() + " does not exist in Stock");
            }
        }
        public StoreAnswer removePurchesWayToProduct(Product _product)
        {
            StockListItem item = findByProduct(_product);
            if (item != null)
            {
                item.PurchesWay = PurchesEnum.IMMIDIATE;
                return new StoreAnswer(1, "item " + _product.toString() + " removed PurchesWay and set back to IMMIDIEATE");
            }
            else
            {
                return new StoreAnswer(0, "product " + _product.toString() + " does not exist in Stock");
            }
        }
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
                return new StoreAnswer(1, "item " + _product.toString() + " removed");
            }
            else
            {
                return new StoreAnswer(0, "product " + _product.toString() + " does not exist in Stock");
            }
        }
        public StoreAnswer addExistingProductToStock(Product _product, int _quantity)
        {
            if (_quantity >= 0)
            {
                StockListItem item = findByProduct(_product);
                item.quantity += _quantity;
                return new StoreAnswer(1, "item " + item + " added by amound of " + _quantity);
            }
            else
            {
                return new StoreAnswer(0, "quantity " + _quantity + " is less then 0");
            }

        }
        /**
         * assume that the product never has been in the list
         **/
        public StoreAnswer addNewProductToStock(Product item, int quantity)
        {
            if (quantity >= 0)
            {
                StockList.AddLast(new StockListItem(quantity, item, null, PurchesEnum.IMMIDIATE));
                return new StoreAnswer(1, "item " + item + " added by amound of " + quantity);
            }
            else
            {
                return new StoreAnswer(0, "quantity " + quantity + " is less then 0");
            }
        }
    }
}
