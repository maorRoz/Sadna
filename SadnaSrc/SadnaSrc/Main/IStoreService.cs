using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.StoreCenter;
using SadnaSrc.UserSpot;

namespace SadnaSrc.Main
{
    public interface IStoreService
    {
        /**
         * general actions that refer to all the stores/some of them
         **/
        MarketAnswer OpenStore(User owner);
        MarketAnswer CloseStore(Store store, User ownerOrSystemAdmin);
        LinkedList<Store> getAllUsersStores(User owner);
        LinkedList<Store> getAllStores();
        LinkedList<Product> getAllMarketProducts();
        Store getStoreByID(int ID);


        /**
         * Store Managment, general Actions
         **/
        MarketAnswer PromoteToOwner(Store store, User CurrentUser, User someoneToPromote);
        MarketAnswer PromoteToManager(Store store, User CurrentUser, User someoneToPromote);
        LinkedList<Product> getAllStoreProducts(Store store);
        LinkedList<String> ViewPurchesHistory(Store store);

        /**
         * Store Managment, handling products
         **/
        MarketAnswer AddProduct(Store store, String _name, int _price, String _description, int quantity);
        MarketAnswer IncreaseProductQuantity(Store store, Product product, int quantity);
        MarketAnswer removeProduct(Store store, Product product);
        MarketAnswer editProductPrice(Store store, Product product, int newprice);
        MarketAnswer editProductName(Store store, Product product, String Name);
        MarketAnswer editProductDescripiton(Store store, Product product, String Desccription);

        /**
         * Store Managment, handling PurchesWay
         **/

        MarketAnswer ChangeProductPurchesWayToImmidiate(Store store, Product product);
        MarketAnswer ChangeProductPurchesWayToLottery(Store store, Product product, DateTime StartDate, DateTime EndDate);
        /**
         * Store Managment, Discounts
         **/

        MarketAnswer addDiscountToProduct_VISIBLE(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer addDiscountToProduct_HIDDEN(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer addDiscountToProduct_presenteges_VISIBLE(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer addDiscountToProduct_presenteges_HIDDEN(Store store, Product product, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer removeDiscount(Store store, Product product);
        MarketAnswer EditDiscountToPrecenteges(Store store, Product product);
        MarketAnswer EditDiscountToNonPrecenteges(Store store, Product product);
        MarketAnswer EditDiscountToHidden(Store store, Product product);
        MarketAnswer EditDiscountToVisible(Store store, Product product);
        MarketAnswer EditDiscountAmunt(Store store, Product product, int amount);
        MarketAnswer EditDiscountStartTime(Store store, Product product, DateTime _startDate);
        MarketAnswer EditDiscountEndTime(Store store, Product product, DateTime _EndDate);

        /**
         * Store Managment, Purches
         **/
        LotteryTicket MakeALotteryPurches(Store store, Product product, int moeny);
        Product MakeAImmidiatePurches(Store store, Product product);


    }
    public enum StoreEnum
    {
        Success,
        UpdateStockFail,
        OpenStoreFail,
        AddStoreOwnerFail,
        AddStoreManagerFail,
        CloseStoreFail,
        ChangePurchesTypeFail
    }
}
