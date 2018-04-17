using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.Walleter
{
    class WalleterException : MarketException
    {
        public WalleterException(WalleterStatus status,string message) : base((int)status,message)
        {
        }


        protected override string GetModuleName()
        {
            return "Walleter";
        }

        protected override string WrapErrorMessageForDb(string message)
        {

            return " " + message;
        }
    }
}
