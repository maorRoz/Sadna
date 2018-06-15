using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.MarketData
{
    class DataException : MarketException
    {

        public DataException() : base(500,
            "Connection with the system data centers has been lost," +
            " opertaion failed!")
        {
        }

        public DataException(string message) : base(600,message)
        {
        }

        protected override string GetModuleName()
        {
            return "Db Connection Lost";
        }
    }
}
