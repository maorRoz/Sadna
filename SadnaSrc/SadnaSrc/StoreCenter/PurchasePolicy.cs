using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SadnaSrc.Main;

namespace SadnaSrc.StoreCenter
{
    class PurchasePolicy
    {
        private string _name;
        private int _minAmount;
        private int _maxAmount;
        private List<string> blocked;
        private List<string> bypass;
        private List<string> relevant;

        public PurchasePolicy(string name, int minAmount, int maxAmount)
        {
            _name = name;
            _minAmount = minAmount;
            _maxAmount = maxAmount;
            blocked = new List<string>();
            bypass = new List<string>();
            relevant = new List<string>();
        }

        public void Isvalid(int amount, string userDetails)
        {
            if (contains(blocked, userDetails))
                throw new StoreException(PurchasePolicyStatus.UserBlocked,
                    "This user is blocked from purchasing this product.");
            if(!contains(bypass, userDetails) || (relevant.Count > 0 && contains(relevant, userDetails)))
                chekcAmount(amount);
        }

        private bool contains(List<string> list, string details)
        {
            foreach (string str in list)
            {
                if (details.Contains(str))
                    return true;
            }

            return false;
        }

        private void chekcAmount(int amount)
        {
            if(amount < _minAmount || (amount > _maxAmount && _maxAmount != 0))
                throw new StoreException(PurchasePolicyStatus.AmountExceeded, "the requested amount doesn't fit the purchase policy!");
        }

        public void AddBlocked(string details)
        {
            blocked.Add(details);
        }

        public void Addbypass(string details)
        {
            bypass.Add(details);
        }

        public void AddRelevant(string details)
        {
            relevant.Add(details);
        }

        public void RemoveBlocked(string details)
        {
            blocked.Remove(details);
        }

        public void Removebypass(string details)
        {
            bypass.Remove(details);
        }

        public void RemoveRelevant(string details)
        {
            relevant.Remove(details);
        }
    }
}
