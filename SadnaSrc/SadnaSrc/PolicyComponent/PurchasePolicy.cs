using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public abstract class PurchasePolicy
    {
        public int _id;
        public readonly PolicyType _type;
        public readonly string _subject;

        public PurchasePolicy(PolicyType type, string subject, int id)
        {
            _id = id;
            _type = type;
            _subject = subject;
        }

        public abstract bool Evaluate(string username, string address, int quantity, double price);

        public abstract string[] GetData();
    }
}
