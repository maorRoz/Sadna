using System;
using System.Collections.Generic;
using System.Data;
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
        public static DateTime MarketDate { get; private set; }
        private MarketYard()
        {
            MarketDate = new DateTime(2018, 4, 14);
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

        public static void SetDateTime(DateTime marketDate)
        {
            if (_instance == null) {return;}
            MarketDate = marketDate;
            var refundOrdersService = new StoreOrderTools();
            refundOrdersService.RefundAllExpiredLotteries();
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
            return new StoreManagementService(new UserSellerHarmony(ref userService,store),store);
        }

        public IStoreShoppingService GetStoreShoppingService(ref IUserService userService)
        {
            return new StoreShoppingService(new UserShopperHarmony(ref userService));
        }

        public IOrderService GetOrderService(ref IUserService userService)
        {
            return new OrderService(new UserBuyerHarmony(ref userService), new StoresSyncherHarmony());
        }

        public IPaymentService GetPaymentService()
        {
            return PaymentService.Instance;
        }

        public ISupplyService GetSupplyService()
        {
            return SupplyService.Instance;
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
