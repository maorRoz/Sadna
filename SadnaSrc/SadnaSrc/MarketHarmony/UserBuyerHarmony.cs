using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.OrderPool;
using SadnaSrc.UserSpot;

namespace SadnaSrc.MarketHarmony
{
    public class UserBuyerHarmony : IUserBuyer
    {
        private UserService _userService;
        public UserBuyerHarmony(ref IUserService userService)
        {
            _userService = (UserService) userService;
            //TODO: we should do something about user which hasn't entered the system here
        }

        public OrderItem[] CheckoutAll()
        {
            ValidUserEnteredSystem();
            CartItem[] userCart = _userService.MarketUser.Cart.GetCartStorage();
            return ConvertCartItemsToOrderItems(userCart);
        }
        public void EmptyCart()
        {
            _userService.MarketUser.Cart.EmptyCart();
        }

        public OrderItem[] CheckoutFromStore(string store)
        {
            ValidUserEnteredSystem();
            CartItem[] userCart = _userService.MarketUser.Cart.GetCartStorage(store);
            return ConvertCartItemsToOrderItems(userCart);
        }

        public void EmptyCart(string store)
        {
            _userService.MarketUser.Cart.EmptyCart(store);
        }

        public OrderItem CheckoutItem(string itemName, string store, int quantity, double unitPrice)
        {
            ValidUserEnteredSystem();
            CartItem userItem = _userService.CheckoutItem(itemName,store,quantity,unitPrice);
            return CnvertCartItemToOrderItem(userItem);
        }

        private static OrderItem[] ConvertCartItemsToOrderItems(CartItem[] cart)
        { 
            OrderItem[] result = new OrderItem[cart.Length];
            for (int i = 0; i < cart.Length; i++)
            {
                result[i] = ConvertCartItemToOrderItem(cart[i]);
            }
            return result;
        }
        private static OrderItem ConvertCartItemToOrderItem(CartItem item)
        {
            return new OrderItem(item.Store, item.Name, item.FinalPrice, item.Quantity);
        }

        private void ValidUserEnteredSystem()
        {
            if (_userService.MarketUser == null)
            {
                throw new UserException(EditCartItemStatus.DidntEnterSystem,
                    "Cannot let User which hasn't entered the system to Purchase items from store!");
            }
        }
        private bool IsRegisteredUser()
        {
            return _userService.MarketUser != null && _userService.MarketUser.IsRegisteredUser();
        }

        public string GetAddress()
        {
            return IsRegisteredUser() ? 
                ((RegisteredUser) _userService.MarketUser).Address : null;
        }

        public string GetName()
        {
            return IsRegisteredUser() ? 
                ((RegisteredUser)_userService.MarketUser).Name : null;
        }

        public string GetCreditCard()
        {
            return IsRegisteredUser() ?
                ((RegisteredUser)_userService.MarketUser).CreditCard : null;
        }

        public void CleanSession()
        {
            _userService.CleanSession();
        }

        //only for unit tests of OrderPool(and not for Integration)
        public void LogInBuyer(string userName,string password)
        {
            _userService.EnterSystem();
            _userService.SignIn(userName, password);
        }
        //only for unit tests of OrderPool(and not for Integration)
        public void MakeGuest()
        {
            _userService.EnterSystem();
        }
    }
}
