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

	    public IActionResult ManageStoreOptions(int systemId, string state,string message, string store)
	    {
		    return View(new StoreItemModel(systemId,state,message,store));
	    }

	    public IActionResult ManageStore(int systemId, string state, string store, string option)
	    {
		    var userService = MarketServer.users[systemId];
		    var answer = userService.GetStoreManagerPolicies(store);
			string[] userPolicies = answer.ReportList;
		    if (userPolicies.Contains(option))
		    {
			    return RedirectToAction(option, new { systemId, state});
			}
			return RedirectToAction("ManageStoreOptions", new { systemId, state , message = "The user doesn't have the permission to operate this action!",store});
		}
    }
}
