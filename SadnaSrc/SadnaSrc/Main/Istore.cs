using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.Main
{
    public interface IStore
    {
        /**
        * Store Managment, general Actions
        **/
        MarketAnswer PromoteToOwner(User someoneToPromote);
        MarketAnswer PromoteToManager(User someoneToPromote);
        LinkedList<Product> getAllStoreProducts();
        LinkedList<String> ViewPurchesHistory();
        MarketAnswer CloseStore(User ownerOrSystemAdmin);
        bool IsStoreActive();
        bool IsOwner(User user);
        Product getProductById(int ID); //will return null if product is not exists
        Discount getProductDiscountByProductID(int ID); //will return null if product is not exists or discount not exists
        PurchesEnum getProductPurchesWayByProductID(int ID);//will return PRODUCTNOTFOUND if product is not exists
        int getProductQuantitybyProductID(int ID);//will return -1 if product is not exists

        /**
         * Store Managment, handling products
         **/
        MarketAnswer AddProduct(String _name, int _price, String _description, int quantity);
        MarketAnswer IncreaseProductQuantity(Product product, int quantity);
        MarketAnswer removeProduct(Product product);
        MarketAnswer editProductPrice(Product product, int newprice);
        MarketAnswer editProductName(Product product, String Name);
        MarketAnswer editProductDescripiton(Product product, String Desccription);

        /**
         * Store Managment, handling PurchesWay
         **/

        MarketAnswer ChangeProductPurchesWayToImmidiate(Product product);
        MarketAnswer ChangeProductPurchesWayToLottery(Product product);
        /**
         * Store Managment, Discounts
         **/

        MarketAnswer addDiscountToProduct_VISIBLE(Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer addDiscountToProduct_HIDDEN(Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer addDiscountToProduct_presenteges_VISIBLE(Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer addDiscountToProduct_presenteges_HIDDEN(Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer EditDiscountToPrecenteges(Product product);
        MarketAnswer EditDiscountToNonPrecenteges(Product product);
        MarketAnswer EditDiscountToHidden(Product product);
        MarketAnswer EditDiscountToVisible(Product product);
        MarketAnswer EditDiscountAmount(Product product, int amount);
        MarketAnswer EditDiscountStartTime(Product product, DateTime _startDate);
        MarketAnswer EditDiscountEndTime(Product product, DateTime _EndDate);
        MarketAnswer removeDiscountFormProduct(Product product);

        /**
         * Store Managment, Purches
         **/
        LotteryTicket MakeALotteryPurches(Product product, int moeny);
        Product MakeAImmidiatePurches(Product product);
    }
}
