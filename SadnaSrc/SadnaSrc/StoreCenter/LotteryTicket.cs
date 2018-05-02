﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.StoreCenter
{
    public class LotteryTicket
    {
        public double IntervalStart { get; set; }
        public double IntervalEnd { get; set; }
        public string LotteryNumber { get; set; }
        public string myID { get; set; }
        public LotteryTicketStatus myStatus { get; set; }
        public int UserID { get; set; }
        

        public double Cost { get; set; }
        private static int globalLotteryTicketID = FindMaxLotteryTicketId();
        public LotteryTicket(string _LotteryNumber, double _IntervalStart, double _IntervalEnd, double cost, int _userID)
        {
            LotteryNumber = _LotteryNumber;
            myID = GetLotteryTicketID();
            IntervalStart = _IntervalStart;
            IntervalEnd = _IntervalEnd;
            Cost = cost;
            myStatus = LotteryTicketStatus.Waiting;
            UserID = _userID;
        }
        public LotteryTicket(string _myID, string _LotteryNumber, double _IntervalStart, double _IntervalEnd, double cost, int _userID)
        {
            LotteryNumber = _LotteryNumber;
            myID = _myID;
            IntervalStart = _IntervalStart;
            IntervalEnd = _IntervalEnd;
            Cost = cost;
            myStatus = LotteryTicketStatus.Waiting;
            UserID = _userID;
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
        public override bool Equals(object obj)
        {
            if (obj.GetType() != GetType())
                return false;
            return Equals((LotteryTicket)obj);
        }
        private bool Equals(LotteryTicket obj)
        {
            StoreSyncerImplementation handler = StoreSyncerImplementation.GetInstance();
            return (obj.IntervalStart == IntervalStart &&
                    obj.IntervalEnd == IntervalEnd &&
                    obj.LotteryNumber == LotteryNumber &&
                    obj.myID == myID &&
                    EnumStringConverter.PrintEnum(obj.myStatus).Equals(EnumStringConverter.PrintEnum(myStatus)) &&
                    obj.UserID == UserID
                    );
        }
        public override string ToString()
        {
            return "lottery ticket of Lottery number: " + LotteryNumber + " ticket number is: " + myID + " status: " + myStatus;
        }

        public override int GetHashCode()
        {
            var hashCode = 1900018414;
            hashCode = hashCode * -1521134295 + IntervalStart.GetHashCode();
            hashCode = hashCode * -1521134295 + IntervalEnd.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LotteryNumber);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(myID);
            hashCode = hashCode * -1521134295 + myStatus.GetHashCode();
            hashCode = hashCode * -1521134295 + UserID.GetHashCode();
            return hashCode;
        }
        public object[] GetTicketValuesArray()
        {

            return new object[]
            {
                myID,
                LotteryNumber,
                IntervalStart,
                IntervalEnd,
                myStatus,
                UserID
            };
        }
        public string[] GetTicketStringValues()
        {
            StoreSyncerImplementation handler = StoreSyncerImplementation.GetInstance();
            return new[]
            {
                "'" + myID + "'",
                "'" + LotteryNumber + "'",
                "'" + IntervalStart + "'",
                "'" + IntervalEnd + "'",
                "'" + Cost + "'",
                "'" + EnumStringConverter.PrintEnum(myStatus) + "'",
                "'" + UserID + "'"
            };
        }
        private static string GetLotteryTicketID()
        {
            globalLotteryTicketID++;
            return "T" + globalLotteryTicketID;
        }
        private static int FindMaxLotteryTicketId()
        {
            StoreDL DL = StoreDL.GetInstance();
            LinkedList<string> list = DL.getAllLotteryTicketIDs();
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