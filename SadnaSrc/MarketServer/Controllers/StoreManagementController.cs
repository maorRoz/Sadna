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
			var userService = MarketServer.Users[systemId];
			string[] storesData = userService.GetControlledStoreNames().ReportList;
			return View(new StoreListModel(systemId, state, storesData));
		}

		public IActionResult ManageStoreOptions(int systemId, string state, string message, string store)
		{
			var userService = MarketServer.Users[systemId];
			var answer = userService.GetStoreManagerPolicies(store);
			string[] options = { "ManageProducts", "PromoteStoreAdmin", "DeclareDiscountPolicy", "ViewPurchaseHistory"};
			if (!answer.ReportList.Contains("StoreOwner"))
			{
				options = answer.ReportList;

			}

			return View(new PermissionOptionsModel(systemId, state, message, store, options));
		}

		public IActionResult ManageStore(int systemId, string state, string store, string option)
		{
			var userService = MarketServer.Users[systemId];
			var answer = userService.GetStoreManagerPolicies(store);
			string[] userPolicies = answer.ReportList;
			if (userPolicies.Contains(option) || userPolicies.Contains("StoreOwner"))
			{
				return RedirectToAction(option, new {systemId, state, message = "", store});
			}

			return RedirectToAction("ManageStoreOptions",
				new {systemId, state, message = "The user doesn't have the permission to operate this action!", store});
		}

		public IActionResult ManageProducts(int systemId, string state, string message, string store)
		{
			var userService = MarketServer.Users[systemId];
			var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
			var answer = storeShoppingService.ViewStoreStockAll(store);
			return View(new StorePorductListModel(systemId, state, message, store, answer.ReportList));
		}

		public IActionResult RemoveProduct(int systemId, string state, string store, string product)
		{
			var userService = MarketServer.Users[systemId];
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.RemoveProduct(product);
			return RedirectToAction("ManageProducts", new {systemId, state, message = "", store});

		}

		public IActionResult EditProductPage(int systemId, string state, string message, string store, string product)
		{
			return View(new ProductInStoreModel(systemId, state, message, store, product));
		}

		public IActionResult EditProduct(int systemId, string state, string store, string product, string whatToEdit,
			string newValue)
		{
			var userService = MarketServer.Users[systemId];
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.EditProduct(product, whatToEdit, newValue);
			if (answer.Status == 0)
			{
				return RedirectToAction("ManageProducts", new {systemId, state, message = answer.Answer, store});
			}

			return RedirectToAction("EditProductPage", new {systemId, state, message = answer.Answer, store, product});
		}

		public IActionResult AddQuanitityToProduct(int systemId, string state, string store, string product, int quantity)

		{
			var userService = MarketServer.Users[systemId];
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.AddQuanitityToProduct(product, 1);
			return RedirectToAction("ManageProducts", new {systemId, state, message = answer.Answer, store});
		}

		public IActionResult AddNewProductPage(int systemId, string state, string message, string store)
		{
			return View(new StoreItemModel(systemId, state, message, store));
		}

		public IActionResult AddNewProduct(int systemId, string state, string store, string product, double price,
			string description, int quantity)
		{
			var userService = MarketServer.Users[systemId];
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
			var userService = MarketServer.Users[systemId];
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.AddNewLottery(product, price, description, startDate,endDate);
			if (answer.Status == 0)
			{
				return RedirectToAction("ManageProducts", new { systemId, state, message = answer.Answer, store });
			}

			return RedirectToAction("AddNewLotteryPage", new { systemId, state, message = answer.Answer, store });
		}

		public IActionResult ViewPurchaseHistory(int systemId, string state, string message, string store)
		{
			var userService = MarketServer.Users[systemId];
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.ViewStoreHistory();
			return View(new PurchaseHistoryModel(systemId, state, answer.ReportList));
		}

		public IActionResult DeclareDiscountPolicy(int systemId, string state, string message, string store, bool valid)
		{
			ViewBag.valid = valid;
			var userService = MarketServer.Users[systemId];
			var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
			var answer = storeShoppingService.ViewStoreStockAll(store);
			return View(new StorePorductListModel(systemId, state, message, store, answer.ReportList));
		}

		public IActionResult AddDiscountPage(int systemId, string state, string message, string store, string product)
		{
			return View(new ProductInStoreModel(systemId, state, message, store, product));
		}

		public IActionResult AddDiscount(int systemId, string state, string message, string store, string productName,
			DateTime startDate, DateTime endDate, int discountAmount, string discountType, string presenteges)
		{
			bool precent = presenteges == "Yes";
			var userService = MarketServer.Users[systemId];
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

		public IActionResult EditDiscount(int systemId, string state, string message, string store, string product,
			string whatToEdit, string newValue)
		{
			var userService = MarketServer.Users[systemId];
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.EditDiscount(product, whatToEdit, newValue);
			if (answer.Status == Success)
			{
				return RedirectToAction("DeclareDiscountPolicy", new {systemId, state, message = answer.Answer, store, valid = true});
			}

			return RedirectToAction("EditDiscountPage", new {systemId, state, message = answer.Answer, store, product});
		}

		public IActionResult RemoveDiscount(int systemId, string state, string message, string store, string product)
		{
			var userService = MarketServer.Users[systemId];
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
		public IActionResult HandleOptionsSelected(int systemId, string state, string message, string store, string[] permissions, string usernameEntry)
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

		public IActionResult PromoteStoreAdminCall(int systemId, string state,string message, string store, string usernameEntry,
			string actions)
		{
			var userService = MarketServer.Users[systemId];
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
			var userService = MarketServer.Users[systemId];
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
			var userService = MarketServer.Users[systemId];
			var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
			var answer = storeManagementService.RemoveProductFromCategory(category,product);
			if (answer.Status == Success)
			{
				return RedirectToAction("RemovingProductCategoryPage", new { systemId, state, message = answer.Answer,store,product, valid = true });
			}
			return RedirectToAction("RemovingProductCategoryPage", new { systemId, state, message = answer.Answer,store, product, valid = false });
		}


		public IActionResult PurchasePolicy(int systemId, string state, string message, string store)
		{
			return View(new StoreItemModel(systemId, state, message, store));
		}
		

	}
}
