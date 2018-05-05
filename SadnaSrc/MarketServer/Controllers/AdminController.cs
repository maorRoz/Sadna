﻿using System;
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
            var userService = MarketServer.users[systemId];
            string[] usersData = userService.ViewUsers().ReportList;
            ViewBag.valid = valid;
            return View(new UserListModel(systemId, state, message, usersData));
        }

        public IActionResult ToRemoveUser(int systemId, string state, string toDeleteName)
        {
            var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.users[systemId]);
            var answer = adminService.RemoveUser(toDeleteName);
            return RedirectToAction("RemoveUserView", new
            {
                systemId,
                state,
                message = answer.Answer,
                valid = (answer.Status == Success)
            });
        }

        public IActionResult AdminSelectView(int systemId, string state, string message)
        {
            return View(new UserModel(systemId, state, message));

        }

        public IActionResult AdminViewPurchaseHistory(int systemId, string state, string viewSubject, string viewKind)
        {
            var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.users[systemId]);
            var answer = viewKind == "Store"
                ? adminService.ViewPurchaseHistoryByStore(viewSubject)
                : adminService.ViewPurchaseHistoryByUser(viewSubject);


            if (answer.Status != Success)
            {
                return RedirectToAction("AdminSelectView", new {systemId, state, message = answer.Answer});
            }

            return View(new PurchaseHistoryModel(systemId, state, answer.ReportList));
        }
    }
}
