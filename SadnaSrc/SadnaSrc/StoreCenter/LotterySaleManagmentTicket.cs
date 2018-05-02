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
        public double ProductNormalPrice { get; }
        public string storeName { get; set; }
        public double TotalMoneyPayed { get; set; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public bool IsActive { get; set; }
        private static int globalLotteryID = FindMaxLotteryId();

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
            StoreDL handler = StoreDL.GetInstance();
            LotteryTicket lottery = new LotteryTicket(SystemID, (int)TotalMoneyPayed,
               (int)(TotalMoneyPayed + moneyPayed), moneyPayed, userID);
            handler.AddLotteryTicket(lottery);
            TotalMoneyPayed += moneyPayed;
            handler.EditLotteryInDatabase(this);
            return lottery;
        }
        public LotteryTicket Dolottery()
        {
            if (TotalMoneyPayed == ProductNormalPrice)
            {
                return InformAllWinner(Random());
            }
            return null;
        }
        public LotteryTicket Dolottery(int numberForTests)
        {
            if (TotalMoneyPayed == ProductNormalPrice)
            {
                return InformAllWinner(numberForTests);
            }
            return null;
        }
        private int Random()
        {

            Random r = new Random(DateTime.Now.Millisecond);
            int winningNumber = r.Next(0, (int)ProductNormalPrice);
            return winningNumber;
        }
        private LotteryTicket InformAllWinner(int winningNumber)
        {
            LotteryTicket winner = null;
            StoreDL handler = StoreDL.GetInstance();
            LinkedList<LotteryTicket> tickets = handler.getAllTickets(SystemID);
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
        internal void InformCancel(IOrderSyncher syncher)
        {
            StoreDL handler = StoreDL.GetInstance();
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
            LotteryTicket lotteryTicket = PurchaseALotteryTicket(moneyPayed, userID);
            if (TotalMoneyPayed == ProductNormalPrice)
                return true;
            return false;
        }

        internal int getWinnerID(int cheatCode)
        {
            int winnerResult = Random();
            if (cheatCode != -1)
            {
                winnerResult = cheatCode;
            }
            return InformAllWinner(winnerResult).UserID;
        }
        public string[] GetLotteryManagmentStringValues()
        {
            string isActive = "";
            if (IsActive)
            {
                isActive = "true";
            }
            else
            {
                isActive = "false";
            }

            return new[]
            {
                "'" + SystemID + "'",
                "'" + Original.SystemId + "'",
                "'" + ProductNormalPrice + "'",
                "'" + TotalMoneyPayed + "'",
                "'" + storeName + "'",
                "'" + StartDate + "'",
                "'" + EndDate + "'",
                "'" + isActive + "'"
            };
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
                IsActive
            };
        }
        private static string GetLottyerID()
        {
            globalLotteryID++;
            return "L" + globalLotteryID;
        }
        private static int FindMaxLotteryId()
        {
            StoreDL DL = StoreDL.GetInstance();
            LinkedList<string> list = DL.getAllLotteryManagmentIDs();
            int max = -5;
            int temp = 0;
            foreach (string s in list)
            {
                temp = Int32.Parse(s.Substring(1));
                if (temp > max)
                {
                    max = temp;
                }
            }
            return max;
        }
    }
}