using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MarketServer.Models;
using MarketWeb.Models;
using Microsoft.AspNetCore.Mvc;
using SadnaSrc.Main;
using static MarketWeb.Models.StoreListModel;

namespace MarketWeb.Controllers
{
	public class StoreManagementController : Controller
	{
		private const int Success = 0;

		public IActionResult StoreControl(int systemId, string state, string message, bool valid)
		{
			ViewBag.valid = valid;
			var userService = EnterController.GetUserSession(systemId);
		    var storesData = new string[0];
            var answer = userService.GetControlledStoreNames();
		    if (answer.Status == Success)
		    {
		        storesData = answer.ReportList;
				valid = true;
		    }
		    else
		    {
		        message = answer.Answer;
				valid = false;
		    }

		    return View(new StoreListModel(systemId, state, storesData,message));
		}

		public IActionResult ManageStoreOptions(int systemId, string state, string message, string store)
		{
			var userService = EnterController.GetUserSession(systemId);
			var answer = userService.GetStoreManagerPolicies(store);
		    if (answer.Status != Success)
		    {
                return RedirectToAction("StoreControl", new { systemId, state, message = answer.Answer });
            }
			string[] options = {"Manage Products", "Promote Store Admins", "Manage Store Discounts",
			    "View Purchase History", "View Promotion History", "Manage Store Purchase-Policy"};
			if (!answer.ReportList.Contains("StoreOwner"))
			{
				options = answer.ReportList;

			}

			return View(new PermissionOptionsModel(systemId, state, message, store, options));
		}

		public IActionResult OpenStoreView(int systemId, string state, string message)
		{
			return View(new UserModel(systemId, state, message));
		}

		public IActionResult OpenStore(int systemId, string state, string storeName, string storeAddress)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
			var answer = storeShoppingService.OpenStore(storeName, storeAddress);
			if (answer.Status == Success)
			{
				return RedirectToAction("StoreControl", new { systemId, state, message = answer.Answer, valid = true });
			}

			return RedirectToAction("OpenStoreView", new { systemId, state , message = answer.Answer});
		}

		public IActionResult ManageStore(int systemId, string state, string store, string option)
		{
			var userService = EnterController.GetUserSession(systemId);
			var answer = userService.GetStoreManagerPolicies(store);
			string[] userPolicies = answer.ReportList;
			if (userPolicies.Contains(option) || userPolicies.Contains("StoreOwner"))
			{
				return RedirectToAction(SetPermissionNameToActionName(option), new {systemId, state, store});
			}

			return RedirectToAction("ManageStoreOptions",
				new {systemId, state, message = "The user doesn't have the permission to operate this action!", store});
		}

	    private string SetPermissionNameToActionName(string optionName)
	    {
	        switch (optionName)
	        {
                case "Manage Products": return "ManageProducts";
                case "Promote Store Admins": return "PromoteStoreAdmin";
                case "Manage Store Discounts": return "DeclareDiscountPolicy";
                case "View Purchase History": return "ViewPurchaseHistory";
                case "View Promotion History": return "ViewPromotionHistory";
                default: return "StorePurchasePolicyPage";

            }
	    }

		public IActionResult ManageProducts(int systemId, string state, string message, string store)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
			var answer = storeShoppingService.ViewStoreStockAll(store);
		    if (answer.Status == Success)
		    {
		        return View(new StorePorductListModel(systemId, state, message, store, answer.ReportList));
		    }

		    return RedirectToAction("StoreControl", new { systemId, state, message = answer.Answer });
        }

		public IActionResult RemoveProduct(int systemId, string state, string store, string product)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.RemoveProduct(product);
		    if (answer.Status == Success)
		    {
		        return RedirectToAction("ManageProducts", new {systemId, state, store});
		    }

		    return RedirectToAction("StoreControl", new { systemId, state, message = answer.Answer });
        }

		public IActionResult EditProductPage(int systemId, string state, string message, string store, string product)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.GetProductInfo(product);
			return View(new ProductInfoModel(systemId, state, message, store, product, answer.ReportList[0]));
		}

		public IActionResult EditProduct(int systemId, string state, string store, string product, string productNewName,
			string basePrice, string description)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.EditProduct(product, productNewName, basePrice, description);
			return answer.Status == Success ? 
			    RedirectToAction("ManageProducts", new {systemId, state, message = answer.Answer, store}) :
			    RedirectToAction("EditProductPage", new {systemId, state, message = answer.Answer, store, product});
		}

		public IActionResult AddQuanitityToProduct(int systemId, string state, string store, string product,int quantity)

		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.AddQuanitityToProduct(product, quantity);
		    return answer.Status == Success ? 
		        RedirectToAction("ManageProducts", new {systemId, state, message = answer.Answer, store}) :
		        RedirectToAction("StoreControl", new { systemId, state, message = answer.Answer });
		}

		public IActionResult AddNewProductPage(int systemId, string state, string message, string store)
		{
			return View(new StoreItemModel(systemId, state, message, store));
		}

		public IActionResult AddNewProduct(int systemId, string state, string store, string product, double price,
			string description, int quantity)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.AddNewProduct(product, price, description, quantity);
			if (answer.Status == 0)
			{
				return RedirectToAction("ManageProducts", new {systemId, state, message = answer.Answer, store});
			}

			return RedirectToAction("AddNewProductPage", new {systemId, state, message = answer.Answer, store});
		}

		public IActionResult AddNewLotteryPage(int systemId, string state, string message, string store)
		{
			return View(new StoreItemModel(systemId, state, message, store));
		}

		public IActionResult AddNewLottery(int systemId, string state, string store, string product, double price,
			string description, DateTime startDate, DateTime endDate)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.AddNewLottery(product, price, description, startDate,endDate);
			if (answer.Status == 0)
			{
				return RedirectToAction("ManageProducts", new { systemId, state, message = answer.Answer, store });
			}

			return RedirectToAction("AddNewLotteryPage", new { systemId, state, message = answer.Answer, store });
		}

		public IActionResult ViewPurchaseHistory(int systemId, string state, string store)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.ViewStoreHistory();
		    if (answer.Status == Success)
		    {
		        return View(new PurchaseHistoryModel(systemId, state, answer.ReportList));
            }

		    return RedirectToAction("StoreControl", new { systemId, state, message = answer.Answer });
        }

	    public IActionResult ViewPromotionHistory(int systemId, string state, string store)
	    {
	        var userService = EnterController.GetUserSession(systemId);
	        var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
	        var answer = storeManagementService.ViewPromotionHistory();
	        if (answer.Status == Success)
	        {
	            return View(new PromotionHistoryModel(systemId, state,store, answer.ReportList));
	        }

	        return RedirectToAction("StoreControl", new { systemId, state, message = answer.Answer });
	    }

        public IActionResult DeclareDiscountPolicy(int systemId, string state, string message, string store, bool valid)
		{
			ViewBag.valid = valid;
			var userService = EnterController.GetUserSession(systemId);
			var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
			var answer = storeShoppingService.ViewStoreStockAll(store);
		    if (answer.Status == Success)
		    {
		        return View(new StorePorductListModel(systemId, state, message, store, answer.ReportList));
            }

		    return RedirectToAction("StoreControl", new { systemId, state, message = answer.Answer });
        }

		public IActionResult AddDiscountPage(int systemId, string state, string message, string store, string product)
		{
			return View(new ProductInStoreModel(systemId, state, message, store, product));
		}

		public IActionResult AddDiscount(int systemId, string state, string store, string productName,
			DateTime startDate, DateTime endDate, int discountAmount, string discountType, string presenteges)
		{
			bool precent = presenteges == "Yes";
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.AddDiscountToProduct(productName, startDate, endDate, discountAmount,
				discountType, precent);
			if (answer.Status == Success)
			{
				return RedirectToAction("DeclareDiscountPolicy", new {systemId, state, message = answer.Answer, store, valid = true});
			}

			return RedirectToAction("AddDiscountPage", new {systemId, state, message = answer.Answer, store, productName});
		}

		public IActionResult EditDiscountPage(int systemId, string state, string message, string store, string product)
		{
			return View(new ProductInStoreModel(systemId, state, message, store, product));
		}

		public IActionResult EditDiscount(int systemId, string state, string store, string product,
		     string discountCode, bool isHidden, string startDate, string endDate, string discountAmount, bool isPercentage)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
		    var answer = storeManagementService.EditDiscount(product, discountCode, isHidden, startDate, endDate, discountAmount, isPercentage);
			if (answer.Status == Success)
			{
				return RedirectToAction("DeclareDiscountPolicy", new {systemId, state, message = answer.Answer, store, valid = true});
			}
            
			return RedirectToAction("EditDiscountPage", new {systemId, state, message = answer.Answer ,store, product});
		}

		public IActionResult RemoveDiscount(int systemId, string state, string store, string product)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.RemoveDiscountFromProduct(product);
			if (answer.Status != Success)
			{
				return RedirectToAction("DeclareDiscountPolicy", new { systemId, state, message = answer.Answer, store, product , valid = false});
			}
			return RedirectToAction("DeclareDiscountPolicy", new {systemId, state, message = answer.Answer, store, product, valid = true});
		}

		public IActionResult PromoteStoreAdmin(int systemId, string state, string message, string store, bool valid)
		{
		    var options = new List<CheckBoxModel>
		    {
		        new CheckBoxModel {Name = "Store Owner"},
		        new CheckBoxModel {Name = "Can Manage Products"},
		        new CheckBoxModel {Name = "Can Promote Store Admin"},
		        new CheckBoxModel {Name = "Can Manage Discounts"},
		        new CheckBoxModel {Name = "Can View Purchase History"}
		    };
			ViewBag.valid = valid;
		    CheckBoxListModel optionsModel = new CheckBoxListModel(systemId, state, message, store) {Items = options };
		    return View(optionsModel);
		}

		public IActionResult HandleOptionsSelected(int systemId, string state, string store, string[] permissions, string usernameEntry)
		{
		    var actions = "";
			foreach (var permission in permissions)
			{
				if (permission != null)
				{
				    actions += SetPermissionNameToPermissionValue(permission) + ",";
				}
			}
			return RedirectToAction("PromoteStoreAdminCall", new { systemId, state, store , usernameEntry, actions});
		}

	    private string SetPermissionNameToPermissionValue(string permission)
	    {
	        switch (permission)
	        {
                case "Store Owner": return "StoreOwner";
                case "Can Manage Products": return "ManageProducts";
                case "Can Promote Store Admin": return "PromoteStoreAdmin";
                case "Can Manage Discounts": return "DeclareDiscountPolicy";
                default: return "ViewPurchaseHistory";
            }
	    }

		public IActionResult PromoteStoreAdminCall(int systemId, string state,string store, string usernameEntry,
			string actions)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
		    if (actions == null)
		    {
		        actions = "";
		    }
			var answer = storeManagementService.PromoteToStoreManager(usernameEntry, actions);
			if (answer.Status == Success)
			{
				return RedirectToAction("PromoteStoreAdmin", new { systemId, state, message = answer.Answer, store, valid = true });
			}
			return RedirectToAction("PromoteStoreAdmin", new { systemId, state, message = answer.Answer, store , valid= false});
		}

		public IActionResult HandleCategoryProduct(int systemId, string state, string message, string store, string product)
		{
			return View(new ProductInStoreModel(systemId, state, message, store, product));
		}

		public IActionResult AddingProductCategoryPage(int systemId, string state,string message, string store, string product, bool valid)
		{
			ViewBag.valid = valid;
			var userService = EnterController.GetUserSession(systemId);
			var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
			string[] categories = storeShoppingService.GetAllCategoryNames().ReportList;
			return View(new ProductInStoreCategoriesModel(systemId, state, message,store,product, categories));
		}

		public IActionResult AddCategoryProduct(int systemId, string state,string store, string product, string category)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.AddProductToCategory(category, product);
			if (answer.Status == Success)
			{
				return RedirectToAction("AddingProductCategoryPage", new { systemId, state, message = answer.Answer,store, product, valid = true });
			}
			return RedirectToAction("AddingProductCategoryPage", new { systemId, state, message = answer.Answer, store, product, valid = false });
		}

		public IActionResult RemovingProductCategoryPage(int systemId, string state, string message,string store, string product, bool valid)
		{
			ViewBag.valid = valid;
			var userService = EnterController.GetUserSession(systemId);
			var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
			string[] categories = storeShoppingService.GetAllCategoryNames().ReportList;
			return View(new ProductInStoreCategoriesModel(systemId, state, message, store, product, categories));
		}

		public IActionResult RemoveCategoryProduct(int systemId, string state, string store, string product, string category)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.RemoveProductFromCategory(category,product);
			if (answer.Status == Success)
			{
				return RedirectToAction("RemovingProductCategoryPage", new { systemId, state, message = answer.Answer,store,product, valid = true });
			}
			return RedirectToAction("RemovingProductCategoryPage", new { systemId, state, message = answer.Answer,store, product, valid = false });
		}


		public IActionResult StorePurchasePolicyPage(int systemId, string state,string message,string store, bool valid)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var conditions = new string[0];
			var operators = new[] { "AND", "OR", "NOT" };
			ViewBag.valid = valid;
			var answer = storeManagementService.ViewPolicies(store);
			if (answer.Status == Success)
			{
				conditions = answer.ReportList;
			}
			else
			{
				message = answer.Answer;
			}

			return View(new StorePurchasePolicyModel(systemId, state, message, store, operators, conditions));
		}

	    public IActionResult AddPurchasePolicy(int systemId, string state, string message, string store, bool valid)
	    {
	        var userService = EnterController.GetUserSession(systemId);
	        var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
	        var conditions = new string[0];
	        var operators = new[] { "AND", "OR", "NOT" };
	        ViewBag.valid = valid;
	        var answer = storeManagementService.ViewPoliciesSessions();
	        if (answer.Status == Success)
	        {
	            conditions = answer.ReportList;
	        }
	        else
	        {
	            message = answer.Answer;
	        }

	        return View(new StorePurchasePolicyModel(systemId, state, message, store, operators, conditions));
	    }

        public IActionResult CreatePolicy(int systemId, string state,string store, string type, string subject, string op, string arg1, string optArg, string usernameText, string addressText, string quantityOp, string quantityText, string priceOp, string priceText, string subject1, string type1)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			if (usernameText != null)
			{
				var answer = storeManagementService.CreatePolicy(type, store, subject, "Username =", usernameText, optArg);
				if (answer.Status != Success)
					return RedirectToAction("AddPurchasePolicy", new { systemId, state, message = answer.Answer,store });
			}
			else if (addressText != null)
			{
				var answer = storeManagementService.CreatePolicy(type, store, subject, "Address =", addressText, optArg);
				if (answer.Status != Success)
					return RedirectToAction("AddPurchasePolicy", new { systemId, state, message = answer.Answer,store });
			}

			else if (quantityText != null)
			{
				var answer = storeManagementService.CreatePolicy(type, store, subject, "Quantity " + quantityOp, quantityText, optArg);
				if (answer.Status != Success)
					return RedirectToAction("AddPurchasePolicy", new { systemId, state, message = answer.Answer ,store});
			}
			else if (priceText != null)
			{
				var answer = storeManagementService.CreatePolicy(type, store, subject, "Price " + priceOp, priceText, optArg);
				if (answer.Status != Success)
					return RedirectToAction("AddPurchasePolicy", new { systemId, state, message = answer.Answer,store });
			}     
			else
			{
			    if (arg1 == null)
			        return RedirectToAction("AddPurchasePolicy", new { systemId, state, store });
                string[] id1 = arg1.Split('|');
				string[] id2 = null;
				if (optArg != null)
				{
					id2 = optArg.Split('|');
					var answer = storeManagementService.CreatePolicy(type, store, subject, op, id1[0], id2[0]);
					if (answer.Status != Success)
						return RedirectToAction("AddPurchasePolicy", new { systemId, state, message = answer.Answer ,store});
				}
				else
				{
					var answer = storeManagementService.CreatePolicy(type, store, subject, op, id1[0], null);
					if (answer.Status != Success)
						return RedirectToAction("AddPurchasePolicy", new { systemId, state, message = answer.Answer ,store});
				}
			}
			return RedirectToAction("AddPurchasePolicy", new { systemId, state, store });
		}

		public IActionResult SavePolicy(int systemId, string state, string store)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.SavePolicy();
			return RedirectToAction("StorePurchasePolicyPage", new { systemId, state, message = answer.Answer, valid = answer.Status == Success, store});
		}

	    public IActionResult RemovePolicy(int systemId, string state, string store, string type, string subject, string optProd)
	    {
	        var userService = EnterController.GetUserSession(systemId);
	        var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
	        var answer = storeManagementService.RemovePolicy(type, subject, optProd);
	        return RedirectToAction("StorePurchasePolicyPage", new { systemId, state, message = answer.Answer, valid = answer.Status == Success, store});
	    }


        public IActionResult CategoryDiscountMenu(int systemId, string state, string message,string store, bool valid)
		{
			ViewBag.valid = valid;
			return View(new StoreItemModel(systemId,state,message,store));
		}

		public IActionResult AddCategoryDiscountPage(int systemId, string state, string message, string store)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
			string[] categories = storeShoppingService.GetAllCategoryNames().ReportList;
			return View(new CategoryStorelistModel(systemId, state, message, store, categories));
		}

		public IActionResult AddCategoryDiscount(int systemId, string state, string store, string categoryName,
			DateTime startDate, DateTime endDate, int discountAmount)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.AddCategoryDiscount(categoryName, startDate, endDate, discountAmount);
			if (answer.Status == Success)
			{
				return RedirectToAction("CategoryDiscountMenu", new { systemId, state, message = answer.Answer, store, valid = true });
			}

			return RedirectToAction("AddCategoryDiscountPage", new { systemId, state, message = answer.Answer, store});
		}

		public IActionResult EditCategoryDiscountPage(int systemId, string state, string message, string store)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
			string[] categories = storeShoppingService.GetAllDiscountCategoriesInStore(store).ReportList;
			return View(new CategoryStorelistModel(systemId, state, message, store, categories));
		}

		public IActionResult EditCategoryDiscount(int systemId, string state, string store, string categoryName,
			string whatToEdit, string newValue)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.EditCategoryDiscount(categoryName, whatToEdit, newValue);
			if (answer.Status == Success)
			{
				return RedirectToAction("CategoryDiscountMenu", new { systemId, state, message = answer.Answer, store, valid = true });
			}

			return RedirectToAction("EditCategoryDiscountPage", new { systemId, state, message = answer.Answer, store});
		}

		public IActionResult RemoveCategoryDiscountPage(int systemId, string state, string message, string store, bool valid)
		{
			ViewBag.valid = valid;
			var userService = EnterController.GetUserSession(systemId);
			var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
			string[] categories = storeShoppingService.GetAllDiscountCategoriesInStore(store).ReportList;
			return View(new CategoryStorelistModel(systemId, state, message, store, categories));
		}

		public IActionResult RemoveCategoryDiscount(int systemId, string state, string store, string categoryName)
		{
			var userService = EnterController.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.RemoveCategoryDiscount(categoryName);
			if (answer.Status != Success)
			{
				return RedirectToAction("RemoveCategoryDiscountPage", new { systemId, state, message = answer.Answer, store, valid = false });
			}
			return RedirectToAction("CategoryDiscountMenu", new { systemId, state, message = answer.Answer, store, valid = true });
		}

	}
}

