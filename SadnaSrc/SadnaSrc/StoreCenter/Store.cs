using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;
using SadnaSrc.UserSpot;

namespace SadnaSrc.StoreCenter
{
    /**
     * this class is describing a single store, the managmnet of all the stores + implementing StoreService is done in StoreCenter
     **/
    public class Store
    {
        public int SystemId { get; }
        private Stock stock { get; set; }
        private User Owner;
        private LinkedList<User> OtherOwners;
        private LinkedList<User> Managers;
        private LinkedList<PurchesPolicy> purchesPolicy;
        private bool isActive { get; set; }

        public Store(User _Owner, int id)
        {
            SystemId = id;
            stock = new Stock();
            Owner = _Owner;
            OtherOwners = new LinkedList<User>();
            Managers = new LinkedList<User>();
            purchesPolicy = new LinkedList<PurchesPolicy>();
            isActive = true;
        }
        public StoreAnswer addProduct(Product product, int quantity)
        {
            return stock.addProductToStock(product, quantity);
        }
        public StoreAnswer PromoteToOwner(User user)
        {
            if (!OtherOwners.Contains(user))
            {
                OtherOwners.AddLast(user);
                return new StoreAnswer(StoreEnum.Success, "user " + user.SystemID + " has been premoted to be a owner of store " + SystemId);
            }
            return new StoreAnswer(StoreEnum.AddStoreOwnerFail, "user " + user.SystemID + " is Already a owner of the store " + SystemId);
        }
        public StoreAnswer PromoteToManager(User user)
        {
            if (!Managers.Contains(user))
            {
                Managers.AddLast(user);
                return new StoreAnswer(StoreEnum.Success, "user " + user.SystemID + " has been premoted to be a manager of store " + SystemId);
            }
            return new StoreAnswer(StoreEnum.AddStoreManagerFail, "user " + user.SystemID + " is Already a manager of the store " + SystemId);
        }
        internal bool IsOwner(User user)
        {
            if (isActive)
            {
                bool result = (user.SystemID == Owner.SystemID);
                if (result) { return result; }
                foreach (User other in OtherOwners)
                {
                    result = result || (user.SystemID == other.SystemID);
                }
                return result;
            }
            return false;
        }
        public StoreAnswer CloseStore()
        {
            if (isActive)
            {
                isActive = false;
                return new StoreAnswer(StoreEnum.Success, "store " + SystemId + " closed");
            }
            return new StoreAnswer(StoreEnum.Success, "store " + SystemId + " is alrady closed");
        }
        public bool isStoreActive()
        {
            return isActive;
        }
        public StoreAnswer addDiscountToProduct(Product p, int _discountCode, discountTypeEnum _discountType, DateTime _startDate, DateTime _EndDate, int _DiscountAmount, bool _presenteges)
        {
            Discount discount = new Discount(_discountCode, _discountType, _startDate, _EndDate, _DiscountAmount, _presenteges);
            return stock.addDiscountToProduct(p, discount);
        }
        //need to think about it. Don't shure it's the best way to go
        /** public StoreAnswer editProductDiscount(Product p, int _discountCode, discountTypeEnum _discountType, DateTime _startDate, DateTime _EndDate, int _DiscountAmount, bool _presenteges)
         {
             Discount discount = new Discount(_discountCode, _discountType, _startDate, _EndDate, _DiscountAmount, _presenteges);
             return stock.editProductDiscount(p, discount);
         }**/
        public StoreAnswer removeProduct(Product p)
        {
            return stock.removeProductFromStock(p);
        }
        public StoreAnswer removeDiscountToProduct(Product p)
        {
            return stock.removeDiscountToProduct(p);
        }
        public StoreAnswer Edi(Product p)
        {
            return stock.removeDiscountToProduct(p);
        }
        public StoreAnswer ChangeProductPurchesWayToImmidiate (Product p)
        {
            return ChangeProductPurchesWay(p, PurchesEnum.IMMIDIATE);
        }
        public StoreAnswer ChangeProductPurchesWayToLottery(Product p)
        {
            return ChangeProductPurchesWay(p, PurchesEnum.LOTTERY);
        }
        private StoreAnswer ChangeProductPurchesWay(Product p, PurchesEnum purches)
        {
            return stock.addPurchesWayToProduct(p, purches);
        }
    }
}
