using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MarketServer.Models;
using MarketWeb.Models;
using SadnaSrc.Main;


namespace MarketWeb.Controllers
{
    public class AdminController : Controller
    {
        private const int Success = 0;
        public IActionResult RemoveUserView(int systemId,string state, string message, bool valid)
        {
            // get user list in db
            return View(new UserListModel(systemId,state,message));
        }

        public IActionResult ToRemoveUser(int systemId, string state, string toDeleteName)
        {
            var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.users[systemId]);
            var answer = adminService.RemoveUser(toDeleteName);
            return RedirectToAction("RemoveUserView", new { systemId, state, message = answer.Answer,
                valid = (answer.Status == Success) });
        }

        public IActionResult AdminSelectView(int systemId, string state, string message)
        {
            return View(new UserModel(systemId,state,message));
        }

        public IActionResult AdminViewPurchaseHistory(int systemId, string state,string viewSubject,string viewKind)
        {
            var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.users[systemId]);
            MarketAnswer answer;
            if (viewKind == "Store")
            {
                answer = adminService.ViewPurchaseHistoryByStore(viewSubject);
            }
            else
            {
                answer = adminService.ViewPurchaseHistoryByUser(viewSubject);
            }

            if (answer.Status != Success)
            {
                return RedirectToAction("AdminSelectView", new{ systemId, state, message = answer.Answer, });
            }

            return View(new PurchaseHistoryModel(systemId,state,viewSubject));
        }
    }
}
