using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MarketServer.Models;

namespace MarketWeb.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult MainLobby(int systemId, string state)
        {
            return View(new UserModel(systemId, state));
        }

        public IActionResult BrowseMarket(int systemId,string state)
        {
            return View(new UserModel(systemId,state));
        }

        public IActionResult SignUp(int systemId, string state)
        {
             return View(new UserModel(systemId, state));
        }

        public IActionResult SignIn(int systemId, string state)
        { 

            return View(new UserModel(systemId, state));
        }


        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
