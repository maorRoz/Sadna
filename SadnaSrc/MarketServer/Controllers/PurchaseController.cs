using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SadnaSrc.Main;


namespace MarketWeb.Controllers
{
    public class PurchaseController : Controller
    {
        private const int Success = 0;
        private IOrderService orderService;
        private MarketAnswer answer;
        public IActionResult BuyImmediateForm(int systemId, string state,string message,string store,
            string product,double unitPrice,int quantity)
        {
            return View();
        }

        public IActionResult BuyAllForm(int systemId, string state, string message)
        {
            return View();
        }

        public IActionResult MakeImmediateBuy(int systemId, string state,string store,string product,double unitPrice,int quantity,
            string coupon,string userName,string userAddress,string userCreditCard)
        {
            InitiateOrder(systemId, userName, userAddress, userCreditCard);
            if (answer.Status != Success)
            {
                return RedirectToAction("BuyImmediateForm",
                    new {systemId, state, message = answer.Answer, store, product, unitPrice, quantity});
            }

            answer = orderService.BuyItemFromImmediate(product, store, quantity, unitPrice, coupon);
            return answer.Status == Success ? RedirectToAction("MainLobby", "Home", new {systemId, state, answer.Answer}) :
                RedirectToAction("BuyImmediateForm", new { systemId, state, message = answer.Answer,store,product,unitPrice,quantity });
        }

        public IActionResult MakeBuyAll(int systemId, string state, string[] coupons,
            string userName, string userAddress, string userCreditCard)
        {
            InitiateOrder(systemId, userName, userAddress, userCreditCard);
            if (answer.Status != Success)
            {
                return RedirectToAction("BuyAllForm",
                    new { systemId, state, message = answer.Answer});
            }

            answer = orderService.BuyEverythingFromCart(coupons);
            return answer.Status == Success ? RedirectToAction("MainLobby", "Home", new { systemId, state, answer.Answer }) :
                RedirectToAction("BuyAllForm", new { systemId, state, message = answer.Answer});
        }

        private void InitiateOrder(int systemId, string userName, string userAddress, string userCreditCard)
        {
            var userService = MarketServer.Users[systemId];
            orderService = MarketYard.Instance.GetOrderService(ref userService);
            answer = orderService.GiveDetails(userName, userAddress, userCreditCard);
        }
    }
}
