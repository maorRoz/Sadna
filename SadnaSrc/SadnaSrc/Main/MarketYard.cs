using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.AdminView;
using SadnaSrc.MarketHarmony;
using SadnaSrc.OrderPool;
using SadnaSrc.StoreCenter;
using SadnaSrc.SupplyPoint;
using SadnaSrc.UserSpot;
using SadnaSrc.Walleter;

namespace SadnaSrc.Main
{
    public class MarketYard
    {
        private static MarketYard _instance;

        public static MarketYard Instance => _instance ?? (_instance = new MarketYard());

        private static SQLiteConnection _dbConnection;
        private MarketYard()
        {
            InitiateDb();
        }

        private void InitiateDb()
        {
            var programPath = AppDomain.CurrentDomain.BaseDirectory.Replace("\\bin\\Debug\\", "");
            programPath = programPath.Replace("\\bin\\Debug", "");
            string[] programPathParts = programPath.Split('\\');
            programPathParts[programPathParts.Length - 1] = "SadnaSrc\\";
            programPath = string.Join("\\", programPathParts);
            var dbPath = "URI=file:" + programPath + "MarketYardDB.db";

            _dbConnection = new SQLiteConnection(dbPath);
            _dbConnection.Open();

            var makeFK = new SQLiteCommand("PRAGMA foreign_keys = ON",_dbConnection);
            makeFK.ExecuteNonQuery();
            SystemDL.InsertDbConnector(_dbConnection);
            MarketException.InsertDbConnector(_dbConnection);
            MarketLog.InsertDbConnector(_dbConnection);

        }


        public IUserService GetUserService()
        {
            return new UserService();
        }

        public ISystemAdminService GetSystemAdminService(IUserService userService)
        {
            return new SystemAdminService(new UserAdminHarmony(userService));
        }

        public IStoreManagementService GetStoreManagementService(IUserService userService,string store)
        {
            return new StoreService(new UserSellerHarmony(ref userService,store),store);
        }

        public IStoreShoppingService GetStoreShoppingService(ref IUserService userService)
        {
            return new StoreShoppingService(new UserShopperHarmony(ref userService));
        }

        public IOrderService GetOrderService(ref IUserService userService)
        {
            return new OrderService(new UserBuyerHarmony(ref userService), new StoresSyncherHarmony(),new PaymentService(),
                                                         new SupplyService());
        }

        public IPaymentService GetPaymentService()
        {
            return new PaymentService();
        }

        public ISupplyService GetSupplyService()
        {
            return new SupplyService();
        }

        public static void CleanSession()
        {
            if (_instance == null)
            {
                return;
            }
            MarketLog.RemoveLogs();
            MarketException.RemoveErrors();
            Exit();
            _instance = null;
        }
        public static void Exit()
        {
            _dbConnection.Close();
        }
    }
}
