using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.Core;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.AdminView
{
    public interface IAdminDL
    {
        string[] GetPurchaseHistory(string field, string givenValue);
        bool IsUserNameExistInHistory(string userName);
        bool IsStoreExistInHistory(string storeName);
        void DeleteUser(string userName);
        string[] FindSolelyOwnedStores();
        void CloseStore(string store);
        bool IsUserExist(string userName);
        void AddCategory(Category category);
        void RemoveCategory(Category category);
        Category GetCategoryByName(string categoryName);
	    string[] GetAllStoresInPurchaseHistory();
	    string[] GetAllUserInPurchaseHistory();

        string[] GetEventLogReport();

        string[] GetEventErrorLogReport();

	    Pair<int, DateTime>[] GetEntranceReport();

        Category[] GetAllCategories();
    }
}
