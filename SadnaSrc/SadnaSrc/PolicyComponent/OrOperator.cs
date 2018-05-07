using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public class OrOperator : Operator
    {
        public OrOperator(PolicyType type, string subject, PurchasePolicy cond1, PurchasePolicy cond2, int id) : base(type, subject, cond1, cond2, id)
        {
        }

        public override bool Evaluate(string username, string address, int quantity, double price)
        {
            return _cond1.Evaluate(username, address, quantity, price) ||
                    _cond2.Evaluate(username, address, quantity, price);
        }

        public override string[] GetData()
        {
            return new[] { "" + ID, "OR", "" + _cond1.ID, "" + _cond2.ID };
        }
    }
}
