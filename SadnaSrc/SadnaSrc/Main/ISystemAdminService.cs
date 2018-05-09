﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SadnaSrc.Main
{
    public interface ISystemAdminService
    {
        MarketAnswer RemoveUser(string userName);
        MarketAnswer ViewPurchaseHistoryByUser(string userName);
        MarketAnswer ViewPurchaseHistoryByStore(string storeName);
        MarketAnswer AddCategory(string categoryname);
        MarketAnswer RemoveCategory(string categoryname);
	    MarketAnswer CreatePolicy(string type, string subject, string op, string arg1, string optArg);
	    MarketAnswer ViewPolicies();
		MarketAnswer SavePolicy();
    }

    public enum RemoveUserStatus
    {
        Success,
        SelfTermination,
        NotSystemAdmin,
        NoUserFound
    }

    public enum ViewPurchaseHistoryStatus
    {
        Success,
        NotSystemAdmin,
        NoUserFound,
        NoStoreFound
    }

    public enum EditCategoryStatus
    {
        Success,
        CategoryNotExistsInSystem,
        CategoryAlradyExist,
		InvalidCategory
    }

    public enum EditPolicyStatus
    {
        Success,
        InvalidPolicyData,
        NoAuthority
        
    }

    public enum ViewPolicyStatus
    {
        Success,
        NoAuthority

    }
}
