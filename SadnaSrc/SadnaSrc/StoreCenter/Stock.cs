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
            public string SystemId { get; set; }
            public int quantity { get; set; }
            public Product product { get; set; }
            public Discount discount { get; set; }

            public PurchaseEnum PurchaseWay { get; set; }

            public StockListItem(int _quantity, Product _product, Discount _discount, PurchaseEnum _PurchaseWay, string id)
            {
                SystemId = id;
                quantity = _quantity;
                product = _product;
                discount = _discount;
                PurchaseWay = _PurchaseWay;
            }
        }
        public string myStoreID;
        public Stock(string _myStoreID)
        {
            myStoreID = _myStoreID;
        }


        internal StockListItem findstockListItembyProductID(string _product)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            StockListItem SLI = handler.dataLayer.getStockListItembyProductID(_product);
            return SLI;
        }
        internal StockListItem findByProduct(Product _product)
        {
            return findstockListItembyProductID(_product.SystemId);
        }
        public Product getProductById(string ID)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
            Product product = handler.dataLayer.getProductID(ID);
            return product;

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
        /**
* assume that the product is in the list
**/

    }
}
