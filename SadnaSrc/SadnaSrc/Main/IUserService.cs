using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.Main
{
    public interface IUserService
    {
        MarketAnswer EnterSystem();
        MarketAnswer SignUp(string name,string address,string password);

        MarketAnswer SignIn(string name, string password);

        void CleanSession(); // only for BlackBox tests

    }

    public enum EnterSystemStatus
    {
        Success
    }

    public enum SignUpStatus
    {
        Success,
        DidntEnterSystem,
        SignedUpAlready,
        TakenName,
        NullEmptyDataGiven,

    }

    public enum SignInStatus
    {
        Success,
        MistakeTipGiven,
        DidntEnterSystem,
        SignedInAlready,
        NoUserFound,
        NullEmptyDataGiven,
    }
}
