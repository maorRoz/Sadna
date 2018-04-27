using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MarketServer.Models;

namespace MarketServer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult MainLobby(int SystemId, string State)
        {
            return View(new UserModel(SystemId, State));
        }

        public IActionResult BrowseMarket(int SystemId,string State)
        {
            return View(new UserModel(SystemId,State));
        }

        public IActionResult SignUp(int SystemId, string State)
        {
             return View(new UserModel(SystemId, State));
        }

        public IActionResult SignIn(int SystemId, string State)
        { 

            return View(new UserModel(SystemId, State));
        }

        public IActionResult CartManagement(int SystemId, string State)
        {
            var userService = MarketServer.users[SystemId];
            return View(new CartModel(SystemId,State,userService.ViewCart().ReportList));
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
