using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    class LotteryTicket
    {
        LotterySaleManagmentTicket MyManager;
        int IntervalStart;
        int IntervalEnd;
        public LotteryTicket(LotterySaleManagmentTicket _MyManager, int _IntervalStart, int _IntervalEnd)
        {
            MyManager = _MyManager;
            IntervalStart = _IntervalStart;
            IntervalEnd = _IntervalEnd;
        }

        internal bool isWinning(int winningNumber)
        {
            return ((winningNumber <= IntervalEnd) && (winningNumber > IntervalStart));
        }

        internal void RunWinning()
        {
            throw new NotImplementedException();
        }

        internal void RunLosing()
        {
            throw new NotImplementedException();
        }

        internal void runCancel()
        {
            throw new NotImplementedException();
        }
    }
}
