﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.UserSpot
{
    public class UserAnswer : MarketAnswer
    {
        public UserAnswer(EnterSystemStatus status, string answer,int userId) : base((int)status, answer,new[]{userId.ToString()})
        {

        }
        public UserAnswer(SignUpStatus status, string answer) : base((int) status,answer)
        {

        }

        public UserAnswer(SignInStatus status, string answer, int loggedId,string state) : base((int) status, answer,new[] {loggedId.ToString(),state})
        {

        }

        public UserAnswer(SignInStatus status, string answer) : base((int)status, answer)
        {

        }

        public UserAnswer(ViewCartStatus status, string answer,string[] cartItems) : base((int)status, answer,cartItems)
        {

        }

        public UserAnswer(EditCartItemStatus status, string answer) : base((int)status, answer)
        {

        }

        public UserAnswer(RemoveFromCartStatus status, string answer) : base((int)status, answer)
        {

        }
    }
}
