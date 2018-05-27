using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketData;

namespace SadnaSrc.UserSpot
{
    public class EditCartItemSlave
    {

        private readonly User _user;

        public UserAnswer Answer { get; private set; }

        private int userID;

        public EditCartItemSlave(User user)
        {
            Answer = null;
            _user = user;
            userID = user?.SystemID ?? -1;
        }
        public void EditCartItem(string store, string product, int quantity,double unitPrice)
        {
            try
            {
                MarketLog.Log("UserSpot", "User " + userID + " attempting to edit his cart item: " + product + " from store: " + store + " ...");
                CartItem toEdit = ApproveModifyCart(store, product, unitPrice);
                MarketLog.Log("UserSpot",
                    "User " + userID + " found cart item: " + product + " from store: " + store +
                    ". proceeding for the edit...");
                _user.Cart.EditCartItem(toEdit, quantity);
                MarketLog.Log("UserSpot",
                    "User " + userID + "successfully edited cart item: " + product + " from store: " + store + " ...");
                Answer = new UserAnswer(EditCartItemStatus.Success, "Edit Cart Item has been successful!");
            }
            catch (UserException e)
            {
                Answer = new UserAnswer((EditCartItemStatus) e.Status, e.GetErrorMessage());
            }
            catch (DataException e)
            {
                Answer = new UserAnswer((EditCartItemStatus)e.Status,e.GetErrorMessage());
            }

        }


        private void ApproveEnetered()
        {
            if (_user != null) { return; }
            throw new UserException(EditCartItemStatus.DidntEnterSystem,
                "Edit Cart Item action has been requested by User which hasn't fully entered the system yet!");

        }


        private CartItem ApproveModifyCart(string store, string product, double unitPrice)
        {
            ApproveEnetered();
            CartItem found = _user.Cart.SearchInCart(store, product, unitPrice);
            if (found != null)
            {
                return found;
            }
            throw new UserException(EditCartItemStatus.NoItemFound,
                "Edit Cart Item operation found no item to modify!");

        }
    }
}
