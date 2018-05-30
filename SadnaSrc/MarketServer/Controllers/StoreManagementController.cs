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

		public IActionResult StoreControl(int systemId, string state)
		{
			var userService = MarketServer.GetUserSession(systemId);
		    string message = null;
		    var storesData = new string[0];
            var answer = userService.GetControlledStoreNames();
		    if (answer.Status == Success)
		    {
		        storesData = answer.ReportList;
		    }
		    else
		    {
		        message = answer.Answer;
		    }

		    return View(new StoreListModel(systemId, state, storesData,message));
		}

		public IActionResult ManageStoreOptions(int systemId, string state, string message, string store)
		{
			var userService = MarketServer.GetUserSession(systemId);
			var answer = userService.GetStoreManagerPolicies(store);
		    if (answer.Status != Success)
		    {
		        return RedirectToAction("StoreControl", new { systemId, state, message = answer.Answer });
            }
			string[] options = {"ManageProducts", "PromoteStoreAdmin", "DeclareDiscountPolicy",
			    "ViewPurchaseHistory", "ViewPromotionHistory", "PurchasePolicy"};
			if (!answer.ReportList.Contains("StoreOwner"))
			{
				options = answer.ReportList;

			}

			return View(new PermissionOptionsModel(systemId, state, message, store, options));
		}

		public IActionResult ManageStore(int systemId, string state, string store, string option)
		{
			var userService = MarketServer.GetUserSession(systemId);
			var answer = userService.GetStoreManagerPolicies(store);
			string[] userPolicies = answer.ReportList;
			if (userPolicies.Contains(option) || userPolicies.Contains("StoreOwner"))
			{
				return RedirectToAction(option, new {systemId, state, store});
			}

			return RedirectToAction("ManageStoreOptions",
				new {systemId, state, message = "The user doesn't have the permission to operate this action!", store});
		}

		public IActionResult ManageProducts(int systemId, string state, string message, string store)
		{
			var userService = MarketServer.GetUserSession(systemId);
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
			var userService = MarketServer.GetUserSession(systemId);
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
			return View(new ProductInStoreModel(systemId, state, message, store, product));
		}

		public IActionResult EditProduct(int systemId, string state, string store, string product, string whatToEdit,
			string newValue)
		{
			var userService = MarketServer.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.EditProduct(product, whatToEdit, newValue);
			if (answer.Status == 0)
			{
				return RedirectToAction("ManageProducts", new {systemId, state, message = answer.Answer, store});
			}

			return RedirectToAction("EditProductPage", new {systemId, state, message = answer.Answer, store, product});
		}

		public IActionResult AddQuanitityToProduct(int systemId, string state, string store, string product)

		{
			var userService = MarketServer.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.AddQuanitityToProduct(product, 1);
		    if (answer.Status == Success)
		    {
		        return RedirectToAction("ManageProducts", new {systemId, state, message = answer.Answer, store});
		    }

		    return RedirectToAction("StoreControl", new { systemId, state, message = answer.Answer });
        }

		public IActionResult AddNewProductPage(int systemId, string state, string message, string store)
		{
			return View(new StoreItemModel(systemId, state, message, store));
		}

		public IActionResult AddNewProduct(int systemId, string state, string store, string product, double price,
			string description, int quantity)
		{
			var userService = MarketServer.GetUserSession(systemId);
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
			var userService = MarketServer.GetUserSession(systemId);
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
			var userService = MarketServer.GetUserSession(systemId);
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
	        var userService = MarketServer.GetUserSession(systemId);
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
			var userService = MarketServer.GetUserSession(systemId);
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
			var userService = MarketServer.GetUserSession(systemId);
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
			string whatToEdit, string newValue)
		{
			var userService = MarketServer.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.EditDiscount(product, whatToEdit, newValue);
			if (answer.Status == Success)
			{
				return RedirectToAction("DeclareDiscountPolicy", new {systemId, state, message = answer.Answer, store, valid = true});
			}

			return RedirectToAction("EditDiscountPage", new {systemId, state, message = answer.Answer, store, product});
		}

		public IActionResult RemoveDiscount(int systemId, string state, string store, string product)
		{
			var userService = MarketServer.GetUserSession(systemId);
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
		    List<CheckBoxModel> lst = new List<CheckBoxModel>
		    {
		        new CheckBoxModel {Name = "StoreOwner"},
		        new CheckBoxModel {Name = "ManageProducts"},
		        new CheckBoxModel {Name = "PromoteStoreAdmin"},
		        new CheckBoxModel {Name = "DeclareDiscountPolicy"},
		        new CheckBoxModel {Name = "ViewPurchaseHistory"}
		    };
			ViewBag.valid = valid;
		    CheckBoxListModel optionList = new CheckBoxListModel(systemId, state, message, store) {Items = lst};
		    return View(optionList);
		}

		[HttpPost]
		public IActionResult HandleOptionsSelected(int systemId, string state, string store, string[] permissions, string usernameEntry)
		{
			StringBuilder actions = new StringBuilder();
			foreach (var permission in permissions)
			{
				if (permission != null)
				{
					actions.Append(permission + ",");
				}
			}
			return RedirectToAction("PromoteStoreAdminCall", new { systemId, state, store , usernameEntry, actions});
		}

		public IActionResult PromoteStoreAdminCall(int systemId, string state,string store, string usernameEntry,
			string actions)
		{
			var userService = MarketServer.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
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
			return View(new ProductInStoreModel(systemId, state, message,store,product));
		}

		public IActionResult AddCategoryProduct(int systemId, string state,string store, string product, string category)
		{
			var userService = MarketServer.GetUserSession(systemId);
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
			return View(new ProductInStoreModel(systemId, state, message, store, product));
		}

		public IActionResult RemoveCategoryProduct(int systemId, string state, string store, string product, string category)
		{
			var userService = MarketServer.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.RemoveProductFromCategory(category,product);
			if (answer.Status == Success)
			{
				return RedirectToAction("RemovingProductCategoryPage", new { systemId, state, message = answer.Answer,store,product, valid = true });
			}
			return RedirectToAction("RemovingProductCategoryPage", new { systemId, state, message = answer.Answer,store, product, valid = false });
		}


		public IActionResult PurchasePolicy(int systemId, string state,string message,string store, bool valid)
		{
			var userService = MarketServer.GetUserSession(systemId);
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

			return View(new StorePurchasePolicyModel(systemId, state, message,store, operators, conditions));
		}

		public IActionResult CreatePolicy(int systemId, string state,string store, string type, string subject, string op, string arg1, string optArg, string usernameText, string addressText, string quantityOp, string quantityText, string priceOp, string priceText, string subject1, string type1)
		{
			var userService = MarketServer.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);

			if (usernameText != null)
			{
				var answer = storeManagementService.CreatePolicy(type, store, subject, "Username =", usernameText, optArg);
				if (answer.Status != Success)
				{
					return RedirectToAction("PurchasePolicy", new { systemId, state, message = answer.Answer,store });
				}

			}

			else if (addressText != null)
			{
				var answer = storeManagementService.CreatePolicy(type, store, subject, "Address =", addressText, optArg);
				if (answer.Status != Success)
				{
					return RedirectToAction("PurchasePolicy", new { systemId, state, message = answer.Answer,store });
				}
			}

			else if (quantityText != null)
			{
				var answer = storeManagementService.CreatePolicy(type, store, subject, "Quantity " + quantityOp, quantityText, optArg);
				if (answer.Status != Success)
				{
					return RedirectToAction("PurchasePolicy", new { systemId, state, message = answer.Answer ,store});
				}
			}

			else if (priceText != null)
			{
				var answer = storeManagementService.CreatePolicy(type, store, subject, "Price " + priceOp, priceText, optArg);
				if (answer.Status != Success)
				{
					return RedirectToAction("PurchasePolicy", new { systemId, state, message = answer.Answer,store });
				}
			}

			else
			{
				string[] id1 = arg1.Split(' ');
				string[] id2 = null;
				if (optArg != null)
				{
					id2 = optArg.Split(' ');
					var answer = storeManagementService.CreatePolicy(type1,store, subject1, op, id1[0], id2[0]);
					if (answer.Status != Success)
					{
						return RedirectToAction("PurchasePolicy", new { systemId, state, message = answer.Answer ,store});
					}
				}

				else
				{
					var answer = storeManagementService.CreatePolicy(type1, store, subject1, op, id1[0], null);
					if (answer.Status != Success)
					{
						return RedirectToAction("PurchasePolicy", new { systemId, state, message = answer.Answer ,store});
					}
				}

			}

			return RedirectToAction("PurchasePolicy", new { systemId, state, store });
		}

		public IActionResult SavePolicy(int systemId, string state)
		{
			var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
			var answer = adminService.SavePolicy();
			return RedirectToAction("PurchasePolicy", new { systemId, state, message = answer.Answer, valid = answer.Status == Success });
		}

		public IActionResult CategoryDiscountMenu(int systemId, string state, string message,string store, bool valid)
		{
			ViewBag.valid = valid;
			return View(new StoreItemModel(systemId,state,message,store));
		}

		public IActionResult AddCategoryDiscountPage(int systemId, string state, string message, string store)
		{
			return View(new StoreItemModel(systemId, state, message, store));
		}

		public IActionResult AddCategoryDiscount(int systemId, string state, string store, string categoryName,
			DateTime startDate, DateTime endDate, int discountAmount)
		{
			var userService = MarketServer.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.AddCategoryDiscount(categoryName, startDate, endDate, discountAmount);
			if (answer.Status == Success)
			{
				return RedirectToAction("DeclareDiscountPolicy", new { systemId, state, message = answer.Answer, store, valid = true });
			}

			return RedirectToAction("AddDiscountPage", new { systemId, state, message = answer.Answer, store, categoryName });
		}

		public IActionResult EditCategoryDiscountPage(int systemId, string state, string message, string store, string product)
		{
			return View(new ProductInStoreModel(systemId, state, message, store, product));
		}

		public IActionResult EditCategoryDiscount(int systemId, string state, string store, string product,
			string whatToEdit, string newValue)
		{
			var userService = MarketServer.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.EditDiscount(product, whatToEdit, newValue);
			if (answer.Status == Success)
			{
				return RedirectToAction("DeclareDiscountPolicy", new { systemId, state, message = answer.Answer, store, valid = true });
			}

			return RedirectToAction("EditDiscountPage", new { systemId, state, message = answer.Answer, store, product });
		}

		public IActionResult RemoveCategoryDiscount(int systemId, string state, string store, string product)
		{
			var userService = MarketServer.GetUserSession(systemId);
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.RemoveDiscountFromProduct(product);
			if (answer.Status != Success)
			{
				return RedirectToAction("DeclareDiscountPolicy", new { systemId, state, message = answer.Answer, store, product, valid = false });
			}
			return RedirectToAction("DeclareDiscountPolicy", new { systemId, state, message = answer.Answer, store, product, valid = true });
		}

	}
}

