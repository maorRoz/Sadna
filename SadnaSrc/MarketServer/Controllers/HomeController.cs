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
        public IActionResult BrowseMarket(int SystemId)
        {
            return View(SystemId);
        }

        public IActionResult SignUp(int SystemId)
        {

            return View(SystemId);
        }

        public IActionResult SignIn(int SystemId)
        { 

            return View(SystemId);
        }



        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
