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
            myStatus = LotteryTicketStatus.Waiting;
        }

        internal bool IsWinning(int winningNumber)
        {
            return ((winningNumber <= IntervalEnd) && (winningNumber > IntervalStart));
        }

        internal void RunWinning()
        {
            myStatus = LotteryTicketStatus.Winning;
        }

        internal void RunLosing()
        {
            myStatus = LotteryTicketStatus.Losing;
        }

        internal void RunCancel()
        {
            myStatus = LotteryTicketStatus.Cancel;
        }
        public override string ToString()
        {
            return "lottery ticket of Lottery number: " + LotteryNumber + " ticket number is: "+myID+" status: "+myStatus;
        }
    }
}
