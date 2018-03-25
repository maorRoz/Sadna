using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    public class UserPolicy
    {
        public enum State { RegisteredUser,SystemAdmin, StoreManager }
        private readonly State _state;


        public UserPolicy(State state)
        {
            _state = state;
        }

        public State GetState()
        {
            return _state;
        }

        public string GetStateString()
        {
            switch (_state)
            {
                case State.StoreManager:
                    return "StoreManager";
                case State.SystemAdmin:
                    return "SystemAdmin";
                default:
                    return "RegisteredUser";
            }
        }

    }
}
