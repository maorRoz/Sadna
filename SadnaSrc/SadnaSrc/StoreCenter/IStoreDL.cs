using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public interface IStoreDL
    {
        void EditProductInDatabase(Product product);
        int GetUserIDFromUserName(string userName);
        Store GetStorebyName(string storeName);
        StockListItem GetStockListItembyProductID(string product);
        bool IsStoreExistAndActive(string store);
        string[] GetStoreInfo(string store);
        void AddLotteryTicket(LotteryTicket lottery);
        LotteryTicket GetLotteryTicket(string ticketID);
        LotterySaleManagmentTicket GetLotteryByProductID(string productID);
        void EditLotteryInDatabase(LotterySaleManagmentTicket lotteryManagment);
        void AddLottery(LotterySaleManagmentTicket lotteryManagment);
        string[] GetAllStoreProductsID(string systemID);
        void RemoveLottery(LotterySaleManagmentTicket lotteryManagment);
        void RemoveStockListItem(StockListItem stockListItem);
        void EditDiscountInDatabase(Discount discount);
        void EditStockInDatabase(StockListItem stockListItem);
        void AddStore(Store toAdd);
        string[] GetHistory(Store store);
        void AddDiscount(Discount discount);
        void AddStockListItemToDataBase(StockListItem stockListItem);
        void RemoveDiscount(Discount discount);
        Product GetProductByNameFromStore(string storeName, string ProductName);
        StockListItem GetProductFromStore(string store, string productName);
        LotterySaleManagmentTicket GetLotteryByProductNameAndStore(string storeName, string productName);
        Category GetCategoryByName(string categoryName);
        LinkedList<Product> GetAllCategoryProducts(string categoryid);
        void AddProductToCategory(string CategoryID, string ProductID);
        void RemoveProductFromCategory(string CategoryID, string ProductID);
        void AddPromotionHistory(string store,string managerName,string promotedName,string[] permissions,string description);
        string[] GetPromotionHistory(string store);
    }
    }
