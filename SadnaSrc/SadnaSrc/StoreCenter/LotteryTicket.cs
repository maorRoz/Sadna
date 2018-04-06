using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.StoreCenter
{
    public class LotteryTicket
    {
        int IntervalStart;
        int IntervalEnd;
        string LotteryNumber;
        LotteryTicketStatus myStatus;
        public LotteryTicket(int _IntervalStart, int _IntervalEnd, string LotteryNumber)
        {
            IntervalStart = _IntervalStart;
            IntervalEnd = _IntervalEnd;
            myStatus = LotteryTicketStatus.WAITING;
        }

        internal bool isWinning(int winningNumber)
        {
            return ((winningNumber <= IntervalEnd) && (winningNumber > IntervalStart));
        }

        internal void RunWinning()
        {
            myStatus = LotteryTicketStatus.WINNING;
        }

        internal void RunLosing()
        {
            myStatus = LotteryTicketStatus.LOSING;
        }

        internal void runCancel()
        {
            myStatus = LotteryTicketStatus.CANCEL;
        }
        public string toString()
        {
            return "lottery ticket of Lottery number: " + LotteryNumber;
        }
    }
}
