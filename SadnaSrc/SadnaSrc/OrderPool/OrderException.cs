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
        public OrderException(string message) : base(message)
        {
        }


        protected override string GetModuleName()
        {
            return "OrderPool";
        }

        protected override string GetErrorMessage(string message)
        {
            // TODO: implement a better message wrapper like in UserException
            return "OrderException: " + message;
        }
    }
}
