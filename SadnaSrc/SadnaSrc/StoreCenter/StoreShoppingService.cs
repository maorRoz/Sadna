using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.MarketHarmony;

namespace SadnaSrc.StoreCenter
{
    class StoreShoppingService : IStoreShoppingService
    {
        private IUserShopper _shopper;
        private readonly ModuleGlobalHandler storeLogic;
        public StoreShoppingService(IUserShopper shopper)
        {
            _shopper = shopper;
            storeLogic = ModuleGlobalHandler.GetInstance();
        }
        public MarketAnswer OpenStore(string storeName, string address)
        {
            try
            {
                _shopper.ValidateCanOpenStore();
                Store newStore = new Store(storeLogic.GetNextStoreId(), storeName, address);
                storeLogic.AddStore(newStore);
                _shopper.AddOwnership(storeName);
                return new StoreAnswer(OpenStoreStatus.Success, "Store " + storeName + " has been opened successfully");
            }
            catch (StoreException e)
            {
                return new StoreAnswer((OpenStoreStatus)e.Status, "Store " + storeName + " creation has been denied. " +
                                                 "something is wrong with adding a new store of that type. . Error message has been created!");
            }
            catch (MarketException e)
            {
                return new StoreAnswer(OpenStoreStatus.InvalidUser,
                    "User validation as store owner has been failed. only registered users can open new stores. Error message has been created!");
            }
        }

        public MarketAnswer ViewStoreInfo(string store)
        {
            try
            {
                _shopper.ValidateCanBrowseMarket();
                //storeToFind =  StoreDL.searchStoreInfo(store);
                return new StoreAnswer(ViewStoreStatus.Success,"Store info has been successfully granted!",null );
            }
            catch (StoreException e)
            {
                return new StoreAnswer((ViewStoreStatus)e.Status, "Store . " +
                                              "something is wrong with viewing "+ store + " info by customers . . Error message has been created!");
            }
            catch (MarketException e)
            {
                return new StoreAnswer(ViewStoreStatus.InvalidUser,
                    "User validation as valid customer has been failed . only valid users can browse market. . Error message has been created!");
            }
        }

        public MarketAnswer ViewStoreStock(string store)
        {
            try
            {
                _shopper.ValidateCanBrowseMarket();
                //storeToFind =  StoreDL.searchStoreStock(store);
                return new StoreAnswer(ViewStoreStatus.Success, "Store stock has been successfully granted!", null);
            }
            catch (StoreException e)
            {
                return new StoreAnswer((ViewStoreStatus)e.Status, "Store . " +
                                            "something is wrong with viewing " + store + " stock by customers. . Error message has been created!");
            }
            catch (MarketException e)
            {
                return new StoreAnswer(ViewStoreStatus.InvalidUser,
                    "User validation as valid customer has been failed . only valid users can browse market. . Error message has been created!");
            }
        }

        public MarketAnswer AddProductToCart(string store, string productName,int quantity)
        {
            try
            {
                //PorductToFind =  StoreDL.searchProductInStore(store,productName,"Immediate");
              //  _shopper.AddToCart(productName,store,quantity);
                return new StoreAnswer(AddProductStatus.Success, quantity +" "+ productName +" from "+store+ "has been" +
                                                                 " successfully added to the user's cart!");
            }
            catch (StoreException e)
            {
                return new StoreAnswer((AddProductStatus)e.Status, "There is no product or store or quantity of that type in the market." +
                                                                  " request has been denied. . Error message has been created!");
            }
            catch (MarketException e)
            {
                return new StoreAnswer(ViewStoreStatus.InvalidUser,
                    "User validation as valid customer has been failed . only valid users can browse market. . Error message has been created!");
            }
        }

        public MarketAnswer AddLotteryTicket(string store, string productName,double amountToPay)
        {
            try
            {
                //PorductToFind =  StoreDL.searchProductInStore(store,productName,"Immediate");
                //  _shopper.AddToCart(productName,store,quantity);
                return new StoreAnswer(AddLotteryTicketStatus.Success, amountToPay + "has been paid to a " + productName + " lottery ticket" +
                                                                       " from " + store + "has been" +" successfully!");
            }
            catch (StoreException e)
            {
                return new StoreAnswer((AddProductStatus)e.Status, "There is no product,ticket,store or quantity of that type in the market." +
                                                                   " request has been denied. . Error message has been created!");
            }
            catch (MarketException e)
            {
                return new StoreAnswer(ViewStoreStatus.InvalidUser,
                    "User validation as valid customer has been failed . only valid users can browse market. . Error message has been created!");
            }
        }
    }
}
