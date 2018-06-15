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
        public string SystemID { get; }
        public Product Original { get; }
        public double ProductNormalPrice { get; }
        public string storeName { get; }
        public double TotalMoneyPayed { get; set; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public bool IsActive { get; set; }
        private static int globalLotteryID = -1;

        public LotterySaleManagmentTicket(string _storeName, Product _original, DateTime _StartDate, DateTime _EndDate)
        {
            SystemID = GetLottyerID() ;
            Original = _original;
            ProductNormalPrice = _original.BasePrice;
            TotalMoneyPayed = 0;
            StartDate = _StartDate;
            EndDate = _EndDate;
            storeName = _storeName;
            IsActive = true;
        }
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

        public bool CanPurchase(double moneyPayed)
        {
            return (TotalMoneyPayed + moneyPayed <= ProductNormalPrice);
        }
        public bool CheckDatesWhenPurches()
        {
            return ((StartDate.Date <= MarketYard.MarketDate) && (EndDate.Date >= MarketYard.MarketDate));
        }
        public static bool CheckDates(DateTime startDate, DateTime endDate)
        {
            return ((startDate > MarketYard.MarketDate) && (endDate > MarketYard.MarketDate) && (endDate > startDate));
        }
        private void PurchaseALotteryTicket(double moneyPayed, int userID)
        {
            StoreDL handler = StoreDL.Instance;
            LotteryTicket lottery = new LotteryTicket(SystemID, (int)TotalMoneyPayed,
               (int)(TotalMoneyPayed + moneyPayed), moneyPayed, userID);
            handler.AddLotteryTicket(lottery);
            TotalMoneyPayed += moneyPayed;
            handler.EditLotteryInDatabase(this);
        }
        public LotteryTicket Dolottery()
        {
            return TotalMoneyPayed == ProductNormalPrice ? InformAllWinner(RandomLotteryNumber()) : null;
        }
        public LotteryTicket Dolottery(int numberForTests)
        {
            return TotalMoneyPayed == ProductNormalPrice ? InformAllWinner(numberForTests) : null;
        }
        private int RandomLotteryNumber()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            int winningNumber = r.Next(0, (int)ProductNormalPrice);
            return winningNumber;
        }
        private LotteryTicket InformAllWinner(int winningNumber)
        {
            LotteryTicket winner = null;
            StoreDL handler = StoreDL.Instance;
            LinkedList<LotteryTicket> tickets = handler.GetAllTickets(SystemID);
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
                handler.EditLotteryTicketInDatabase(lotter);
            }
            return winner;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType() == GetType() && Equals((LotterySaleManagmentTicket)obj);
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
        internal void InformCancel(IOrderSyncher syncher)
        {
            StoreDL handler = StoreDL.Instance;
            IsActive = false;
            handler.EditLotteryInDatabase(this);
            syncher.CancelLottery(SystemID);
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
        internal bool updateLottery(double moneyPayed, int userID)
        {
            PurchaseALotteryTicket(moneyPayed, userID);
            return TotalMoneyPayed == ProductNormalPrice;
        }

        internal int getWinnerID(int cheatCode)
        {
            int winnerResult = RandomLotteryNumber();
            if (cheatCode != -1)
            {
                winnerResult = cheatCode;
            }

	        if (InformAllWinner(winnerResult) != null)
	        {
		        return InformAllWinner(winnerResult).UserID;
			}

	        return -1;
        }
        public object[] GetLotteryManagmentValuesArray()
        {
            return new object[]
            {
                SystemID,
                Original.SystemId,
                ProductNormalPrice,
                TotalMoneyPayed,
                storeName,
                StartDate,
                EndDate,
                IsActive.ToString()
            };
        }
        private static string GetLottyerID()
        {
            if (globalLotteryID == -1)
            {
                globalLotteryID = StockSyncher.GetMaxEntityID(StoreDL.Instance.GetAllLotteryManagmentIDs());
            }
            globalLotteryID++;
            return "L" + globalLotteryID;
        }

        public static void RestartLotteryID()
        {
            globalLotteryID = -1;
        }
    }
}