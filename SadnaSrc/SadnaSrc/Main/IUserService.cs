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

        MarketAnswer ViewCart();

        MarketAnswer EditCartItem(string store, string product, double unitPrice, string sale, int quantity);
        MarketAnswer RemoveFromCart(string store, string product, double unitPrice, string sale);

        void CleanGuestSession(); // only for tests
        void CleanSession(); // only for tests

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

    public enum ViewCartStatus
    {
        Success,
        DidntEnterSystem,
    }

    public enum EditCartItemStatus
    {
        Success,
        DidntEnterSystem,
        NoItemFound,
        ZeroNegativeQuantity
    }

    public enum RemoveFromCartStatus
    {
        Success,
        DidntEnterSystem,
        NoItemFound
    }

    public enum PromoteStoreManager
    {
        NoUserFound,
        InvalidPromotion
    }
}
