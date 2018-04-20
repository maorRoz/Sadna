using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    class SignUpSlave
    {
        private readonly UserServiceDL userDB;

        public UserAnswer Answer { get; private set; }

        public SignUpSlave()
        {
            userDB = UserServiceDL.Instance;
            Answer = null;
        }


    }
}
