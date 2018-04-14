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
        public string MyStoreID { get; set; }
        public Stock(string myStoreID)
        {
            MyStoreID = myStoreID;
        }


        public StockListItem FindstockListItembyProductID(string product)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            return handler.DataLayer.GetStockListItembyProductID(product);
        }
        public StockListItem FindByProduct(Product _product)
        {
            return FindstockListItembyProductID(_product.SystemId);
        }
        public Product GetProductById(string ID)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            Product product = handler.DataLayer.GetProductID(ID);
            return product;

        }

        /**
         * this function will be use by store, no deligation
         **/

        public Discount GetProductDiscount(Product _product)
        {
            StockListItem item = FindByProduct(_product);
            if (item != null) { 
            return item.Discount;
            }
            return null;

        }


        
        /**
* assume that the product is in the list
**/

    }
}
