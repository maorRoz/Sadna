﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.OrderPool
{
    class OrderAnswer : MarketAnswer
    {
        public OrderAnswer(OrderStatus status, string answer) : base((int)status, answer)
        {

        }

        public OrderAnswer(OrderItemStatus status, string answer) : base((int)status, answer)
        {

        }
    }
}
