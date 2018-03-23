using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    class Guest : User
    {
        protected UserPolicy policies;
        int systemID;
        protected Cart shoppingCart;
        public Guest()
        {
            policies = new UserPolicy();
            Random random = new Random();
            systemID = random.Next(1000, 10000);
        }
        public bool addUserPolicy(UserPolicy policy)
        {
            //         throw new UserException("Guest " + systemID + " rejected the addition of UserPolicy " + policy.getName());
            return false;
        }

        public bool removeUserPolicy(UserPolicy policy)
        {
            //        throw new UserException("Guest "+systemID+" rejected the removal of UserPolicy "+policy.getName());
            return false;
        }
    }
}
