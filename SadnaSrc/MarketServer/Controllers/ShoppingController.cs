﻿using System;
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

        public IActionResult ViewStoreStock(int systemId, string state, string store, bool valid, string message)
        {
            var userService = MarketServer.users[systemId];
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
            var userService = MarketServer.users[systemId];
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
            var userService = MarketServer.users[systemId];
            var storeShoppingService = MarketYard.Instance.GetStoreShoppingService(ref userService);
            var answer = storeShoppingService.ViewStoreStock(store);
            if (answer.Status == 0)
            {
                //TODO: implement model and page
                return View();
            }
            return RedirectToAction("BrowseMarket", new { systemId, state, answer.Answer });
        }

    }
}