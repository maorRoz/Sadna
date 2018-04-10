using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.UserSpot
{
    public class StatePolicy
    {
        public enum State { RegisteredUser,SystemAdmin }
        private readonly State _state;


        public StatePolicy(State state)
        {
            _state = state;
        }

        public State GetState()
        {
            return _state;
        }

        public string GetStateString()
        {
            return _state == State.SystemAdmin ? "SystemAdmin" : "RegisteredUser";
        }

        public static State GetStateFromString(string state)
        {
            return state.Equals("SystemAdmin") ? State.SystemAdmin : State.RegisteredUser;
        }

    }
}
