using System;
using System.Linq;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace SadnaSrc.UserSpot
{
	internal class GetStoreManagerPoliciesSlave
	{
		private readonly User _user;
		public UserAnswer Answer { get; private set; }
		private int userID;

		public GetStoreManagerPoliciesSlave(User user)
		{
			Answer = null;
			_user = user;
			userID = user?.SystemID ?? -1;
		}


		public void GetStoreManagerPolicies(string store){

			try
			{
			    MarketLog.Log("UserSpot", "User " + userID + " attempting to view this policies in the store " + store);
                ApproveEnetered();
				MarketLog.Log("UserSpot", "User " + userID + " has successfully viewing his policies in this store...");
				StoreManagerPolicy[] policies = _user.GetStoreManagerPolicies(store);
				string[] stringPolicies = {"Manage Products", "Promote Store Admins", "Manage Store Discounts",
				    "View Purchase History", "View Promotion History", "Manage Store Purchase-Policy"};

                if (!_user.IsSystemAdmin())
				{
					stringPolicies = new string[policies.Length];
					int count = 0;
					foreach (StoreManagerPolicy policy in policies)
					{

						stringPolicies[count] = StoreManagerPolicy.GetStoreActionName(policy.Action);
						count++;

					}
				}
				Answer = new UserAnswer(GetStoreManagerPoliciesStatus.Success, "View of store policies has been successfully granted!",
					stringPolicies);
			}
			catch (UserException e)
			{
				Answer = new UserAnswer((GetStoreManagerPoliciesStatus)e.Status, e.GetErrorMessage());
			}
			catch (DataException e)
			{
			    Answer = new UserAnswer((GetStoreManagerPoliciesStatus)e.Status, e.GetErrorMessage());
			}

        }

		public void ApproveEnetered()
		{
			if (_user != null) { return; }
			throw new UserException(GetStoreManagerPoliciesStatus.DidntEnterSystem,
				"View user's policies in the current store has been requested by User which hasn't fully entered the system yet!");

		}

	}
}

