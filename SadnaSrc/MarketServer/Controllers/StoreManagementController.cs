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
            var userService = MarketServer.Users[systemId];
	        string[] storesData;
	        if (state.Equals("Admin"))
	        {
		        var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
		        storesData = storeShoppingService.GetAllStores().ReportList;

	        }
	        else
	        {
		        storesData = userService.GetControlledStoreNames().ReportList;
			}
            
            return View(new StoreListModel(systemId, state, storesData));
        }

	    public IActionResult ManageStoreOptions(int systemId, string state,string message, string store)
	    {
		    return View(new StoreItemModel(systemId,state,message,store));
	    }

	    public IActionResult ManageStore(int systemId, string state, string store, string option)
	    {
		    var userService = MarketServer.Users[systemId];
		    var answer = userService.GetStoreManagerPolicies(store);
			string[] userPolicies = answer.ReportList;
		    if (userPolicies.Contains(option))
		    {
			    return RedirectToAction(option, new { systemId, state ,message = "", store } );
			}
			return RedirectToAction("ManageStoreOptions", new { systemId, state , message = "The user doesn't have the permission to operate this action!",store});
		}

	    public IActionResult ManageProducts(int systemId, string state, string message, string store)
	    {
		    var userService = MarketServer.Users[systemId];
		    var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
		    var answer = storeShoppingService.ViewStoreStock(store);
			return View(new StorePorductListModel(systemId, state, message, store, answer.ReportList));
		}

	    public IActionResult RemoveProduct(int systemId, string state, string store, string product)
	    {
			var userService = MarketServer.Users[systemId];
		    var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
		    var answer = storeManagementService.RemoveProduct(product);
			return RedirectToAction("ManageProducts" , new { systemId, state , message = "", store});

		}

	    public IActionResult EditProduct(int systemId, string state, string store, string product, string whatToEdit, string newValue)
	    {
			var userService = MarketServer.Users[systemId];
		    var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
		    var answer = storeManagementService.EditDiscount(product,whatToEdit,newValue);
		    return RedirectToAction("ManageProducts", new { systemId, state, message = "", store });
		}

	    public IActionResult AddQuanitityToProduct(int systemId, string state, string store, string product, int quantity)

		{
			var userService = MarketServer.Users[systemId];
		    var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
		    var answer = storeManagementService.AddQuanitityToProduct(product,quantity);
		    return RedirectToAction("ManageProducts", new { systemId, state, message = "", store });
		}

	    public IActionResult AddNewProduct(int systemId, string state, string store, string product, double price, string description, int quantity)
	    {
			var userService = MarketServer.Users[systemId];
		    var storeManagementService = MarketYard.Instance.GetStoreManagementService(userService, store);
		    var answer = storeManagementService.AddNewProduct(product, price, description, quantity);
		    return RedirectToAction("ManageProducts", new { systemId, state, message = "", store });
		}
	}
}
