using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Components.DictionaryAdapter;
using Castle.Core;
using Castle.Core.Internal;
using SadnaSrc.AdminView;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.UserSpot
{
	public class GetEntranceDetailsSlave
	{
		private readonly IAdminDL _adminDB;
		private int adminSystemID;
		private readonly IUserAdmin _admin;

		public AdminAnswer Answer { get; private set; }

		public GetEntranceDetailsSlave(IAdminDL adminDB, IUserAdmin admin)
		{
			_adminDB = adminDB;
			_admin = admin;
			adminSystemID = _admin.GetAdminSystemID();
		}

		public void GetEntranceDetails()
		{
			try
			{
				MarketLog.Log("UserSpot", "User " + adminSystemID + " attempting to view entrance details...");
				_admin.ValidateSystemAdmin();
				MarketLog.Log("UserSpot", "User " + adminSystemID + " has successfully retrieved all entrance information...");
				Answer = new AdminAnswer(GetEntranceDetailsEnum.Success, "View of Entrance report successfully!",
					GetDataAsString(GetDetails()));

			}
			catch (UserException e)
			{
				Answer = new AdminAnswer((GetEntranceDetailsEnum)e.Status, e.GetErrorMessage());
			}
			catch (DataException e)
			{
				Answer = new AdminAnswer((GetEntranceDetailsEnum)e.Status, e.GetErrorMessage());
			}
		}

		private Pair<int, DateTime>[] GetDetails()
		{
			Pair<int, DateTime>[] userDateReport = _adminDB.GetEntranceReport();
			DateTime[] uniqueDates = getUniqueDates(userDateReport);
			List<Pair<int, DateTime>> numEntranceDate = new List<Pair<int, DateTime>>();
			foreach (var date in uniqueDates)
			{
				int count = 0;
				foreach (var userDate in userDateReport)
				{
					if (userDate.Second.Equals(date))
					{
						count++;
					}

				}
				Pair<int, DateTime> p = new Pair<int, DateTime>(count, date);
				numEntranceDate.Add(p);
			}

			return numEntranceDate.ToArray();
		}

		private DateTime[] getUniqueDates(Pair<int, DateTime>[] userDateReport)
		{
			List<DateTime> uniqueDates = new List<DateTime>();
			if (userDateReport.Length != 0)
			{
				uniqueDates.Add(userDateReport[0].Second);
			}

			for (int i = 1; i < userDateReport.Length; i++)
			{
				if (!uniqueDates.Contains(userDateReport[i].Second))
				{
					uniqueDates.Add(userDateReport[i].Second);
				}
			}

			return uniqueDates.ToArray();
		}

		private string[] GetDataAsString(Pair<int, DateTime>[] data)
		{
			string[] res = new string[data.Length];
			for (int i = 0; i < data.Length; i++)
			{
				res[i] = "Number: " + data[i].First + " Date: " + data[i].Second;
			}

			return res;
		}
	}
}