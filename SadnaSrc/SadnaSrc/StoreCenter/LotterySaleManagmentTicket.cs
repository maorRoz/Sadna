using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    public class LotterySaleManagmentTicket
    {
        public string SystemID { get; set; }
        public Product Original { get; }
        public int ProductNormalPrice { get;  }
        public  double TotalMoneyPayed { get; set; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public bool IsActive { get; set; }

        public LotterySaleManagmentTicket(string _SystemID, Product _original, DateTime _StartDate, DateTime _EndDate)
        {
            SystemID = _SystemID;
            Original = _original;
            ProductNormalPrice = _original.BasePrice;
            TotalMoneyPayed = 0;
            StartDate = _StartDate;
            EndDate = _EndDate;
            IsActive = true;
        }
        
        /** 
         * will be used by the store
         **/

        public bool CanPurchase(double moneyPayed) {
            return (TotalMoneyPayed + moneyPayed < ProductNormalPrice);
        }
        public static bool CheckDates(DateTime startDate, DateTime endDate)
        {
            return ((startDate > DateTime.Now.Date) && (endDate > DateTime.Now.Date) && (endDate > startDate));
        }
        //TODO: fix this
        public LotteryTicket PurchaseALotteryTicket(double moneyPayed, int userID)
        {
            if (CanPurchase(moneyPayed))
            {
                //TODO: this int thing of interval is really bad, you should do this as precentage from item price
                //TODO: and make them double
                ModuleGlobalHandler handler = ModuleGlobalHandler.GetInstance();
                LotteryTicket lottery = new LotteryTicket(handler.GetLotteryTicketID(), SystemID, (int)TotalMoneyPayed,
                   (int) (TotalMoneyPayed + moneyPayed), moneyPayed, userID);
                handler.DataLayer.AddLotteryTicket(lottery);
                TotalMoneyPayed += moneyPayed;
                return lottery;
            }
                return null;
        }
        public LotteryTicket Dolottery()
        {
            if (TotalMoneyPayed == ProductNormalPrice)
            {
                return InformAllWinner();
            }
            InformCancel();
            return null; //TODO : think of better return in this case
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
            IsActive = false;
        //call maor method here
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
    }
}
