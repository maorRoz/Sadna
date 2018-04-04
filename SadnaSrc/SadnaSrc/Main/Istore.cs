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
        MarketAnswer PromoteToOwner(int currentUserID, int someoneToPromoteID);
        MarketAnswer PromoteToManager(int currentUserID,int someoneToPromoteID);
        LinkedList<string> getAllStoreProducts();
        LinkedList<string> ViewPurchesHistory();
        MarketAnswer CloseStore(int ownerOrSystemAdmin);
        bool IsStoreActive();
        bool IsOwner(int user);
        string getProductById(string ID); //will return null if product is not exists
        string getProductDiscountByProductID(string ID); //will return null if product is not exists or discount not exists
        string getProductPurchaseWayByProductID(string ID);//will return PRODUCTNOTFOUND if product is not exists
        int getProductQuantitybyProductID(string ID);//will return -1 if product is not exists
        bool canPurchaseImmediate(string productName, int quantity);
        bool canPurchaseLottery(string productName, int amountOfMoney);

        /**
         * Store Managment, handling products
         **/
        MarketAnswer AddProduct(String _name, int _price, String _description, int quantity);
        MarketAnswer IncreaseProductQuantity(string productName, int quantity);
        MarketAnswer removeProduct(string productName);
        MarketAnswer editProductPrice(string productName, int newprice);
        MarketAnswer editProductName(string productName, String Name);
        MarketAnswer editProductDescripiton(string productName, String Desccription);

        /**
         * Store Managment, handling PurchaseWay
         **/

        MarketAnswer ChangeProductPurchaseWayToImmediate(string productName);
        MarketAnswer ChangeProductPurchaseWayToLottery(string productName, DateTime StartDate, DateTime EndDate);
        /**
         * Store Managment, Discounts
         **/

        MarketAnswer addDiscountToProduct_VISIBLE(string productName, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer addDiscountToProduct_HIDDEN(string productName, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer addDiscountToProduct_presenteges_VISIBLE(string productName, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer addDiscountToProduct_presenteges_HIDDEN(string productName, DateTime _startDate, DateTime _EndDate, int _DiscountAmount);
        MarketAnswer EditDiscountToPrecenteges(string productName);
        MarketAnswer EditDiscountToNonPrecenteges(string productName);
        MarketAnswer EditDiscountToHidden(string productName);
        MarketAnswer EditDiscountToVisible(string productName);
        MarketAnswer EditDiscountAmount(string productName, int amount);
        MarketAnswer EditDiscountStartTime(string productName, DateTime _startDate);
        MarketAnswer EditDiscountEndTime(string productName, DateTime _EndDate);
        MarketAnswer removeDiscountFormProduct(string productName);

        /**
         * Store Managment, Purchase
         **/
        string MakeALotteryPurchase(string productName, int moeny);
        string MakeAImmediatePurchase(string productName, int quantity);
        string DoLottery(string productName); // will return the lottery ticket of the winner, or null if faild
        double getProductPrice(string _product, int _DiscountCode, int _quantity);
    }
    public enum PurchaseEnum { IMMEDIATE, LOTTERY, PRODUCTNOTFOUND }
public enum discountTypeEnum { HIDDEN, VISIBLE };
public enum LotteryTicketStatus { WAITING, WINNING, LOSING, CANCEL};
}
