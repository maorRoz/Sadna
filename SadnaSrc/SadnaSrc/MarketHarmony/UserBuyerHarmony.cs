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
            CartItem itemFromStore = _userService.MarketUser.Cart.SearchInCart(store, itemName, unitPrice);
            if (itemFromStore == null)
            {
                throw new UserException(EditCartItemStatus.NoItemFound,
                    "No item by that info  has been found in the user cart!");
            }

            if (quantity > itemFromStore.Quantity)
            {
                throw new UserException(EditCartItemStatus.NoItemFound,
                    "User doesn't have that amount of this product in the cart!");
            }

            if (quantity <= 0)
            {
                throw new UserException(EditCartItemStatus.NoItemFound,
                    "Checkout amount needs to be a positive number!");
            }

            return ConvertCartItemToOrderItem(itemFromStore, quantity);
        }

        public void RemoveItemFromCart(string itemName, string store, int quantity, double unitPrice)
        {
            CartItem itemFromStore = _userService.MarketUser.Cart.SearchInCart(store, itemName, unitPrice);
            if (itemFromStore == null || quantity > itemFromStore.Quantity)
            {
                throw new UserException(EditCartItemStatus.NoItemFound,
                    "No item by that info or quantity has been found in the user cart!");
            }
            if (quantity == itemFromStore.Quantity)
            {
                _userService.MarketUser.Cart.RemoveFromCart(itemFromStore);
            }
            else if (quantity < 0)
            {
                throw new UserException(EditCartItemStatus.NoItemFound,
                    "Can't remove a negative number of items!");
            }
            else
            {
                _userService.MarketUser.Cart.EditCartItem(itemFromStore, -quantity);
            }
        }

        private static OrderItem[] ConvertCartItemsToOrderItems(CartItem[] cart)
        { 
            OrderItem[] result = new OrderItem[cart.Length];
            for (int i = 0; i < cart.Length; i++)
            {
                result[i] = ConvertCartItemToOrderItem(cart[i], cart[i].Quantity);
            }
            return result;
        }
        private static OrderItem ConvertCartItemToOrderItem(CartItem item, int quantity)
        {
            return new OrderItem(item.Store, item.Name, item.FinalPrice, quantity);
        }

        private void ValidUserEnteredSystem()
        {
            if (_userService.MarketUser == null)
            {
                throw new UserException(EditCartItemStatus.DidntEnterSystem,
                    "Cannot let User which hasn't entered the system to Purchase items from store!");
            }
        }

        public void ValidateRegisteredUser()
        {
            if (!IsRegisteredUser())
            {
                throw new UserException(EditCartItemStatus.DidntEnterSystem,
                    "Cannot let User which hasn't registerd to the system to Purchase items of that kind!");
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
