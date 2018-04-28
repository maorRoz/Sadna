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

        public IActionResult BrowseMarket(int systemId,string state, string message)
        {
            return View(new UserModel(systemId,state,message));
        }

        public IActionResult SignUp(int systemId, string state,string message)
        {
             return View(new UserModel(systemId, state,message));
        }

        public IActionResult SignIn(int systemId, string state, string message)
        { 

            return View(new UserModel(systemId, state,message));
        }

        public IActionResult SubmitSignUp(int systemId,string usernameEntry,string addressEntry,
            string passwordEntry,string creditCardEntry)
        {
            var userService = MarketServer.users[systemId];
            var answer = userService.SignUp(usernameEntry, addressEntry, passwordEntry, creditCardEntry);
            if (answer.Status == Success)
            {
                return RedirectToAction("MainLobby", new { systemId, state ="Registered", message = answer.Answer });
            }

            return RedirectToAction("SignUp", new {systemId, state ="Guest", message = answer.Answer});
        }

        public IActionResult SubmitSignIn(int systemId, string usernameEntry, string passwordEntry)
        {
            var userService = MarketServer.users[systemId];
            var answer = userService.SignIn(usernameEntry, passwordEntry);
            if (answer.Status == Success)
            {
                MarketServer.users.Remove(systemId);
                systemId = Convert.ToInt32(answer.ReportList[0]);
                if (!MarketServer.users.ContainsKey(systemId))
                {
                    MarketServer.users.Add(Convert.ToInt32(systemId), userService);
                }

                var state = answer.ReportList[1];
                return RedirectToAction("MainLobby", new { systemId, state, message = answer.Answer });
            }

            return RedirectToAction("SignIn", new { systemId, state = "Guest", message = answer.Answer });
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
