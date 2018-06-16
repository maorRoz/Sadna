using Castle.Core.Internal;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.AdminView
{
    public class AddCategorySlave
    {
        public MarketAnswer Answer;
        private readonly IUserAdmin _admin;
        private readonly IAdminDL _adminDlInstacne;
        public AddCategorySlave( IAdminDL adminDl, IUserAdmin admin)
        {
            _admin = admin;
            _adminDlInstacne = adminDl;
        }

        public void AddCategory(string categoryName)
        {
            try
            {
                MarketLog.Log("StoreCenter", "trying to add category to the store");
                _admin.ValidateSystemAdmin();
                MarketLog.Log("StoreCenter", " check if category name exists");
                CheckIfCategoryExists(categoryName);
                MarketLog.Log("StoreCenter", " adding category");
                if (categoryName.IsNullOrEmpty())
                {
                    Answer = new AdminAnswer(EditCategoryStatus.InvalidCategory, "The category name is empty!");
                    return;
                }
                Category category = new Category(categoryName);
                _adminDlInstacne.AddCategory(category);
                Answer = new AdminAnswer(EditCategoryStatus.Success, "Category " + categoryName + " added.");
            }
            catch (AdminException e)
            {
                Answer = new AdminAnswer((EditCategoryStatus) e.Status, e.GetErrorMessage());
            }
            catch (DataException e)
            {
                Answer = new AdminAnswer((EditCategoryStatus)e.Status, e.GetErrorMessage());
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
