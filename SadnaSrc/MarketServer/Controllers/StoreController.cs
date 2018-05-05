﻿using System;
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
			return View(new StoreListModel(systemId, state, usersData));
		}

		public IActionResult ProductManagement(int systemId, string state, string message, string store)
		{
			var userService = MarketServer.users[systemId];
			var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
			string[] usersData = storeShoppingService.ViewStoreStock(store).ReportList;
			return View(new ProductListModel(systemId, state, message, usersData));
		}
	}
}