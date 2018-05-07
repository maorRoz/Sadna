using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public abstract class PurchasePolicy
    {
        public int ID;
        public readonly PolicyType Type;
        public readonly string Subject;

        public PurchasePolicy(PolicyType type, string subject, int id)
        {
            ID = id;
            Type = type;
            Subject = subject;
        }

        public abstract bool Evaluate(string username, string address, int quantity, double price);

        public abstract string[] GetData();
    }
}
