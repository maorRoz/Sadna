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
        MarketAnswer SignUp(string name,string address,string password,string creditCard);
        MarketAnswer SignIn(string name, string password);
        MarketAnswer ViewCart();
		MarketAnswer EditCartItem(string store, string product, int quantity , double unitPrice);
        MarketAnswer RemoveFromCart(string store, string product, double unitPrice);
        MarketAnswer ViewUsers();
        MarketAnswer GetControlledStoreNames();
        MarketAnswer GetUserDetails();

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
        NullEmptyFewDataGiven,

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

    public enum ViewUsersStatus
    {
        Success,
        DidntEnterSystem
    }

    public enum GetControlledStoresStatus
    {
        Success,
        DidntEnterSystem
    }

    public enum GetUserDetails
    {
        Success,
        DidntEnterSystem
    }

    public enum BrowseMarketStatus
    {
        DidntEnterSystem,
        DidntLoggedSystem
    }

    public enum ManageMarketSystem
    {
        NotSystemAdmin
    }



	
    
}
