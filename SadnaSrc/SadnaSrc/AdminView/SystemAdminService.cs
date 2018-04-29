﻿using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.AdminView
{
    public class SystemAdminService : ISystemAdminService
    {
        private readonly IUserAdmin _admin;
        private readonly IAdminDL adminDB;
        public SystemAdminService(IUserAdmin admin)
        {
            _admin = admin;
            adminDB = AdminDL.Instance;
        }

        public MarketAnswer RemoveUser(string userName)
        {
            RemoveUserSlave slave = new RemoveUserSlave(adminDB,_admin);
            slave.RemoveUser(userName);
            return slave.Answer;
        }
        public MarketAnswer ViewPurchaseHistoryByUser(string userName)
        {
            AdminViewPurchaseHistorySlave slave = new AdminViewPurchaseHistorySlave(adminDB,_admin);
            slave.ViewPurchaseHistoryByUser(userName);
            return slave.Answer;
        }

        public MarketAnswer ViewPurchaseHistoryByStore(string storeName)
        {
            AdminViewPurchaseHistorySlave slave = new AdminViewPurchaseHistorySlave(adminDB, _admin);
            slave.ViewPurchaseHistoryByStore(storeName);
            return slave.Answer;
        }

		
	}
}
