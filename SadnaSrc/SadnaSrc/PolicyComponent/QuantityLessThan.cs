﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public class QuantityLessThan : Condition
    {
        public QuantityLessThan(PolicyType type, string subject, string value) : base(type, subject, value)
        {
        }

        public override bool Evaluate(string username, string address, int quantity, double price)
        {
            return quantity < Int32.Parse(_value);
        }
    }
}
