using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketWeb.Models;
using Microsoft.AspNetCore.Mvc;
using SadnaSrc.Main;

namespace MarketWeb.Controllers
{
    public class StoreController : Controller
    {
		public IActionResult StoreManagement(int systemId, string state, string message)
		{
			var userService = MarketServer.users[systemId];
			var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
			string[] usersData = storeShoppingService.GetAllStores().ReportList;
			return View(new StoreListModel(systemId, state, message, usersData));
		}
	}
}