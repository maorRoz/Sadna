using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketData;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.AdminView
{
    public class RemoveUserSlave
    {
        private readonly IAdminDL _adminDB;


        public AdminAnswer Answer { get; private set; }

        private readonly int adminSystemID;
        private readonly IUserAdmin _admin;


        public RemoveUserSlave(IAdminDL adminDB,IUserAdmin admin)
        {
            _adminDB = adminDB;
            Answer = null;
            _admin = admin;
            adminSystemID = _admin.GetAdminSystemID();
        }

        public void RemoveUser(string userName)
        {
            try
            {
                MarketLog.Log("AdminView", "System Admin " + adminSystemID +
                                           " attempting to execute remove user operation on User " + userName + "...");
                _admin.ValidateSystemAdmin();
                ApproveNotSelfTermination(userName);
                ValidateUserExist(userName);
                MarketLog.Log("AdminView", "User " + userName +
                                           " has been found by the system. Removing user's saved cart and profile...");

                _adminDB.DeleteUser(userName);
                MarketLog.Log("AdminView", "System Admin " + adminSystemID +
                                           " successfully removed User " + userName + " from the system!");

                MarketLog.Log("AdminView", "looking for sole ownership of User " + userName + " on stores...");
                RemoveSolelyOwnedStores(userName);

                MarketLog.Log("AdminView", "User " + userName +
                                           " solely owned stores has been deactivated. Operation is " +
                                           "finally completed safely!");
                Answer = new AdminAnswer(RemoveUserStatus.Success, "Remove user has been successful!");
            }
            catch (AdminException e)
            {
                Answer = new AdminAnswer((RemoveUserStatus) e.Status, e.GetErrorMessage());
            }
            catch (DataException e)
            {
                Answer = new AdminAnswer((RemoveUserStatus)e.Status, e.GetErrorMessage());
            }
            catch (MarketException e)
            {
                Answer = new AdminAnswer(RemoveUserStatus.NotSystemAdmin, e.GetErrorMessage());
            }
        }

        private void ApproveNotSelfTermination(string userName)
        {
            if (_admin.GetAdminName() == userName)
            {
                throw new AdminException(RemoveUserStatus.SelfTermination, "remove user action has been requested " +
                                                                           "by System Admin on himself!");
            }
        }

        private void RemoveSolelyOwnedStores(string userName)
        {
            string[] solelyOwnedStores = _adminDB.FindSolelyOwnedStores();
            foreach (string store in solelyOwnedStores)
            {
                MarketLog.Log("AdminView", "User " + userName + " found to be a sole Store Owner of '"
                                           + store + "' store. System Admin " + adminSystemID
                                           + " deactivating therefore store '" + store + "'");
                _adminDB.CloseStore(store);
                MarketLog.Log("AdminView", "System Admin " + adminSystemID +
                                           " deactivated store '" + store + "' successfully!");
            }
        }

        private void ValidateUserExist(string userName)
        {
            if (!_adminDB.IsUserExist(userName))
            {
                throw new AdminException(RemoveUserStatus.NoUserFound, "Couldn't find any User with that Name to remove");
            }
        }
    }
}
