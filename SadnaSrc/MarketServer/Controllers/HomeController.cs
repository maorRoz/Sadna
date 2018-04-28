using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MarketServer.Models;
using SadnaSrc.Main;

namespace MarketWeb.Controllers
{
    public class HomeController : Controller
    {
        private const int Success = 0;
        public IActionResult MainLobby(int systemId, string state, string message)
        {
            return View(new UserModel(systemId, state, message));
        }

        public IActionResult BrowseMarket(int systemId,string state)
        {
            return View(new UserModel(systemId,state));
        }

        public IActionResult SignUp(int systemId, string state,string message)
        {
             return View(new UserModel(systemId, state,message));
        }

        public IActionResult SignIn(int systemId, string state)
        { 

            return View(new UserModel(systemId, state));
        }

        [HttpPost]
        public IActionResult SubmitSignUp(int systemId, string state,string usernameEntry,string addressEntry,
            string passwordEntry,string creditCardEntry)
        {
            var userService = MarketServer.users[systemId];
            var answer = userService.SignUp(usernameEntry, addressEntry, passwordEntry, creditCardEntry);
            if (answer.Status == Success)
            {
                return RedirectToAction("SignUp", new { systemId = systemId, state = state, message = answer.Answer});
            }

            return RedirectToRoute("MainLobby", new {systemId = systemId, state = state, message = answer.Answer});
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
