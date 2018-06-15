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
using Newtonsoft.Json;

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

		public IActionResult ViewLogs(int systemId, string state)
		{
			var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
			var answer = adminService.ViewLog();
			if (answer.Status == Success)
			{
				return View(new ErrorLogModel(systemId, state, answer.ReportList));
			}

			return RedirectToAction("MainLobby", "Home",
				new {systemId, state, message = answer.Answer});
		}

		public IActionResult ViewErrors(int systemId, string state)
		{
			var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
			var answer = adminService.ViewError();
			if (answer.Status == Success)
			{
				return View(new ErrorLogModel(systemId, state, answer.ReportList));
			}

			return RedirectToAction("MainLobby", "Home",
				new {systemId, state, message = answer.Answer});
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

		public IActionResult AddCategory(int systemId, string state, string category)
		{
			var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
			var answer = adminService.AddCategory(category);
			if (answer.Status == Success)
			{
				return RedirectToAction("AddingCategoryPage", new {systemId, state, message = answer.Answer, valid = true});
			}

			return RedirectToAction("AddingCategoryPage", new {systemId, state, message = answer.Answer, valid = false});

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
				return RedirectToAction("RemovingCategoryPage", new {systemId, state, message = answer.Answer, valid = true});
			}

			return RedirectToAction("RemovingCategoryPage", new {systemId, state, message = answer.Answer, valid = false});

		}

        public IActionResult AddPurchasePolicy(int systemId, string state, string message, bool valid)
		{
			var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
			var operators = new string[0];
			var conditions = new string[0];
			operators = new[] {"AND", "OR", "NOT"};
			ViewBag.valid = valid;
			var answer = adminService.ViewPoliciesSessions();
			if (answer.Status == Success)
		    {
		       
		        conditions = answer.ReportList;
		    }
		    else
		    {
		        message = answer.Answer;
		    }

		    return View(new MarketPurchasePolicyModel(systemId, state, message,operators, conditions));
		}

        public IActionResult PurchasePolicyPage(int systemId, string state, string message, bool valid)
        {
            var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));

            ViewBag.valid = valid;
            var answer = adminService.ViewPolicies();
            var operators = new string[0];
            var conditions = new string[0];
            if (answer.Status == Success)
            {       
                conditions = answer.ReportList;
            }
            else
            {
                message = answer.Answer;
            }

            return View(new MarketPurchasePolicyModel(systemId, state, message, operators, conditions));
        }

        public IActionResult CreatePolicy(int systemId, string state, string type, string subject, string op, string arg1, string optArg, string usernameText, string addressText, string quantityOp,string quantityText, string priceOp, string priceText, string subject1, string type1)
	    {
		    var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
		    if (usernameText != null)
		    {
			    var answer = adminService.CreatePolicy(type, subject, "Username =", usernameText, optArg);
			    if (answer.Status != Success)				   
					return RedirectToAction("AddPurchasePolicy", new { systemId, state, message = answer.Answer});			
		    }
		    else if (addressText != null)
		    {
				var answer = adminService.CreatePolicy(type, subject, "Address =", addressText, optArg);
			    if (answer.Status != Success)
				    return RedirectToAction("AddPurchasePolicy", new { systemId, state, message = answer.Answer });
			}
		    else if (quantityText != null)
		    {
			    var answer = adminService.CreatePolicy(type, subject, "Quantity "+quantityOp, quantityText, optArg);
		        if (answer.Status != Success)
		            return RedirectToAction("AddPurchasePolicy", new {systemId, state, message = answer.Answer});
		    }
		    else if (priceText != null)
		    {
			    var answer = adminService.CreatePolicy(type, subject, "Price " + priceOp, priceText, optArg);
			    if (answer.Status != Success)
				    return RedirectToAction("AddPurchasePolicy", new { systemId, state, message = answer.Answer });
			}
		    else
		    {
                if(arg1 == null)
                    return RedirectToAction("AddPurchasePolicy", new { systemId, state});
                string[] id1 = arg1.Split('|');
			    string[] id2 = null;
				if (optArg != null)
			    {
				    id2 = optArg.Split('|');
				    var answer = adminService.CreatePolicy(type1, subject1, op, id1[0], id2[0]);
				    if (answer.Status != Success)
					    return RedirectToAction("AddPurchasePolicy", new { systemId, state, message = answer.Answer });
				}
				else
				{
					var answer = adminService.CreatePolicy(type1, subject1, op, id1[0], null);
					if (answer.Status != Success)
						return RedirectToAction("AddPurchasePolicy", new { systemId, state, message = answer.Answer });
				}
			}
			return RedirectToAction("AddPurchasePolicy", new {systemId, state});
		}

	    public IActionResult SavePolicy(int systemId, string state)
	    {
		    var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
		    var answer = adminService.SavePolicy();
		    return RedirectToAction("PurchasePolicyPage", new { systemId, state, message = answer.Answer, valid = answer.Status == Success });
		}

        public IActionResult RemovePolicy(int systemId, string state, string type, string subject)
        {
            var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
            var answer = adminService.RemovePolicy(type, subject);
            return RedirectToAction("PurchasePolicyPage", new { systemId, state, message = answer.Answer, valid = answer.Status == Success });
        }

		public IActionResult ChartsView(int systemId, string state)
		{
			var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
			var answer = adminService.GetEntranceDetails();
			string message = null;
			if (answer.Status != Success)
			{
				message = answer.Answer;
			}


			return View(new UserModel(systemId, state, message));
		}

		public ContentResult JSON(int systemId)
		{
			List<DataPoint> dataPoints = new List<DataPoint>();
			var adminService = MarketYard.Instance.GetSystemAdminService(MarketServer.GetUserSession(systemId));
			var answer = adminService.GetEntranceDetails();
			if (answer.Status != Success)
			{
				return null;
			}

			for (int i = 0; i < answer.ReportList.Length; i++)
			{
				var dataParam = answer.ReportList[i].Split(new[] {"Number: ", " Date: "}, StringSplitOptions.RemoveEmptyEntries);
				string number = dataParam[0];
				int num = Int32.Parse(number);
				string date = dataParam[1];
				DateTime time = DateTime.Parse(date);
				dataPoints.Add(new DataPoint(time.ToString("dd/MM/yyyy"), num));
			}

			return Content(JsonConvert.SerializeObject(dataPoints, _jsonSetting), "application/json");
		}

		JsonSerializerSettings _jsonSetting = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };


	}
}
