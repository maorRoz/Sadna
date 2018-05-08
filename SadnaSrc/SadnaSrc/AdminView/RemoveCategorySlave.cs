using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.AdminView
{
    public class RemoveCategorySlave
    {
        public MarketAnswer Answer { get; set; }
        private readonly IAdminDL _adminDlInstacne;
        public RemoveCategorySlave(IAdminDL adminDl)
        {
            _adminDlInstacne = adminDl;
        }
        public void RemoveCategory(string categoryName)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to remove category from the system");
                MarketLog.Log("StoreCenter", " check if category name exists");
                CheckIfCategoryExists(categoryName);
                MarketLog.Log("StoreCenter", " removing category");
                Category category = _adminDlInstacne.GetCategoryByName(categoryName);
                _adminDlInstacne.RemoveCategory(category);
                Answer = new AdminAnswer(EditCategoryStatus.Success, "category" + categoryName + " removed successfully");
            }
            catch (AdminException e)
            {
                Answer = new AdminAnswer((EditCategoryStatus)e.Status,e.GetErrorMessage());
            }
        }

        private void CheckIfCategoryExists(string categoryName)
        {
            Category category = _adminDlInstacne.GetCategoryByName(categoryName);
            if (category == null)
            {
                throw new AdminException(EditCategoryStatus.CategoryNotExistsInSystem, "category not exists in the system");
            }
        }

    }
}