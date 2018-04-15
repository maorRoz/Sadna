using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.OrderPool
{
    class OrderException : MarketException
    {
        public OrderException(GiveDetailsStatus status, string message) : base((int)status, message)
        {
        }
        public OrderException(OrderStatus status,string message) : base((int)status,message)
        {
        }

        public OrderException(OrderItemStatus status, string message) : base((int)status, message)
        {
        }

        public OrderException(LotteryOrderStatus status, string message) : base((int)status, message)
        {
        }

        protected override string GetModuleName()
        {
            return "OrderPool";
        }

        protected override string WrapErrorMessageForDb(string message)
        {
            return "OrderException: " + message;
        }
    }
}
