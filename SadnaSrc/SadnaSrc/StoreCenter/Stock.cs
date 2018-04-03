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
            foreach (StockListItem item in StockList) {
                if (findItemByProductPred(item, _product))
                {
                    return item;
                }
            }
            return null;
            //return StockList.Find(item => item.product.equal(_product));
        }
        public Product getProductById(int ID)
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
            if (item != null) { 
            return item.discount;
            }
            return null;

        }

        public int getProductQuantity(Product _product)
        {
            StockListItem item = findByProduct(_product);
            if (item != null){
                return item.quantity;
            }
            return -1;

        }
        public PurchesEnum getProductPurchaseWay(Product _product)
        {
            StockListItem item = findByProduct(_product);
            if (item != null){
                return item.PurchesWay;
            }
            return PurchesEnum.PRODUCTNOTFOUND;
        }
        // keep in mind next Function
        public StoreAnswer UpdateQuantityAfterPruchese(Product _product, int _quantity)
        {
            if (CheckIfAvailable(_product, _quantity))
            {
                StockListItem item = findByProduct(_product);
                
                if (item != null) {
                    item.quantity = item.quantity - _quantity;
                return new StoreAnswer(StoreEnum.Success, "amount of " + _quantity + " unites has been removed from product" + _product);
                }
                return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + _product.toString() + " does not exist in Stock");
            }
            else
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "there are not enough units from product" + _product);
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
         * assume that the product is in the list
         **/
        public StoreAnswer addDiscountToProduct(Product _product, Discount _discount)
        {
            StockListItem item = findByProduct(_product);
            if (item!=null)
            {
                item.discount = _discount;
                return new StoreAnswer(StoreEnum.Success, "item " + _product.toString() + " added discount of" + _discount.toString());
            }
            else
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + _product.toString() + " does not exist in Stock");
            }
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

        internal StoreAnswer editProductPrice(Product product, int newprice)
        {
            StockListItem item = findByProduct(product);
            if (item != null)
            {
                item.product.BasePrice = newprice;
                return new StoreAnswer(StoreEnum.Success, "product " + product.SystemId + " price has been updated to " + newprice);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + product.toString() + " does not exist in Stock");

        }

        internal StoreAnswer editProductName(Product product, string name)
        {
            StockListItem item = findByProduct(product);
            if (item != null)
            {
                item.product.name = name;
                return new StoreAnswer(StoreEnum.Success, "product " + product.SystemId + " name has been updated to " + name);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + product.toString() + " does not exist in Stock");

        }
        internal StoreAnswer editProductDescripiton(Product product, string Description)
        {
            StockListItem item = findByProduct(product);
            if (item != null)
            {
                item.product.description = Description;
                return new StoreAnswer(StoreEnum.Success, "product " + product.SystemId + " Description has been updated to " + Description);
            }
            return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + product.toString() + " does not exist in Stock");

        }
        public StoreAnswer EditDiscountPrecenteges(Product product, bool isPresenteges)
        {
            StockListItem item = findByProduct(product);
            if (item != null)
            {
                if (isPresenteges && item.discount.DiscountAmount > 100)
                {
                    return new StoreAnswer(StoreEnum.UpdateStockFail, "DiscountAmount is >= 100, cant make it presenteges");
                }
                item.discount.Presenteges= isPresenteges;
                if (isPresenteges) { 
                return new StoreAnswer(StoreEnum.Success, "item " + product.toString() + " discount preseneges become true");}
                return new StoreAnswer(StoreEnum.Success, "item " + product.toString() + " discount preseneges become false");
            }
            else
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + product.toString() + " does not exist in Stock");
            }
        }
        public StoreAnswer EditDiscountMode(Product product, discountTypeEnum type)
        {
            StockListItem item = findByProduct(product);
            if (item != null)
            {
                item.discount.discountType = type;
                return new StoreAnswer(StoreEnum.Success, "item " + product.toString() + " discount type become " + Discount.PrintEnum(type));
            }
            else
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + product.toString() + " does not exist in Stock");
            }
        }
        public StoreAnswer EditDiscountAmount(Product product, int amount)
        {
            StockListItem item = findByProduct(product);
            if (item != null)
            {
                if (item.discount.Presenteges&&amount>100)
                {
                        return new StoreAnswer(StoreEnum.UpdateStockFail, "DiscountAmount is >= 100, cant make it presenteges");
                }
                item.discount.DiscountAmount = amount;
                return new StoreAnswer(StoreEnum.Success, "item " + product.toString() + " discount amount become " + amount);
            }
            else
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + product.toString() + " does not exist in Stock");
            }
        }

        public StoreAnswer EditDiscountStartTime(Product product, DateTime startTime)
        {
            StockListItem item = findByProduct(product);
            if (item != null)
            {
                if (startTime < DateTime.Now.Date)
                {
                    return new StoreAnswer(StoreEnum.UpdateStockFail, "can't set start time in the past");
                }

                if (startTime > item.discount.EndDate)
                {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "can't set start time that is later then the discount end time");
                }
            item.discount.startDate = startTime;
                return new StoreAnswer(StoreEnum.Success, "item " + product.toString() + " discount Start Date become " + startTime);
            }
            else
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + product.toString() + " does not exist in Stock");
            }
        }
        public StoreAnswer EditDiscountEndTime(Product product, DateTime EndTime)
        {
            StockListItem item = findByProduct(product);
            if (item != null)
            {
                if (EndTime < DateTime.Now.Date)
                {
                    return new StoreAnswer(StoreEnum.UpdateStockFail, "can't set end time to the past");
                }

                if (EndTime < item.discount.startDate)
                {
                    return new StoreAnswer(StoreEnum.UpdateStockFail, "can't set End time that is sooner then the discount start time");
                }
                item.discount.EndDate = EndTime;
                return new StoreAnswer(StoreEnum.Success, "item " + product.toString() + " discount End Date become " + EndTime);
            }
            else
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + product.toString() + " does not exist in Stock");
            }
        }
        public StoreAnswer removeDiscountToProduct(Product _product)
        {
            StockListItem item = findByProduct(_product);
            if (item != null)
            {
                item.discount = null;
                return new StoreAnswer(StoreEnum.Success, "item " + _product.toString() + " discount has removed");
            }
            else
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + _product.toString() + " does not exist in Stock");
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
                return new StoreAnswer(StoreEnum.Success, "item " + _product.toString() + " added PurchesWay of" + _purchesWay);
            }
            else
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + _product.toString() + " does not exist in Stock");
            }
        }
        public StoreAnswer removePurchesWayToProduct(Product _product)
        {
            StockListItem item = findByProduct(_product);
            if (item != null)
            {
                item.PurchesWay = PurchesEnum.IMMIDIATE;
                return new StoreAnswer(StoreEnum.Success, "item " + _product.toString() + " removed PurchesWay and set back to IMMIDIEATE");
            }
            else
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + _product.toString() + " does not exist in Stock");
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
                return new StoreAnswer(StoreEnum.Success, "item " + _product.toString() + " removed");
            }
            else
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "product " + _product.toString() + " does not exist in Stock");
            }
        }
        private StoreAnswer addExistingProductToStock(Product _product, int _quantity)
        {
            if (_quantity >= 0)
            {
                StockListItem item = findByProduct(_product);
                item.quantity += _quantity;
                return new StoreAnswer(StoreEnum.Success, "item " + item + " added by amound of " + _quantity);
            }
            else
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "quantity " + _quantity + " is less then 0");
            }

        }
        private StoreAnswer addNewProductToStock(Product item, int quantity)
        {
            if (quantity >= 0)
            {
                StockList.AddLast(new StockListItem(quantity, item, null, PurchesEnum.IMMIDIATE));
                return new StoreAnswer(StoreEnum.Success, "item " + item + " added by amound of " + quantity);
            }
            else
            {
                return new StoreAnswer(StoreEnum.UpdateStockFail, "quantity " + quantity + " is less then 0");
            }
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
