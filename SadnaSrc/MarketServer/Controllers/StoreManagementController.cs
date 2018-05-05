using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarketWeb.Models;
using Microsoft.AspNetCore.Mvc;
using SadnaSrc.Main;
using static MarketWeb.Models.StoreListModel;

namespace MarketWeb.Controllers
{
    public class StoreManagementController : Controller
    {
        public IActionResult StoreControl(int systemId, string state)
        {
            var userService = MarketServer.users[systemId];
            string[] storesData = userService.GetControlledStoreNames().ReportList;
            return View(new StoreListModel(systemId, state, storesData));
        }

	    public IActionResult ManageStoreOptions(int systemId, string state, string store)
	    {
		    string[] data = {store};
		    return View(new StoreListModel(systemId,state,data));
	    }

	    public IActionResult ManageStore(int systemId, string state, string option)
	    {
		    return RedirectToAction(option, new { systemId, state = "Registered"});
		}
    }
}
