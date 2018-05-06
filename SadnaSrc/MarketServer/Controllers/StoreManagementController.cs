using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketWeb.Models;
using Microsoft.AspNetCore.Mvc;
using SadnaSrc.Main;
using static MarketWeb.Models.StoreListModel;

namespace MarketWeb.Controllers
{
    public class StoreManagementController : Controller
    {
        public IActionResult StoreControl(int systemId, string state)
        {
            var userService = MarketServer.users[systemId];
            string[] storesData = userService.GetControlledStoreNames().ReportList;
            return View(new StoreListModel(systemId, state, storesData));
        }

	    public IActionResult ManageStoreOptions(int systemId, string state,string message, string store)
	    {
		    return View(new StoreItemModel(systemId,state,message,store));
	    }

	    public IActionResult ManageStore(int systemId, string state, string store, string option)
	    {
		    var userService = MarketServer.users[systemId];
		    var answer = userService.GetStoreManagerPolicies(store);
			string[] userPolicies = answer.ReportList;
		    if (userPolicies.Contains(option))
		    {
			    return RedirectToAction(option, new { systemId, state});
			}
			return RedirectToAction("ManageStoreOptions", new { systemId, state , message = "The user doesn't have the permission to operate this action!",store});
		}

	    public IActionResult ManageProducts(int systemId, string state, string message, string store)
	    {
		    var userService = MarketServer.users[systemId];
		    var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
		    var answer = storeShoppingService.ViewStoreStock(store);
			return View(new StorePorductListModel(systemId, state, message, store, answer.ReportList));
		}

	    public IActionResult RemoveProduct(int systemId, string state, string store, string product)
	    {
			var userService = MarketServer.users[systemId];
		    var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
		    var answer = storeManagementService.RemoveProduct(product);
			return RedirectToAction("ManageProducts" , new { systemId, state , message = "", store});

		}

	    public IActionResult EditProduct()
	    {
		    return null;
	    }

	    public IActionResult AddQuanitityToProduct()
	    {
		    return null;
	    }

	    public IActionResult AddNewProduct()
	    {
		    return null;
	    }
	}
}
