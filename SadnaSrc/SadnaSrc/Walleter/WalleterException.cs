﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.Walleter
{
    class WalleterException : MarketException
    {
        public WalleterException(string message) : base(message)
        {
        }


        protected override string GetModuleName()
        {
            return "Walleter";
        }

        protected override string GetErrorMessage(string message)
        {
            // TODO: implement a better message wrapper like in UserException
            return " " + message;
        }
    }
}
