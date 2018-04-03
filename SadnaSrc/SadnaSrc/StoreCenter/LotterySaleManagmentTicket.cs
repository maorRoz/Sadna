using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    class LotterySaleManagmentTicket
    {
        int SystemID;
        Product original;
        int ProductNormalPrice;
        int TotalMoneyPayed;
        DateTime StartDate;
        DateTime EndDate;
        LinkedList<LotteryTicket> tickets;

        public LotterySaleManagmentTicket(int _SystemID, Product _original, DateTime _StartDate, DateTime _EndDate)
        {
            SystemID = _SystemID;
            original = _original;
            ProductNormalPrice = _original.BasePrice;
            TotalMoneyPayed = 0;
            StartDate = _StartDate;
            EndDate = _EndDate;
            tickets = new LinkedList<LotteryTicket>();
        }
        public LotterySaleManagmentTicket(int _SystemID, Product _original, DateTime _EndDate)
        {
            SystemID = _SystemID;
            original = _original;
            ProductNormalPrice = _original.BasePrice;
            TotalMoneyPayed = 0;
            StartDate = DateTime.Now.Date;
            EndDate = _EndDate;
            tickets = new LinkedList<LotteryTicket>();
        }
        /** 
         * will be used by the store
         **/
        public bool CanPurches(int moneyPaied) {
            return (TotalMoneyPayed + moneyPaied < ProductNormalPrice);
        }
        public LotteryTicket PurchesALotteryTicket(int moneyPaied)
        {
            if (TotalMoneyPayed+ moneyPaied<ProductNormalPrice)
            {
                LotteryTicket lottery = new LotteryTicket(this, TotalMoneyPayed, TotalMoneyPayed+moneyPaied);
                TotalMoneyPayed += moneyPaied;
                tickets.AddLast(lottery);
                return lottery;
            }
            else
            {
                return null;
            }
        }
        public void TimeUp()
        {
            if (TotalMoneyPayed==ProductNormalPrice)
            {
                informAllWinner();
            }
            else
            {
                informAllCancel();
            }
        }
        private void informAllWinner()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            int winningNumber = r.Next(0, ProductNormalPrice);
            foreach (LotteryTicket lotter in tickets)
            {
                if (lotter.isWinning(winningNumber))
                {
                    lotter.RunWinning();
                }
                else
                {
                    lotter.RunLosing();
                }
            }
        }
        private void informAllCancel()
        {
            foreach (LotteryTicket lotter in tickets)
            {
                lotter.runCancel();
            }
        }

        
    }
}
