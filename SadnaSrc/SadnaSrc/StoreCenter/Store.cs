using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.StoreCenter
{
    /**
     * this class is describing a single store, the managmnet of all the stores + implementing StoreService is done in StoreCenter
     **/
    public class Store
    {
        public string SystemId { get; }
        private readonly LinkedList<PurchasePolicy> purchasePolicy;
        private bool isActive;
        public string Name { get; set; }
        public string Address { private get; set; }

        private static int storeIdCounter = -1;

        public Store(string name, string address)
        {
            SystemId = GetNextStoreId();
            Name = name;
            Address = address;
            purchasePolicy = new LinkedList<PurchasePolicy>();
            isActive = true;
        }
        public Store(string id, string name, string address)
        {
            SystemId = id;
            Name = name;
            Address = address;
            purchasePolicy = new LinkedList<PurchasePolicy>();
            isActive = true;
        }

        public Store(string id, string name, string address, string active)
        {
            SystemId = id;
            Name = name;
            Address = address;
            purchasePolicy = new LinkedList<PurchasePolicy>();
            GetActiveFromString(active);
        }

        private void GetActiveFromString(string active)
        {
            if (active.Equals("Active"))
                isActive = true;
            isActive = false;
        }

        private string GetStringFromActive()
        {
            return isActive ? "Active" : "InActive";
        }
        public MarketAnswer CloseStore()
        {
            if (isActive)
            {
                isActive = false;
                StoreDL handler = StoreDL.Instance;
                handler.EditStore(this);
                return new StoreAnswer(StoreEnum.Success, "store " + SystemId + " closed");
            }
            return new StoreAnswer(StoreEnum.CloseStoreFail, "store " + SystemId + " is already closed");
        }
        private bool Equals(Store obj)
        {
            return obj.SystemId.Equals(SystemId) && obj.Name.Equals(Name) && obj.Address.Equals(Address);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType() == GetType() && Equals((Store)obj);
        }

        public override int GetHashCode()
        {
            var hashCode = 501679021;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SystemId);
            hashCode = hashCode * -1521134295 + EqualityComparer<LinkedList<PurchasePolicy>>.Default.GetHashCode(purchasePolicy);
            hashCode = hashCode * -1521134295 + isActive.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Address);
            return hashCode;
        }
        public object[] GetStoreArray()
        {
            return new object[]
            {
                SystemId,
                Name,
                Address,
                GetStringFromActive()
            };
        }
        private static string GetNextStoreId()
        {
            if (storeIdCounter == -1)
            {
                storeIdCounter = StockSyncher.GetMaxEntityID(StoreDL.Instance.GetAllStoresIDs());
            }
            storeIdCounter++;
            return "S" + storeIdCounter;
        }

    }
}
