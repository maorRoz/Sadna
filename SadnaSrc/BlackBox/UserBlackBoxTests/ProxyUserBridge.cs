using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace BlackBox
{
    class ProxyUserBridge : IUserBridge
    {
        public IUserBridge real;

        public ProxyUserBridge()
        {
            real = null;
        }

        public MarketAnswer EnterSystem()
        {
            if (real != null)
            {
                return real.EnterSystem();
            }

            throw new NotImplementedException();
        }

        public MarketAnswer SignUp(string name, string address, string password, string creditCard)
        {
            if (real != null)
            {
                return real.SignUp(name, address, password, creditCard);
            }
            throw new NotImplementedException();
        }

        public MarketAnswer SignIn(string name, string password)
        {
            if (real != null)
            {
                return real.SignIn(name, password);
            }
            throw new NotImplementedException();
        }

        public MarketAnswer ViewCart()
        {
            if (real != null)
            {
                return real.ViewCart();
            }

            throw new NotImplementedException();
        }

        public MarketAnswer EditCartItem(string store, string product, int quantity,double unitPrice)
        {
            if (real != null)
            {
                return real.EditCartItem(store, product, quantity, unitPrice);
            }
            throw new NotImplementedException();
        }

        public MarketAnswer RemoveFromCart(string store, string product, double unitPrice)
        {
            if (real != null)
            {
                return real.RemoveFromCart(store, product, unitPrice);
            }
            throw new NotImplementedException();
        }

        public IUserService GetUserSession()
        {
            if (real != null)
            {
                return real.GetUserSession();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void CleanSession()
        {
            if (real != null)
            {
                real.CleanSession();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public void CleanMarket()
        {
            if (real != null)
            {
                real.CleanMarket();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

    }
}