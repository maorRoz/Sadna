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
	    MarketAnswer GetStoreManagerPolicies(string store);
        MarketAnswer GetUserDetails();
	    MarketAnswer GetAllStores();
	   
    }

    public enum EnterSystemStatus
    {
        Success,
        NoDB = 500
    }

    public enum SignUpStatus
    {
        Success,
        DidntEnterSystem,
        SignedUpAlready,
        TakenName,
        NullEmptyFewDataGiven,
        NoDB = 500,
        BadInput = 600

    }

    public enum SignInStatus
    {
        Success,
        MistakeTipGiven,
        DidntEnterSystem,
        SignedInAlready,
        NoUserFound,
        NullEmptyDataGiven,
        NoDB = 500,
        BadInput = 600
    }

    public enum ViewCartStatus
    {
        Success,
        DidntEnterSystem,
        NoDB = 500,
        BadInput = 600
    }

	public enum EditCartItemStatus
    {
        Success,
        DidntEnterSystem,
        NoItemFound,
        ZeroNegativeQuantity,
        NoDB = 500,
        BadInput = 600
    }

    public enum RemoveFromCartStatus
    {
        Success,
        DidntEnterSystem,
        NoItemFound,
        NoDB = 500,
        BadInput = 600
    }

    public enum ViewUsersStatus
    {
        Success,
        DidntEnterSystem,
        NoDB = 500,
        BadInput = 600
    }

    public enum GetControlledStoresStatus
    {
        Success,
        DidntEnterSystem,
        NoDB = 500,
        BadInput = 600
    }

	public enum ViewStoresStatus
	{
		Success,
		NoPermission,
	    NoDB = 500,
	    BadInput = 600
    }

	public enum GetStoreManagerPoliciesStatus
	{
		Success,
		DidntEnterSystem,
	    NoDB = 500,
	    BadInput = 600
    }
    public enum GetUserDetailsStatus
    {
        Success,
        DidntEnterSystem,
        NoDB = 500,
        BadInput = 600
    }

    public enum BrowseMarketStatus
    {
        DidntEnterSystem,
        DidntLoggedSystem,
        NoDB = 500,
        BadInput = 600
    }

    public enum ManageMarketSystem
    {
        NotSystemAdmin,
        NoDB = 500,
        BadInput = 600
    }
}
