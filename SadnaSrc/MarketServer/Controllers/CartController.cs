using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MarketServer.Models;
using Microsoft.CodeAnalysis.Editing;


namespace MarketWeb.Controllers
{
    public class CartController : Controller
    {
        private const int Success = 0;
        public IActionResult CartManagement(int systemId, string state,string message)
        {
            var userService = EnterController.GetUserSession(systemId);
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

        public IActionResult IncreaseOrDecreaseCartItem(int systemId, string state, string store,
            string product, int amountToAddOrSub, double unitPrice, string modeButton)
        {
            if (modeButton == "add")
            {
                return RedirectToAction("IncreaseCartItem", new { systemId, state, store,product,amountToAdd = amountToAddOrSub, unitPrice });
            }
            return RedirectToAction("DecreaseCartItem", new { systemId, state, store, product, amountToSub = amountToAddOrSub, unitPrice });
        }

        public IActionResult IncreaseCartItem(int systemId, string state, string store,string product,int amountToAdd, double unitPrice)
        {
            var userService = EnterController.GetUserSession(systemId);
            var answer = userService.EditCartItem(store, product, amountToAdd, unitPrice);
            var message = answer.Status == 0 ? null : answer.Answer;
            return RedirectToAction("CartManagement", new { systemId, state, message });
        }

        public IActionResult DecreaseCartItem(int systemId, string state, string store, string product, int amountToSub, double unitPrice)
        {
            var userService = EnterController.GetUserSession(systemId);
            var answer = userService.EditCartItem(store, product,-amountToSub, unitPrice);
            var message = answer.Status == 0 ? null : answer.Answer;
            return RedirectToAction("CartManagement", new { systemId, state,message });
        }

        public IActionResult RemoveCartItem(int systemId, string state, string store, string product, double unitPrice)
        {
            var userService = EnterController.GetUserSession(systemId);
            var answer = userService.RemoveFromCart(store, product, unitPrice);
            var message = answer.Status == 0 ? null : answer.Answer;
            return RedirectToAction("CartManagement", new { systemId, state, message });
        }
    }
}
