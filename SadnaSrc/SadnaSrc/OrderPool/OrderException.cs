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
        public OrderException(int status,string message) : base(status,message)
        {
        }


        protected override string GetModuleName()
        {
            return "OrderPool";
        }

        protected override string WrapErrorMessageForDb(string message)
        {
            // TODO: implement a better message wrapper like in UserException
            return " " + message;
        }
    }
}
