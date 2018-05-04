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
        public bool IsActive { get; set; }
        public string Name { get; set; }
        public string Address { private get; set; }

        private static int storeIdCounter = FindMaxStoreId();

        public Store(string name, string address)
        {
            SystemId = GetNextStoreId();
            Name = name;
            Address = address;
            purchasePolicy = new LinkedList<PurchasePolicy>();
            IsActive = true;
        }
        public Store(string id, string name, string address)
        {
            SystemId = id;
            Name = name;
            Address = address;
            purchasePolicy = new LinkedList<PurchasePolicy>();
            IsActive = true;
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
                IsActive = true;
            IsActive = false;
        }

        public string GetStringFromActive()
        {
            return IsActive ? "Active" : "InActive";
        }
        public MarketAnswer CloseStore()
        {
            if (IsActive)
            {
                IsActive = false;
                StoreDL handler = StoreDL.GetInstance();
                handler.EditStore(this);
                return new StoreAnswer(StoreEnum.Success, "store " + SystemId + " closed");
            }
            return new StoreAnswer(StoreEnum.CloseStoreFail, "store " + SystemId + " is already closed");
        }
        public bool Equals1(object obj)
        {
            if (obj.GetType() == GetType())
            {
                return ((Store)obj).SystemId.Equals(SystemId) &&
                    ((Store)obj).Name.Equals(Name) &&
                    ((Store)obj).Address.Equals(Address);
            }
            return false;

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
            hashCode = hashCode * -1521134295 + IsActive.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Address);
            return hashCode;
        }
        public string[] GetStoreStringValues()
        {
            return new[]
            {
                "'" + SystemId + "'",
                "'" + Name + "'",
                "'" + Address + "'",
                "'" + GetStringFromActive() + "'"
            };
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
        private static int FindMaxStoreId()
        {
            StoreDL DL = StoreDL.GetInstance();
            LinkedList<string> list = DL.GetAllStoresIDs();
            int max = -5;
            int temp = 0;
            foreach (string s in list)
            {
                temp = Int32.Parse(s.Substring(1));
                if (temp > max)
                {
                    max = temp;
                }
            }
            return max;
        }
        private static string GetNextStoreId()
        {
            storeIdCounter++;
            return "S" + storeIdCounter;
        }

    }
}
