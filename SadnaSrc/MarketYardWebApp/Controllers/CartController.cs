using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MarketYardWebApp.Models;


namespace MarketYardWebApp.Controllers
{
    public class CartController : Controller
    {
        public IActionResult CartManagement(int systemId, string state,string message)
        {
            var userService = MarketServer.Users[systemId];
            var cartData = userService.ViewCart().ReportList;
            return View(new CartModel(systemId, state, message, cartData));
        }

        public IActionResult IncreaseCartItem(int systemId, string state, string store, string product, double unitPrice)
        {
            var userService = MarketServer.Users[systemId];
            userService.EditCartItem(store, product, 1, unitPrice);
            return RedirectToAction("CartManagement", new { systemId, state });
        }

        public IActionResult DecreaseCartItem(int systemId, string state, string store, string product, double unitPrice)
        {
            var userService = MarketServer.Users[systemId];
            var answer = userService.EditCartItem(store, product,-1, unitPrice);
            var message = answer.Status == 0 ? null : answer.Answer;
            return RedirectToAction("CartManagement", new { systemId, state,message });
        }

        public IActionResult RemoveCartItem(int systemId, string state, string store, string product, double unitPrice)
        {
            var userService = MarketServer.Users[systemId];
            userService.RemoveFromCart(store, product, unitPrice);
            return RedirectToAction("CartManagement", new { systemId, state });
        }
    }
}
