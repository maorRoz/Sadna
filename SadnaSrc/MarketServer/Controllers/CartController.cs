using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MarketServer.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MarketWeb.Controllers
{
    public class CartController : Controller
    {
        public IActionResult CartManagement(int systemId, string state)
        {
            var userService = MarketServer.users[systemId];
            return View(new CartModel(systemId, state, userService.ViewCart().ReportList));
        }

        public IActionResult RemoveCartItem(int systemId, string state, string store, string product, double unitPrice)
        {
            var userService = MarketServer.users[systemId];
            var answer = userService.RemoveFromCart(store, product, unitPrice);
            return RedirectToAction("CartManagement", new { systemId = systemId, state = state });
        }
    }
}
