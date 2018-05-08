using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SadnaSrc.PolicyComponent
{
    public class NotOperator : Operator
    {
        public NotOperator(PolicyType type, string subject, PurchasePolicy cond1, PurchasePolicy cond2, int id) : base(type, subject, cond1, cond2, id)
        {
        }

        public override bool Evaluate(string username, string address, int quantity, double price)
        {
            return !_cond1.Evaluate(username, address, quantity, price);
        }

        public override string[] GetData()
        {
            return new[] { "" + ID, "NOT", "" + _cond1.ID };
        }
        public override string GetMyType()
        {
            return "NotOperator";
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            return obj.GetType() == GetType() && Equals((NotOperator)obj);
        }
        private bool Equals(NotOperator obj)
        {
            return obj.ID.Equals(ID) && obj.Subject.Equals(Subject)
                                     && obj.Type.Equals(Type) && obj._cond1.Equals(_cond1);
        }
    }


}
