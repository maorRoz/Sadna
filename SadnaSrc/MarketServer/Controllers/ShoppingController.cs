using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketWeb.Models;
using Microsoft.AspNetCore.Mvc;
using SadnaSrc.Main;

namespace MarketWeb.Controllers
{
    public class ShoppingController : Controller
    {
        private const int Success = 0;
		public IActionResult BrowseMarket(int systemId, string state)
		{
			var userService = MarketServer.GetUserSession(systemId);
		    var usersData = new string[0];
		    string message = null;
            var answer = userService.GetAllStores();
		    if (answer.Status == Success)
		    {
		        usersData = answer.ReportList;
		    }
		    else
		    {
		        message = answer.Answer;
		    }

		    return View(new StoreListModel(systemId, state, usersData,message));
		}

        public IActionResult ViewStoreStock(int systemId, string state, string store, bool valid, string message)
        {
            var userService = MarketServer.GetUserSession(systemId);
            var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
            var answer = storeShoppingService.ViewStoreStock(store);
            ViewBag.valid = valid;
            if (answer.Status == 0)
            {
                return View(new StorePorductListModel(systemId,state, message, store,answer.ReportList));
            }
            return RedirectToAction("BrowseMarket", new { systemId, state, answer.Answer });
        }

        public IActionResult AddToCart(int systemId, string state, string store, string product, int quantity, int directViewStoreStock)
        {
            var userService = MarketServer.GetUserSession(systemId);
            var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
            var answer = storeShoppingService.AddProductToCart(store,product,quantity);
	        if (directViewStoreStock==0)
	        {
		        return RedirectToAction("ViewStoreStock", answer.Status == 0 ?
			        new { systemId, state, store, valid = true, message = answer.Answer } :
			        new { systemId, state, store, valid = false, message = answer.Answer });
			}

	        else
	        {
		        return RedirectToAction("SearchProductView", new {systemId, state});
	        }
            
        }

        public IActionResult ViewStoreInfo(int systemId, string state, string store)
        {
            var userService = MarketServer.GetUserSession(systemId);
            var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
            var answer = storeShoppingService.ViewStoreInfo(store);

            if (answer.Status != 0)
            {
                return RedirectToAction("BrowseMarket", new {systemId, state, answer.Answer});
            }
            var storeInfo = "Name : " + answer.ReportList[0] + " Address : " + answer.ReportList[1];
            return View(new StoreDetailsModel(systemId,state, answer.Answer, storeInfo));
        }

	    public IActionResult SearchProductView(int systemId, string state, string message)
	    {
		    var userService = MarketServer.Users[systemId];
		    var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
		    string[] categories = storeShoppingService.GetAllCategoryNames().ReportList;
		    return View(new CategoryListModel(systemId, state, message, categories));
	    }

	    public IActionResult SearchProduct(int systemId, string state, string type, string value, double minPrice, double maxPrice, string category)
	    {
		    var userService = MarketServer.Users[systemId];
		    var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
		    var answer = storeShoppingService.SearchProduct(type, value, minPrice, maxPrice, category);
			return RedirectToAction("ProductsView", new { systemId, state, results=answer.ReportList});
		}

		public IActionResult ProductsView(int systemId, string state,string message, string[] results)
	    {
		    return View(new ProductListModel(systemId,state, message, results));
	    }

	}
}