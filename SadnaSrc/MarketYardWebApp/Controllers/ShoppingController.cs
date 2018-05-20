using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketYardWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using SadnaSrc.Main;

namespace MarketYardWebApp.Controllers
{
    public class ShoppingController : Controller
    {
		public IActionResult BrowseMarket(int systemId, string state)
		{
			var userService = MarketServer.Users[systemId];
			string[] usersData = userService.GetAllStores().ReportList;
			return View(new StoreListModel(systemId, state, usersData));
		}

        public IActionResult ViewStoreStock(int systemId, string state, string store, bool valid, string message)
        {
            var userService = MarketServer.Users[systemId];
            var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
            var answer = storeShoppingService.ViewStoreStock(store);
            ViewBag.valid = valid;
            if (answer.Status == 0)
            {
                return View(new StorePorductListModel(systemId,state, message, store,answer.ReportList));
            }
            return RedirectToAction("BrowseMarket", new { systemId, state, answer.Answer });
        }

        public IActionResult AddToCart(int systemId, string state, string store, string product, int quantity)
        {
            var userService = MarketServer.Users[systemId];
            var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
            var answer = storeShoppingService.AddProductToCart(store,product,quantity);
            return RedirectToAction("ViewStoreStock", answer.Status == 0 ? 
                new { systemId, state,store,valid = true, message = answer.Answer } :
                new { systemId, state, store, valid = false, message = answer.Answer });
        }

        public IActionResult AddTicket(int systemId, string state, string store, string product, double price)
        {
            //TODO: implement this
            return null;
        }

        public IActionResult ViewStoreInfo(int systemId, string state, string store)
        {
            var userService = MarketServer.Users[systemId];
            var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
            var answer = storeShoppingService.ViewStoreInfo(store);
	        string storeInfo = "Name : " + answer.ReportList[0] + " Address : " + answer.ReportList[1];

			if (answer.Status == 0)
            {

                return View(new StoreDetailsModel(systemId,state, answer.Answer, storeInfo));
            }
            return RedirectToAction("BrowseMarket", new { systemId, state, answer.Answer });
        }

    }
}