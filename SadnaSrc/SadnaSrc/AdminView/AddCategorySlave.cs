using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.AdminView
{
    public class AddCategorySlave
    {
        public MarketAnswer Answer;
        private readonly IAdminDL _adminDlInstacne;
        public AddCategorySlave( IAdminDL adminDl)
        {
            _adminDlInstacne = adminDl;
        }

        public Category AddCategory(string categoryName)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to add category to the store");
                MarketLog.Log("StoreCenter", " check if category name exists");
                CheckIfCategoryExists(categoryName);
                MarketLog.Log("StoreCenter", " adding category");
                Category category = new Category(categoryName);
                _adminDlInstacne.AddCategory(category);
                Answer = new AdminAnswer(EditCategoryStatus.Success, "category"+categoryName+" added successfully");
                return category;
            }
            catch (AdminException e)
            {
                Answer = new AdminAnswer((EditCategoryStatus)e.Status,e.GetErrorMessage());
                return null;
            }
        }

        private void CheckIfCategoryExists(string categoryName)
        {
            Category category = _adminDlInstacne.GetCategoryByName(categoryName);
            if (category != null)
            {
                throw new AdminException(EditCategoryStatus.CategoryAlradyExist, "category exists in the store");
            }
        }
    }
}
