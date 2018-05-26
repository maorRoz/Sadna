using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MarketServer.Models;


namespace MarketWeb.Controllers
{
    public class CartController : Controller
    {
        private const int Success = 0;
        public IActionResult CartManagement(int systemId, string state,string message)
        {
            var userService = MarketServer.GetUserSession(systemId);
            var answer = userService.ViewCart();
            var cartData = new string[0];
            if (answer.Status == Success)
            {
                cartData = userService.ViewCart().ReportList;
            }
            else
            {
                message = answer.Answer;
            }

            return View(new CartModel(systemId, state, message, cartData));
        }

        public IActionResult IncreaseCartItem(int systemId, string state, string store, string product, double unitPrice)
        {
            var userService = MarketServer.GetUserSession(systemId);
            var answer = userService.EditCartItem(store, product, 1, unitPrice);
            var message = answer.Status == 0 ? null : answer.Answer;
            return RedirectToAction("CartManagement", new { systemId, state, message });
        }

        public IActionResult DecreaseCartItem(int systemId, string state, string store, string product, double unitPrice)
        {
            var userService = MarketServer.GetUserSession(systemId);
            var answer = userService.EditCartItem(store, product,-1, unitPrice);
            var message = answer.Status == 0 ? null : answer.Answer;
            return RedirectToAction("CartManagement", new { systemId, state,message });
        }

        public IActionResult RemoveCartItem(int systemId, string state, string store, string product, double unitPrice)
        {
            var userService = MarketServer.GetUserSession(systemId);
            var answer = userService.RemoveFromCart(store, product, unitPrice);
            var message = answer.Status == 0 ? null : answer.Answer;
            return RedirectToAction("CartManagement", new { systemId, state, message });
        }
    }
}
