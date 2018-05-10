
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

        public MarketAnswer AddCategory(string categoryName)
        {
            AddCategorySlave slave = new AddCategorySlave(adminDB);
            Category category = slave.AddCategory(categoryName);
            return slave.Answer;
        }
        public MarketAnswer RemoveCategory(string categoryName)
        {
            RemoveCategorySlave slave = new RemoveCategorySlave(adminDB);
            slave.RemoveCategory(categoryName);
            return slave.Answer;
        }

        public MarketAnswer CreatePolicy(string type, string subject, string op, string arg1, string optArg)
        {
            AddPolicySlave slave = new AddPolicySlave(_admin, MarketYard.Instance.GetGlobalPolicyManager());
            slave.CreatePolicy(type,subject,op,arg1,optArg);
            return slave.Answer;
        }

        public MarketAnswer SavePolicy()
        {
            AddPolicySlave slave = new AddPolicySlave(_admin, MarketYard.Instance.GetGlobalPolicyManager());
            slave.SaveFullPolicy();
            return slave.Answer;
        }

        public MarketAnswer RemovePolicy(string type, string subject)
        {
            RemovePolicySlave slave = new RemovePolicySlave(_admin, MarketYard.Instance.GetGlobalPolicyManager());
            slave.RemovePolicy(type,subject);
            return slave.Answer;
        }

        public MarketAnswer ViewPolicies()
        {
            ViewPoliciesSlave slave = new ViewPoliciesSlave(_admin, MarketYard.Instance.GetGlobalPolicyManager());
            slave.ViewPolicies();
            return slave.Answer;
        }

    }
}
