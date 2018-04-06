using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    public class LotterySaleManagmentTicket
    {
        string SystemID;
        internal Product original { get; }
        int ProductNormalPrice;
        int TotalMoneyPayed;
        DateTime StartDate;
        DateTime EndDate;
        LinkedList<LotteryTicket> tickets;
        internal bool isActive { get; set; }

        public LotterySaleManagmentTicket(string _SystemID, Product _original, DateTime _StartDate, DateTime _EndDate)
        {
            SystemID = _SystemID;
            original = _original;
            ProductNormalPrice = _original.BasePrice;
            TotalMoneyPayed = 0;
            StartDate = _StartDate;
            EndDate = _EndDate;
            tickets = new LinkedList<LotteryTicket>();
            isActive = true;
        }
        
        /** 
         * will be used by the store
         **/

        public bool CanPurchase(int moneyPayed) {
            return (TotalMoneyPayed + moneyPayed < ProductNormalPrice);
        }
        public static bool checkDates(DateTime startDate, DateTime endDate)
        {
            return ((startDate > DateTime.Now.Date) && (endDate > DateTime.Now.Date) && (endDate > startDate));
        }
        public LotteryTicket PurchaseALotteryTicket(int moneyPayed)
        {
            if (CanPurchase(moneyPayed))
            {
                ModuleGlobalHandler handler = ModuleGlobalHandler.getInstance();
                LotteryTicket lottery = new LotteryTicket(TotalMoneyPayed, TotalMoneyPayed+ moneyPayed, SystemID, handler.getLotteryTicketID());
                handler.dataLayer.addLotteryTicket(lottery);
                TotalMoneyPayed += moneyPayed;
                tickets.AddLast(lottery);
                return lottery;
            }
                return null;
        }
        public LotteryTicket Dolottery()
        {
            if (TotalMoneyPayed==ProductNormalPrice)
            {
                return informAllWinner();
            }
            else
            {
                informCancel();
                return null;
            }
        }
        private LotteryTicket informAllWinner()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            int winningNumber = r.Next(0, ProductNormalPrice);
            LotteryTicket winner = null;
            foreach (LotteryTicket lotter in tickets)
            {
                if (lotter.isWinning(winningNumber))
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
        internal void informCancel()
        {
            isActive = false;
        //call maor method here
        }
    }
}
