using System;
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

        public UserAnswer(EnterSystemStatus status, string answer) : base((int) status, answer)
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

	    public UserAnswer(ViewStoresStatus status, string answer, string[] storeNames) : base((int)status, answer,storeNames)
	    {

	    }

	    public UserAnswer(ViewStoresStatus status, string answer) : base((int)status, answer)
	    {

	    }

		public UserAnswer(ViewUsersStatus status, string answer, string[] usersNames) : base((int)status, answer, usersNames)
        {

        }

        public UserAnswer(ViewUsersStatus status, string answer) : base((int)status, answer)
        {

        }

        public UserAnswer(GetControlledStoresStatus status, string answer, string[] usersNames) : base((int)status, answer, usersNames)
        {

        }

        public UserAnswer(GetControlledStoresStatus status, string answer) : base((int)status, answer)
        {

        }

        public UserAnswer(GetStoreManagerPoliciesStatus status, string answer, string[] policies) : base((int) status,
          answer,policies)
        {

        }

        public UserAnswer(GetStoreManagerPoliciesStatus status, string answer) : base((int)status,
          answer)
        {

        }
        public UserAnswer(GetUserDetails status, string answer, string[] usersNames) : base((int)status, answer, usersNames)
        {

        }

        public UserAnswer(GetUserDetails status, string answer) : base((int)status, answer)
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
