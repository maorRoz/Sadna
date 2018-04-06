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
        internal int IntervalStart { get; set; }
        internal int IntervalEnd { get; set; }
        internal string LotteryNumber { get; set; }
        internal string myID { get; set; }
        internal LotteryTicketStatus myStatus { get; set; }
        public LotteryTicket(int _IntervalStart, int _IntervalEnd, string _LotteryNumber, string _myID)
        {
            LotteryNumber = _LotteryNumber;
            myID = _myID;
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
            return "lottery ticket of Lottery number: " + LotteryNumber + " ticket number is: "+myID+" status: "+myStatus;
        }
    }
}
