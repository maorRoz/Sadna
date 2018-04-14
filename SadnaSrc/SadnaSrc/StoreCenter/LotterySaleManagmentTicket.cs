using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    public class LotterySaleManagmentTicket
    {
        public string SystemID { get; set; }
        public Product Original { get; }
        public int ProductNormalPrice { get;  }
        public string storeName { get; set; }
        public  double TotalMoneyPayed { get; set; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public bool IsActive { get; set; }

        public LotterySaleManagmentTicket(string _SystemID, string _storeName, Product _original, DateTime _StartDate, DateTime _EndDate)
        {
            SystemID = _SystemID;
            Original = _original;
            ProductNormalPrice = _original.BasePrice;
            TotalMoneyPayed = 0;
            StartDate = _StartDate;
            EndDate = _EndDate;
            storeName = _storeName;
            IsActive = true;
        }
        
        /** 
         * will be used by the store
         **/

        public bool CanPurchase(double moneyPayed) {
            return (TotalMoneyPayed + moneyPayed <= ProductNormalPrice);
        }
        public bool checkDatesWhenPurches()
        {
            return ((StartDate.Date <= MarketYard.MarketDate) && (EndDate.Date >= MarketYard.MarketDate));
        }
        public static bool CheckDates(DateTime startDate, DateTime endDate)
        {
            return ((startDate > MarketYard.MarketDate) && (endDate > MarketYard.MarketDate) && (endDate > startDate));
        }
        public LotteryTicket PurchaseALotteryTicket(double moneyPayed, int userID)
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            LotteryTicket lottery = new LotteryTicket(handler.GetLotteryTicketID(), SystemID, (int)TotalMoneyPayed,
               (int)(TotalMoneyPayed + moneyPayed), moneyPayed, userID);
            handler.DataLayer.AddLotteryTicket(lottery);
            TotalMoneyPayed += moneyPayed;
            handler.DataLayer.EditLotteryInDatabase(this);
            return lottery;
        }
        public LotteryTicket Dolottery()
        {
            if (TotalMoneyPayed == ProductNormalPrice)
            {
                return InformAllWinner();
            }
            return null;
        }
        private LotteryTicket InformAllWinner()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            int winningNumber = r.Next(0, ProductNormalPrice);
            LotteryTicket winner = null;
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            LinkedList<LotteryTicket> tickets = handler.DataLayer.getAllTickets(this.SystemID);
            foreach (LotteryTicket lotter in tickets)
            {
                if (lotter.IsWinning(winningNumber))
                {
                    winner = lotter;
                    lotter.RunWinning();
                }
                else
                {
                    lotter.RunLosing();
                }
            }
            return winner;
        }
        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
                return false;
            return Equals((LotterySaleManagmentTicket)obj);
        }
        private bool Equals(LotterySaleManagmentTicket obj)
        {
            return (obj.SystemID == SystemID &&
                    obj.ProductNormalPrice == ProductNormalPrice &&
                    obj.Original.SystemId == Original.SystemId &&
                    obj.StartDate.Equals(StartDate) &&
                    obj.EndDate.Equals(EndDate) &&
                    obj.TotalMoneyPayed.Equals(TotalMoneyPayed) &&
                    obj.IsActive == IsActive);
        }
        internal void InformCancel()
        {
            ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
            IsActive = false;
            handler.DataLayer.EditLotteryInDatabase(this);
            OrderSyncherHarmony syncer = new OrderSyncherHarmony();
            syncer.CancelLottery(SystemID);
        }

        public override int GetHashCode()
        {
            var hashCode = 808578065;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SystemID);
            hashCode = hashCode * -1521134295 + EqualityComparer<Product>.Default.GetHashCode(Original);
            hashCode = hashCode * -1521134295 + ProductNormalPrice.GetHashCode();
            hashCode = hashCode * -1521134295 + TotalMoneyPayed.GetHashCode();
            hashCode = hashCode * -1521134295 + StartDate.GetHashCode();
            hashCode = hashCode * -1521134295 + EndDate.GetHashCode();
            hashCode = hashCode * -1521134295 + IsActive.GetHashCode();
            return hashCode;
        }
        internal bool updateLottery(int moneyPayed, int userID)
        {
            LotteryTicket lotteryTicket = PurchaseALotteryTicket(moneyPayed, userID);
            if (TotalMoneyPayed == ProductNormalPrice)
                return true;
            return false;
        }

        internal int getWinnerID()
        {
            return InformAllWinner().UserID;
        }
    }
}
