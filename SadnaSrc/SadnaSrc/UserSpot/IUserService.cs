﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    public interface IUserService
    {
        void EnterSystem();
        void SignUp(string Name,string Address,string Password);

        //   void SignIn();

    }
}
