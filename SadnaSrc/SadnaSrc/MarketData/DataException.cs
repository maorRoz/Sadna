using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.MarketData
{
    class DataException : Exception
    {
        private string errorMessage;
        public int Status { get;}

        public DataException()
        {
            errorMessage = "Connection with the system data centers has been lost, opertaion failed!";
            Status = 500;
        }

        public string GetErrorMessage()
        {
            return errorMessage;
        }
    }
}
