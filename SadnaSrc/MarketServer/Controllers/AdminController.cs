using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MarketServer.Models;
using MarketWeb.Models;
using SadnaSrc.Main;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MarketWeb.Controllers
{
    public class AdminController : Controller
    {
        private const int Success = 0;

        public IActionResult RemoveUserView(int systemId, string state, string message, bool valid)
        {
            var userService = MarketServer.GetUserSession(systemId);
            var usersData = new string[0];
            var answer = userService.ViewUsers();
            if (answer.Status == Success)
            {
                usersData = answer.ReportList;
                ViewBag.valid = valid;
            }
            else
            {
                message = answer.Answer;
                ViewBag.valid = false;
            }
 
            return View(new UserListModel(systemId, state, message, usersData));
        }

        public IActionResult ToRemoveUser(int systemId, string state, string toDeleteName)
        {
            var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
            var answer = adminService.RemoveUser(toDeleteName);
            return RedirectToAction("RemoveUserView", new
            {
                systemId,
                state,
                message = answer.Answer,
                valid = answer.Status == Success
            });
        }

        public IActionResult AdminSelectView(int systemId, string state, string message)
        {
            return View(new UserModel(systemId, state, message));

        }

        public IActionResult AdminViewPurchaseHistory(int systemId, string state, string viewSubject, string viewKind)
        {
            var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
            var answer = viewKind == "Store"
                ? adminService.ViewPurchaseHistoryByStore(viewSubject)
                : adminService.ViewPurchaseHistoryByUser(viewSubject);


            if (answer.Status != Success)
            {
                return RedirectToAction("AdminSelectView", new {systemId, state, message = answer.Answer});
            }

            return View(new PurchaseHistoryModel(systemId, state, answer.ReportList));
        }

	    public IActionResult HandlingCategoryView(int systemId, string state)
	    {
		    return View(new UserModel(systemId, state, null));
	    }

	    public IActionResult AddingCategoryPage(int systemId, string state, string message, bool valid)
	    {
		    ViewBag.valid = valid;
		    return View(new UserModel(systemId, state, message));
	    }

	    public IActionResult AddCategory(int systemId,string state, string category)
	    {
		    var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
		    var answer = adminService.AddCategory(category);
		    if (answer.Status == Success)
		    {
			    return RedirectToAction("AddingCategoryPage", new {systemId, state, message = answer.Answer, valid = true});
		    }
		    return RedirectToAction("AddingCategoryPage", new { systemId, state, message = answer.Answer, valid = false });

		}

	    public IActionResult RemovingCategoryPage(int systemId, string state, string message, bool valid)
	    {
		    ViewBag.valid = valid;
		    return View(new UserModel(systemId, state, message));
	    }

	    public IActionResult RemoveCategory(int systemId, string state, string category)
	    {
		    var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
		    var answer = adminService.RemoveCategory(category);
		    if (answer.Status == Success)
		    {
			    return RedirectToAction("RemovingCategoryPage", new { systemId, state, message = answer.Answer, valid = true });
		    }
		    return RedirectToAction("RemovingCategoryPage", new { systemId, state, message = answer.Answer, valid = false });

	    }

		public IActionResult PurchasePolicy(int systemId, string state, string message)
		{
			var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
		    var operators = new string[0];
            var conditions = new string[0];
		    var answer = adminService.ViewPolicies();
		    if (answer.Status == Success)
		    {
		        operators = new []{"AND", "OR", "NOT"};
		        conditions = answer.ReportList;
		    }
		    else
		    {
		        message = answer.Answer;
		    }

		    return View(new ConditionsOperatorsModel(systemId, state, message,operators,conditions));
		}

	    public IActionResult CreatePolicy(int systemId, string state, string type, string subject, string op, string arg1, string optArg)
	    {
		    var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
		    var answer = adminService.CreatePolicy(type, subject, op, arg1, optArg);
		    if (answer.Status == Success)
		    {
			    return RedirectToAction("PurchasePolicy", new { systemId, state, message = answer.Answer, valid = true });
			}
		    return RedirectToAction("PurchasePolicy", new { systemId, state, message = answer.Answer, valid = false });
		}

	    public IActionResult SavePolicy(int systemId, string state)
	    {
		    var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
		    var answer = adminService.SavePolicy();
		    return RedirectToAction("PurchasePolicy", new { systemId, state, message = answer.Answer, valid = answer.Status == Success });
		}

	}
}
