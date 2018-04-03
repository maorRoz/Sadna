﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.StoreCenter
{
    public class LotteryTicket
    {
        LotterySaleManagmentTicket MyManager;
        int IntervalStart;
        int IntervalEnd;
        LotteryTicketStatus myStatus;
        public LotteryTicket(LotterySaleManagmentTicket _MyManager, int _IntervalStart, int _IntervalEnd)
        {
            MyManager = _MyManager;
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
    }
}
