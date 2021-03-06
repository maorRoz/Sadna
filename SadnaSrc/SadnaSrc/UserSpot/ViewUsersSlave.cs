﻿using SadnaSrc.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.MarketData;

namespace SadnaSrc.UserSpot
{
	public class ViewUsersSlave
	{
		private readonly User _user;
		private readonly IUserDL _userDB;

		public UserAnswer Answer { get; private set; }

		private int userID;
		public ViewUsersSlave(User user, IUserDL userDB)
		{
			Answer = null;
			_user = user;
			_userDB = userDB;
			userID = user?.SystemID ?? -1;
		}
		public void ViewUsers()
		{
		    try
		    {
		        MarketLog.Log("UserSpot", "User " + userID + " attempting to view all users...");
		        ApproveEnetered();
		        MarketLog.Log("UserSpot", "User " + userID + " has successfully retrieved all users info...");
		        Answer = new UserAnswer(ViewUsersStatus.Success, "View of users has been granted successfully!",
		            _userDB.UserNamesInSystem());
		    }
		    catch (UserException e)
		    {
		        Answer = new UserAnswer((ViewUsersStatus) e.Status, e.GetErrorMessage());
		    }
		    catch (DataException e)
		    {
		        Answer = new UserAnswer((ViewUsersStatus)e.Status, e.GetErrorMessage());
            }

		}

		private void ApproveEnetered()
		{
			if (_user != null) { return; }
			throw new UserException(ViewUsersStatus.DidntEnterSystem,
				"View users action has been requested by User which hasn't fully entered the system yet!");

		}
	}
}
