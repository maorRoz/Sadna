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
		public IActionResult BrowseMarket(int systemId, string state)
		{
			var userService = MarketServer.users[systemId];
			var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
			string[] usersData = storeShoppingService.GetAllStores().ReportList;
			return View(new StoreListModel(systemId, state, usersData));
		}

        public IActionResult ViewStoreStock(int systemId, string state, string store)
        {
            var userService = MarketServer.users[systemId];
            var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
            var answer = storeShoppingService.ViewStoreStock(store);
            if (answer.Status == 0)
            {
                return View(new StorePorductListModel(systemId,state,null,store,answer.ReportList));
            }
            return RedirectToAction("BrowseMarket", new { systemId, state, answer.Answer });
        }
	}
}