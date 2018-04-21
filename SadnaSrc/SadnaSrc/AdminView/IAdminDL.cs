﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.AdminView
{
    interface IAdminDL
    {
        string[] GetPurchaseHistory(string field, string givenValue);
        bool IsUserNameExistInHistory(string userName);
        bool IsStoreExistInHistory(string storeName);
        void DeleteUser(string userName);
        string[] FindSolelyOwnedStores();
        void CloseStore(string store);
        bool IsUserExist(string userName);
    }
}